//  Copyright (c) 2007-2008 Adrian Kuhn <akuhn(a)iam.unibe.ch>
//  
//  This file is part of 'Fame (for .NET)'.
//  
//  'Fame (for .NET)' is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or (at your
//  option) any later version.
//  
//  'Fame (for .NET)' is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
//  or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
//  License for more details.
//  
//  You should have received a copy of the GNU Lesser General Public License
//  along with 'Fame (for .NET)'. If not, see <http://www.gnu.org/licenses/>.

namespace Fame
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using Common;
	using Dsl;
	using Fm3;
	using Internal;
	using Parser;

	/// <inheritdoc />
	/// <summary>
	/// A meta-model (ie elements conforming to FM3).
	/// </summary>
	public class MetaRepository : Repository
    {
	    private readonly Dictionary<string, Element> _bindings;
	    private readonly Dictionary<Type, MetaDescription> _classes;
	    private bool _immutable;

		public static MetaRepository CreateFm3()
	    {
		    MetaRepository mse = new MetaRepository(null);
			mse.With(typeof(MetaDescription));
			mse.With(typeof(Element));
		    mse.With(typeof(PackageDescription));
		    mse.With(typeof(PropertyDescription));
		    mse.AddAll(mse._bindings.Values.ToArray());
		    mse.SetImmutable();

		    return mse;
	    }

	    public static bool IsValidElementName(string name)
	    {
		    if (string.IsNullOrEmpty(name))
			    return false;

		    bool expectStart = true;
		    foreach (char ch in name)
		    {
			    if (expectStart)
			    {
				    if (char.IsLetter(ch))
				    {
					    expectStart = false;
				    }
				    else
				    {
					    return false;
				    }
			    }
			    else
			    {
				    if (ch == '.')
				    {
					    expectStart = true;
				    }
				    else
				    {
					    if (!char.IsLetterOrDigit(ch) && ch != '_')
						    return false;
				    }
			    }
		    }
		    return !expectStart;
	    }

	    public static bool IsValidName(string name)
	    {
		    return !string.IsNullOrEmpty(name) && char.IsLetter(name[0]); // && isAlphanumeric(string); //TODO: see utils
		}

		public MetaRepository() : this (CreateFm3())
	    {
	    }

	    public MetaRepository(MetaRepository metamodel) : base(metamodel)
	    {
		    _classes = new Dictionary<Type, MetaDescription>();
		    _bindings = new Dictionary<string, Element>();
	    }

	    public override void Add(object element)
	    {
			Debug.Assert(!_immutable);
			Debug.Assert(element is Element, element.GetType().ToString());
			if (element is MetaDescription)
			{
				MetaDescription meta = (MetaDescription)element;
				if (meta.IsPrimitive() || meta.IsRoot()) return;
			}

			base.Add(element);

			if (!_bindings.ContainsKey(((Element)element).Fullname))
				_bindings.Add(((Element)element).Fullname, (Element)element);
		}

	    public void AddClassDescription(Type cls, MetaDescription desc)
	    {
		    _classes.Add(cls, desc);
	    }

	    public ICollection<MetaDescription> AllClassDescriptions()
	    {
		    return All<MetaDescription>();
	    }

	    public ICollection<PackageDescription> AllPackageDescriptions()
	    {
		    return All<PackageDescription>();
	    }

	    public ICollection<PropertyDescription> AllPropertyDescriptions()
	    {
		    return All<PropertyDescription>();
	    }

	    public MetamodelBuilder Builder()
	    {
		    IParseClient client = new Importer(GetMetamodel(), this);
		    client = new DebugClient(client);
		    client = new ProtocolChecker(client);

		    return new MetamodelBuilder(client);
	    }

	    public Warnings CheckConstraints()
	    {
		    Warnings warnings = new Warnings();
		    foreach (object each in GetElements())
		    {
			    ((Element)each).CheckConstraints(warnings);
		    }
		    return warnings;
	    }

	    public MetaDescription DescriptionNamed(string fullname)
	    {
		    object found = Get<Element>(fullname);
		    if (found is MetaDescription)
				return (MetaDescription)found;

		    return null;
	    }

		/// <summary>
		/// Returns the Fame-class to which the given C#-class is connected in this metammodel.
		/// 
		/// throws Exception if the C#-class is not connected to any Fame-class in this metammodel.
		/// see #with(Class)
		/// </summary>
		/// <param name="jclass">a causally connected C#-class.</param>
		/// <returns>return a causally connected Fame-class.</returns>
		public MetaDescription GetDescription(Type jclass)
	    {
		    MetaDescription m = LookupPrimitive(jclass);
		    if (m != null) return m;

		    for (Type curr = jclass; curr != null; curr = curr.BaseType)
		    {
			    m = LookupClass(curr);
			    if (m != null) return m;
		    }
		    
			throw new Exception("Class not metadescribed: " + jclass);
		    //throw new ClassNotMetadescribedException(jclass);
	    }

	    private MetaDescription LookupPrimitive(Type jclass)
	    {
		    if (jclass.IsPrimitive)
		    {
			    return typeof(bool) == jclass ? MetaDescription.BOOLEAN : MetaDescription.NUMBER;
		    }

		    if (jclass.IsNumber())
		    {
			    return MetaDescription.NUMBER;
		    }

		    if (typeof(bool) == jclass)
		    {
			    return MetaDescription.BOOLEAN;
		    }

		    if (typeof(string) == jclass)
		    {
			    return MetaDescription.STRING;
		    }

		    if (typeof(char[]) == jclass)
		    {
			    return MetaDescription.STRING;
		    }

		    if (typeof(object) == jclass)
		    {
			    return MetaDescription.OBJECT;
		    }

		    return null;
		}

		public PackageDescription InitializePackageNamed(string name)
	    {
		    PackageDescription description = Get<PackageDescription>(name);
		    if (description == null)
		    {
			    description = new PackageDescription(name);
			    _bindings.Add(name, description);
		    }

		    return description;
	    }

	    public bool IsSelfDescribed()
	    {
		    return GetMetamodel() == this;
	    }

	    private MetaDescription LookupClass(Type jclass)
	    {
			if (_classes.ContainsKey(jclass))
				return _classes[jclass];

		    return null;
	    }

	    private MetaDescription LookupFull(Type jclass)
	    {
		    MetaDescription m = LookupPrimitive(jclass);
		    if (m == null) m = LookupClass(jclass);

		    return m;
	    }

	    public void SetImmutable()
	    {
		    _immutable = true;
	    }

		/// <summary>
		/// Processes the annotations of the given Java-class, creates an according
		/// Fame-class and establishes a Scenario-II connection between both. If such
		/// a connection is already present, nothing is done. 
		/// 
		/// @see FameDescription
		/// @see FamePackage
		/// @see FameProperty
		/// @throws AssertionError if the Java-class is not annotated.
		/// </summary>
		/// <param name="annotatedType">an annotated C#-class</param>
		public void With(Type annotatedType)
	    {
		    MetaDescription m = LookupFull(annotatedType);
		    if (m == null)
			{
			    MetaDescriptionFactory factory = new MetaDescriptionFactory(annotatedType, this);
			    if (factory.AnnotationPresent)
			    {
				    m = factory.CreateInstance();
				    _classes.Add(annotatedType, m);
				    factory.InitializeInstance();
				    _bindings.Add(m.Fullname, m);
			    }

			    if (!IsSelfDescribed())
			    { 
					// TODO explain? breaks meta-loop!
				    Add(m);
			    }
		    }

			Debug.Assert(m != null, annotatedType.ToString());
	    }

		/// <summary>
		/// Processes all given Java-classes.
		/// 
		/// @see #with(Class)
		/// @throws AssertionError if any o fthe C#-classes is not annotated.
		/// 
		/// </summary>
		/// <param name="jclasses">some annotated C#-classes.</param>
		public void WithAll(params Type[] jclasses)
	    {
		    foreach (Type jclass in jclasses)
		    {
			    With(jclass);
		    }
	    }

		public T Get<T>(string fullname) where T : Element
	    {
			if (_bindings.ContainsKey(fullname))
				return (T)_bindings[fullname];

		    return default(T);
	    }
    }
}