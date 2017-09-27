//  Copyright (c) 2007-2008 Adrian Kuhn <akuhn(a)iam.unibe.ch>
//  
//  This file is part of 'Fame (for Java)'.
//  
//  'Fame (for Java)' is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or (at your
//  option) any later version.
//  
//  'Fame (for Java)' is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
//  or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
//  License for more details.
//  
//  You should have received a copy of the GNU Lesser General Public License
//  along with 'Fame (for Java)'. If not, see <http://www.gnu.org/licenses/>.

namespace Fame
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using Common;
	using Fm3;
	using Internal;
	using Parser;

	/// <summary>
	/// A group of elements that conform to the same meta-model.
	/// 
	/// @author Adrian Kuhn, 2007-2008
	/// </summary>
	public class Repository
	{
		private ICollection<object> _elements;
		private MetaRepository _metamodel;

		/// <summary>
		/// Resolves the fully qualified name of an element, or <code>null</code> if
		/// {@linkplain INamed} is not implemented. Nested elements must implement
		/// {@linkplain INested} in order to resolve their full name.
		/// 
		/// @see INamed
		/// @see INested
		/// 
		/// </summary>
		/// <param name="element"> should implement INamed and maybe also Owned.</param>
		/// <returns>may return null</returns>
		public static string Fullname(object element)
		{
			if (!(element is INamed))
				return null;

			string name = ((INamed)element).Name;
			Debug.Assert(name != null);

			if (!(element is INested))
				return name;

			object owner = ((INested)element).GetOwner();
			if (owner == null)
				return name;

			string ownerName = Fullname(owner);
			Debug.Assert(ownerName != null);

			return ownerName + "." + name;
		}

		/// <summary>
		/// Creates an empty tower of models. The tower has three layers: both this
		/// and the meta-layer are initially empty, whereas the topmost layer is
		/// initialized with a new FM3 package.
		/// </summary>
		public Repository() : this(new MetaRepository(MetaRepository.CreateFm3()))
		{
		}

		/// <summary>
		/// Creates a empty layer with the given meta-layer.
		/// If the specified parameter is <code>null</code>, creates a
		/// self-describing layer(ie an meta-metamodel).
		/// </summary>
		/// <param name="metamodel"></param>
		public Repository(MetaRepository metamodel)
		{
			// allow null in order to boot-strap self-described meta-models
			_metamodel = metamodel ?? (MetaRepository)this;
			_elements = new HashSet<object>();
		}

		public void Accept(IParseClient visitor)
		{
			var runner = new RepositoryVisitor(this, visitor);
			Task.Run((Action) runner.Run);
		}

		public void Add(object element, params object[] more)
		{
			Add(element);
			foreach (object o in more)
			{
				Add(o);
			}
		}

		public void AddAll(ICollection<object> all)
		{
			foreach (object o in all)
			{
				Add(o);
			}
		}

		/// <summary>
		/// Collect all elements with the specified class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ICollection<T> All<T>()
		{
			return _elements.OfType<T>().ToArray();
		}

		public virtual void Add(object element)
		{
			Debug.Assert(element != null);

			if (!_elements.Contains(element))
			{
				_elements.Add(element);

				MetaDescription meta = _metamodel.GetDescription(element.GetType());
				Debug.Assert(meta != null, element.GetType().ToString());

				foreach (PropertyDescription property in meta.AllAttributes())
				{
					if (!property.IsPrimitive())
					{
						bool isRoot = property.Type.IsRoot();

						foreach (object value in property.ReadAll(element))
						{
							Debug.Assert(value != null, property.Fullname);

							if (!(isRoot && (value is string || value is bool || value.IsNumber())))
							{
								try
								{
									Add(value);
								}
								catch (ClassNotMetadescribedException)
								{
									throw new ElementInPropertyNotMetadescribed(property);
								}
							}
						}
					}
				}
			}
		}

		public MetaDescription DescriptionOf(object element)
		{
			try
			{
				return _metamodel.GetDescription(element.GetType());
			}
			catch (ClassNotMetadescribedException)
			{
				throw new ObjectNotDescribed(element);
			}
		}

		/// <summary>
		/// Exports all elements as MSE-formatted string.
		/// </summary>
		/// <returns></returns>
		public string ExportMSE()
		{
			StringWriter stream = new StringWriter();
			this.ExportMSE(stream);
			return stream.ToString();
		}

		public void ImportMSE(InputSource input)
		{
			Importer importer = new Importer(GetMetamodel());
			importer.ReadFrom(input);
			AddAll(importer.Result.GetElements());
		}

		public void ImportMSEFile(string name)
		{
			ImportMSE(InputSource.FromFilename(name));
		}

		public void ImportMSE(string content)
		{
			ImportMSE(InputSource.FromString(content.ToCharArray()));
		}

		public void ImportMSE(Stream stream)
		{
			ImportMSE(InputSource.FromInputStream(stream));
		}

		public void ExportMSEFile(string filename)
		{
			Accept(new MsePrinter(File.CreateText(filename)));
		}

		public void ExportMSE(TextWriter stream)
		{
			Accept(new MsePrinter(stream));
		}

		public ICollection<object> GetElements()
		{
			return _elements;
		}

		public MetaRepository GetMetamodel()
		{
			return _metamodel;
		}

		public T NewInstance<T>(string qname)
		{
			MetaDescription m = _metamodel.DescriptionNamed(qname);
			Debug.Assert(m != null);

			T element = (T)m.NewInstance();
			Add(element);

			return element;
		}

		public T Read<T>(string propertyName, object element)
		{
			MetaDescription m = DescriptionOf(element);
			PropertyDescription p = m.AttributeNamed(propertyName);
			T value = (T) p.Read(element);

			return value;
		}

		public void Write(string propertyName, object element, params object[] values)
		{
			MetaDescription m = DescriptionOf(element);
			PropertyDescription p = m.AttributeNamed(propertyName);
			p.WriteAll(element, values);
			if (p.HasOpposite())
			{
				foreach (object v in values)
				{
					p.Opposite.WriteAll(v, new [] {element});
				}
			}
		}

		public int Size()
		{
			return _elements.Count;
		}

		public bool IsEmpty()
		{
			return _elements.Count == 0;
		}

		public int Count(Type kind)
		{
			return _elements.Count(e => e.GetType() == kind); // TODO: Check if correct
		}
	}

	public class ElementInPropertyNotMetadescribed : AssertionError
	{
		private static readonly long SerialVersionUid = 1661566781761376913L;
		public PropertyDescription Property;

		public ElementInPropertyNotMetadescribed(PropertyDescription property)
		{
			Property = property;
		}
	}

	public class ObjectNotDescribed : AssertionError
	{
		private static readonly long SerialVersionUid = -3268614108861432571L;
		public object Object;

		public ObjectNotDescribed(object o)
		{
			Object = o;
		}
	}

	public class ClassNotMetadescribedException : AssertionError
	{
		private static long SerialVersionUid = 894469439304582534L;
		private Type _cls;

		public Type GetTheClass()
		{
			return _cls;
		}
		public ClassNotMetadescribedException(Type cls)
		{
			_cls = cls;
		}
	}
}