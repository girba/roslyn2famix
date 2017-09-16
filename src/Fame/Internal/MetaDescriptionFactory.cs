namespace Fame.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using Fm3;

	/// <summary>
	/// Creates metamodel element for <seealso cref="FameDescriptionAttribute"/> annotated class.
	/// </summary>
	public class MetaDescriptionFactory
	{
		private readonly Type _base;
		private readonly List<PropertyFactory> _childFactories;
		private MetaDescription _instance;
		private readonly MetaRepository _repository;

		public MetaDescriptionFactory(Type @base, MetaRepository repository)
		{
			_base = @base;
			_repository = repository;
			_childFactories = new List<PropertyFactory>();
		}

		public MetaDescription CreateInstance()
		{
			Debug.Assert(AnnotationPresent);

			_instance = new MetaDescription(Name());

			return _instance;
		}

		private void CreatePropertyFactories()
		{
			foreach (var method in _base.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				if (method.GetCustomAttribute(typeof(FamePropertyAttribute)) != null)
				{
					PropertyFactory factory = new PropertyFactory(new MethodAccess(method), _repository);
					_childFactories.Add(factory);
				}
			}

			foreach (var field in _base.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
			{
				if (field.GetCustomAttribute(typeof(FamePropertyAttribute)) != null)
				{
					PropertyFactory factory = new PropertyFactory(new FieldAccess(field), _repository);
					_childFactories.Add(factory);
				}
			}
		}

		private void CreatePropertyInstances()
		{
			foreach (PropertyFactory factory in _childFactories)
			{
				PropertyDescription property = factory.CreateInstance();
				_instance.AddOwnedAttribute(property);
				property.OwningMetaDescription = _instance;
			}
		}

		private FameDescriptionAttribute Annotation => (FameDescriptionAttribute)_base.GetCustomAttribute(typeof(FameDescriptionAttribute));

		private void InitializeBaseClass()
		{
			_instance.BaseClass = _base;
		}

		public void InitializeInstance()
		{
			InitializePackage();
			CreatePropertyFactories();
			CreatePropertyInstances();
			InitializeSuperclass();
			InitializeProperties();
			InitializeBaseClass();
			InitializeIsAbstract();
		}

		private void InitializeIsAbstract()
		{
			bool isAbstract = _base.IsAbstract;
			_instance.IsAbstract = isAbstract;
		}

		private void InitializePackage()
		{
			PackageDescription pack = _repository.InitializePackageNamed(PackageName());
			_instance.Package = pack;
			pack.AddElement(_instance);
		}

		private void InitializeProperties()
		{
			foreach (PropertyFactory factory in _childFactories)
			{
				factory.InitializeInstance();
			}
		}

		private void InitializeSuperclass()
		{
			_repository.With(_base.BaseType);
			MetaDescription superclass = _repository.GetDescription(_base.BaseType);
			_instance.SuperClass = superclass;
		}

		// /////////////////////////////////////////

		public virtual bool AnnotationPresent => _base.GetCustomAttribute(typeof(FameDescriptionAttribute)) != null;

		/// <summary>
		/// Answer either the name given in the annotation, or the class name.
		/// </summary>
		private string Name()
		{
			string name = Annotation.Value;
			if (name.Equals("*"))
			{
				name = _base.Name;
			}

			return name;
		}

		private string PackageName()
		{
			Type curr = _base;
			while (curr != null)
			{
				var p = (FamePackageAttribute)curr.GetCustomAttribute(typeof(FamePackageAttribute));
				if (p != null)
				{
					return p.Value;
				}

				curr = curr.DeclaringType;
			}


			// TODO: Java Package = C# Namespace --> Is there a way in C# to have attributes on namspaces?
			// At the moment the assembly's attributes are used
			var j = _base.Assembly;
			var pa = (FamePackageAttribute)j.GetCustomAttribute(typeof(FamePackageAttribute));
			if (pa != null)
			{
				return pa.Value;
			}

			string fullName = j.FullName;
			return fullName.Substring(fullName.LastIndexOf('.') + 1).ToUpper();
		}
	}
}