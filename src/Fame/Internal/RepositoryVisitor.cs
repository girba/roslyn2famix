namespace Fame.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Common;
	using Fm3;
	using Parser;

	/// <summary>
	/// Accepts a IParseClient on a repository.
	/// </summary>
	public class RepositoryVisitor
	{
		private readonly Repository _repo;
		private IDictionary<object, int> _index;
		private readonly IParseClient _visitor;

		public RepositoryVisitor(Repository repo, IParseClient visitor)
		{
			_repo = repo;
			_visitor = visitor;
			_index = new Dictionary<object, int>();
			var serial = 1;

			foreach (var each in repo.GetElements())
			{
				_index[each] = serial++;
			}
		}

		private void AcceptElement(object each)
		{
			MetaDescription meta = _repo.DescriptionOf(each);
			_visitor.BeginElement(meta.Fullname);
			_visitor.Serial(GetSerialNumber(meta, each));

			// XXX there can be more than one children property per element!
			PropertyDescription childrenProperty = this.childrenProperty(meta);
			Debug.Assert(childrenProperty == null || !childrenProperty.IsContainer, "Children property must not be a container!");

			foreach (PropertyDescription property in sortAttributes(meta.AllAttributes().ToArray()))
			{
				ICollection<object> values = property.ReadAll(each);
				if (property.IsDerived)
				{
					continue;
				}

				if (property.IsContainer)
				{
					continue;
				}

				if (property.Type == MetaDescription.BOOLEAN && !property.IsMultivalued && values.Count > 0)
				{
					if (!(bool)values.First())
					{
						continue;
					}
				}

				if (values.Count > 0)
				{
					_visitor.BeginAttribute(property.Name);
					bool isPrimitive = property.Type.IsPrimitive();
					bool isRoot = property.Type.IsRoot();
					bool isComposite = property == childrenProperty;

					foreach (object value in values)
					{
						if (value is MetaDescription)
						{
							MetaDescription m = (MetaDescription)value;

							if (m.IsPrimitive() || m.IsRoot())
							{
								_visitor.Reference(m.Name);
								continue;
							}
						}

						if (isPrimitive || isRoot && (value is string || value is bool || value.IsNumber()))
						{
							_visitor.Primitive(value);
						}
						else
						{
							if (isComposite)
							{
								AcceptElement(value);
							}
							else
							{
								int serial = GetSerialNumber(property, value);
								_visitor.Reference(serial);
							}
						}
					}

					_visitor.EndAttribute(property.Name);
				}
			}

			_visitor.EndElement(meta.Fullname);
		}

		/// <summary>
		/// Element which was found by browsing the object-graph, but which
		/// was not known to the repository yet
		/// /// </summary>		
		public class UnknownElementError : AssertionError
		{
			internal const long SerialVersionUid = -6761765027263961388L;
			public object Unknown;
			public Element Element;

			public UnknownElementError(Element element, object unknown) : base("Unknown element: " + unknown + " found via description: " + element.Fullname)
			{
				Unknown = unknown;
				Element = element;
			}
		}

		/// <summary>
		/// Returns the serial number of a given element
		/// </summary>
		/// <param name="description"></param>
		/// <param name="element">an element of the current repository.</param>
		/// <exception cref="AssertionError"> if the given object is not an element of the
		/// current repository. This may happen, if a meta-described property refers
		/// to objects that are not contained in the repository. Repositories must
		/// be complete under transitive closure, that is, all objects reachable from
		/// elements in a repository must be elements of the repository themselves. 
		/// </exception>
		/// <returns> a unique serial number.</returns>
		private int GetSerialNumber(Element description, object element)
		{
			if (!_index.ContainsKey(element))
			{
				throw new UnknownElementError(description, element);
			}

			return _index[element];
		}

		private void AcceptVisitor()
		{
			_visitor.BeginDocument();
			ICollection<object> elements = rootElements(_repo);
			elements = removeBuiltinMetaDescriptions(elements);

			foreach (object each in elements)
			{
				AcceptElement(each);
			}

			_visitor.EndDocument();
		}

		private PropertyDescription childrenProperty(MetaDescription meta)
		{
			return meta.Attributes.FirstOrDefault(a => a.IsComposite());
		}

		private ICollection<object> removeBuiltinMetaDescriptions(ICollection<object> elements)
		{
			ISet<object> copy = new HashSet<object>(elements);
			copy.Remove(MetaDescription.OBJECT);
			copy.Remove(MetaDescription.STRING);
			copy.Remove(MetaDescription.NUMBER);
			copy.Remove(MetaDescription.BOOLEAN);
			copy.Remove(MetaDescription.DATE);

			return copy;
		}

		private ICollection<object> rootElements(Repository m)
		{
			return m.GetElements().Where(each => m.DescriptionOf(each).ContainerPropertyOrNull()?.Read(each) == null).ToArray();
		}

		public void Run()
		{
			Debug.Assert(_index != null, "Can not run the same visitor twice.");

			// TODO: Run in new thread
			AcceptVisitor();
			_index = null;
		}

		private ICollection<PropertyDescription> sortAttributes(ICollection<PropertyDescription> properties)
		{
			List<PropertyDescription> sorted = new List<PropertyDescription>(properties);
			sorted.Sort(new PropertyDescriptionComparer());

			return sorted;
		}

		private class PropertyDescriptionComparer : IComparer<PropertyDescription>
		{
			public int Compare(PropertyDescription a, PropertyDescription b)
			{
				string a0 = a?.Name;
				string b0 = b?.Name;

				if (a0 != null && a0.Equals(b0))
				{
					return 0;
				}

				if (a0 != null && a0.Equals("name"))
				{
					return -1;
				}

				if (b0 != null && b0.Equals("name"))
				{
					return +1;
				}

				if (a != null && b != null && !a.IsComposite() && b.IsComposite())
				{
					return -1;
				}

				if (a != null && b != null && a.IsComposite() && !b.IsComposite())
				{
					return +1;
				}

				return string.Compare(a0, b0, StringComparison.Ordinal);
			}
		}
	}
}