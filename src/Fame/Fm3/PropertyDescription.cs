namespace Fame.Fm3
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using Common;
	using Internal;

	[FamePackage("FM3")]
    [FameDescription("Property")]
    public class PropertyDescription : Element
    {
        public PropertyDescription()
        {
        }

        public PropertyDescription(string name) : base(name)
        {
        }

        private MetaDescription _declaringClass;
        [FameProperty(Name = "class", Opposite = "attributes", Container = true)]
        public MetaDescription OwningMetaDescription
        {
            get
            {
                return _declaringClass;
            }
            set
            {
                _declaringClass = value;
                value.AddOwnedAttribute(this);
            }
        }

        [FameProperty(Name = "package", Opposite = "extensions")]
        public PackageDescription ExtendingPackage { get; set; }

        [FameProperty]
        public bool IsContainer { get; set; }

        [FameProperty]
        public bool IsDerived { get; set; }

        [FameProperty]
        public bool IsMultivalued { get; set; }

        [FameProperty(Opposite = "opposite")]
        public PropertyDescription Opposite { get; set; }

        [FameProperty]
        public MetaDescription Type { get; set; }

        public Access Access { private get; set; }

        public new string Name { get; set; }

        public bool HasOpposite()
        {
            return Opposite != null;
        }

        [FamePropertyWithDerived]
        public bool IsComposite()
        {
            return HasOpposite() && Opposite.IsContainer;
        }

        public override Element GetOwner()
        {
            return OwningMetaDescription;
        }

        public bool IsPrimitive()
        {
            Debug.Assert(Type != null, Fullname);
            return Type.IsPrimitive();
        }

        public override void CheckConstraints(Warnings warnings)
        {
	        if (IsContainer)
	        {
		        if (IsMultivalued)
		        {
			        warnings.Add("Container must be single-values", this);
		        }
	        }

	        if (Opposite != null)
	        {
		        if (this != Opposite.Opposite)
		        {
			        warnings.Add("Opposites must match", this);
		        }
	        }

	        if (!MetaRepository.IsValidName(Name))
	        {
		        warnings.Add("Name must be alphanumeric", this);
	        }

	        if (!char.IsLower(Name[0]))
	        {
		        warnings.Add("Name should start lowerCase", this);
	        }

	        if (Type == null)
	        {
		        warnings.Add("Missing type", this);
	        }

	        if (_declaringClass == null)
	        {
		        warnings.Add("Must have an owning class", this);
	        }
		}

		public void SetComposite(bool composite)
        {
            Debug.Assert(Opposite != null);
            Opposite.IsContainer = composite;
        }

        public object Read(object element)
        {
            Debug.Assert(Access != null);
            return Access.Read(element);
        }

        public ICollection<object> ReadAll(object element)
        {
            if (Access == null) throw new NoAccessorException();
            Debug.Assert(element != null, "Trying to read property (" + this + ") from null");

	        if (IsMultivalued)
	        {
		        return PrivateReadAllMultivalued(element);
	        }

	        object result = Read(element);

	        return result == null ? new List<object>() : new List<object> {result};
        }

        private ICollection<object> PrivateReadAllMultivalued(object element)
        {
            ICollection<object> all;
            object read = Read(element);
            if (read == null) return new List<object>();
            if (read.GetType() == typeof(ICollection<object>))
            {
                var test = (ICollection<object>)read;
                all = new ArrayWrapper<object, ICollection<object>>(test);
            }
            else
            {
                all = (ICollection<object>) read;
            }

            Debug.Assert(!all.Contains(null), "Multivalued property contains null" + this);
            return all;
        }

        public void WriteAll<T>(object element, ICollection<T> values)
        {
            Debug.Assert(Access != null, Fullname);
	        if (IsMultivalued)
	        {
		        Access.Write(element, values);
	        }
	        else
	        {
		        Debug.Assert(values.Count <= 1, values + " for " + Fullname);
		        foreach (T first in values)
		        {
			        Access.Write(element, first);
			        return;
		        }
	        }
        }
    }

    // TODO: Why is this required? Look like in C# it is not necessary.
    public class ArrayWrapper<T, TCollection> : Collection<T>, IEnumerable where TCollection : ICollection<T>
    {
        public readonly TCollection Array;

        public ArrayWrapper(TCollection array)
        {
            Array = array;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return Array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public new int Count()
        {
            return Array.Count;
        }
    }

	public class ReadingPropertyFailed : AssertionError
	{
		private static readonly long SerialVersionUid = 6381545746042993261L;
		public PropertyDescription Property;
		public object Object;

		public ReadingPropertyFailed(Exception e, PropertyDescription property, object @object) : base(e)
		{
			Property = property;
			Object = @object;
		}
	}

	public class NoAccessorException : Exception
	{
		private static readonly long SerialVersionUid = -2828241533257508153L;

		// TODO
		//public PropertyDescription Outer()
		//{
			//return PropertyDescription.this;
		//}
	}
}