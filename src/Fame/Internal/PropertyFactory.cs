namespace Fame.Internal
{
	using System;
	using System.Diagnostics;
	using Fm3;

	/// <summary>
	/// Creates metamodel element for <seealso cref="FamePropertyAttribute"/> annotated method.
	/// </summary>

	public class PropertyFactory
	{
		private readonly Access _base;
		private PropertyDescription _instance;
		private readonly MetaRepository _repository;

		protected internal PropertyFactory(Access accessor, MetaRepository repository)
		{
			_base = accessor;
			_repository = repository;
		}

		private Type BaseType()
		{
			Type type = Annotation.Type;
			if (type == typeof(void))
			{ // ie, no type has been specified
				type = _base.ElementType;
			}

			return type;
		}

		protected internal PropertyDescription CreateInstance()
		{
			Debug.Assert(AnnotationPresent);

			_instance = new PropertyDescription(Name);

			return _instance;
		}

		protected internal FamePropertyAttribute Annotation => _base.Annotation;

		private string Name
		{
			get
			{
				string name = Annotation.Name;
				if (name.Equals("*"))
				{
					name = _base.Name;

					if (name.StartsWith("Is", StringComparison.Ordinal))
					{
						name = char.ToLower(name[2]) + name.Substring(3);
					}

					if (name.StartsWith("get_", StringComparison.Ordinal))
					{
						name = char.ToLower(name[4]) + name.Substring(5);
					}
				}

				return name;
			}
		}

		private void InitializeAccessors()
		{
			_instance.Access = _base;
		}

		protected internal void InitializeInstance()
		{
			InitializeType();
			InitializeIsMultivalues();
			InitializeIsDerived();
			InitializeIsContainer();
			InitializeAccessors();
			InitializeOpposite();
		}

		private void InitializeIsContainer()
		{
			bool isContainer = Annotation.Container;
			_instance.IsContainer = isContainer;
		}

		private void InitializeIsDerived()
		{
			bool isDerived = Annotation.Derived;
			_instance.IsDerived = isDerived;
		}

		private void InitializeIsMultivalues()
		{
			bool isMultivalued = Multivalued;
			_instance.IsMultivalued = isMultivalued;
		}

		private void InitializeOpposite()
		{
			string oppositeName = OppositeName();
			if (oppositeName == null)
			{
				return;
			}

			PropertyDescription opposite = _instance.Type.AttributeNamed(oppositeName);
			Debug.Assert(opposite != null, "Opposite not found: " + oppositeName + " in " + _instance.Type);  // TODO: Assert crashes because opposite reference is not found (e.g. attributes --> class)
			_instance.Opposite = opposite;
		}

		private void InitializeType()
		{
			_repository.With(BaseType());
			MetaDescription type = _repository.GetDescription(BaseType());
			_instance.Type = type;
		}

		protected internal bool AnnotationPresent => _base.AnnotationPresent;

		private bool Multivalued => _base.Multivalued;

		private string OppositeName()
		{
			string name = Annotation.Opposite;
			if (name.Length == 0)
			{
				name = null;
			}

			return name;
		}
	}
}