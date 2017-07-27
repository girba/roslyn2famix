﻿namespace Fame.Fm3
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Common;
    using Internal;

    //TODO
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

        public new MetaDescription GetOwner()
        {
            return OwningMetaDescription;
        }

        public bool IsPrimitive()
        {
            Debug.Assert(Type != null, Fullname);
            return Type.IsPrimitive();
        }

        public override void CheckContraints(Warnings warnings)
        {
            // TODO
            //if (isContainer)
            //    if (isMultivalued)
            //    {
            //        warnings.add("Container must be single-values", this);
            //    }
            //if (opposite != null)
            //    if (this != opposite.getOpposite())
            //    {
            //        warnings.add("Opposites must match", this);
            //    }
            //if (!MetaRepository.isValidName(getName()))
            //{
            //    warnings.add("Name must be alphanumeric", this);
            //}
            //if (!Character.isLowerCase(getName().charAt(0)))
            //{
            //    warnings.add("Name should start lowerCase", this);
            //}
            //if (type == null)
            //    warnings.add("Missing type", this);
            //if (declaringClass == null)
            //    warnings.add("Must have an owning class", this);
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

            try
            {
                if (IsMultivalued)
                {
                    return PrivateReadAllMultivalued(element);
                }

                object result = Read(element);
                return result == null ? new List<object>() : new List<object> {result};
            }
            catch (Exception)
            {
                throw;
            }
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
            try
            {
                if (IsMultivalued)
                {
                    Access.Write(element, values);
                }
                else
                {
                    Debug.Assert(values.Count <= 1, values + " for " + Fullname);
                    foreach (T first in values)
                    {
                        Access.Write<T>(element, first);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    // TODO: Why is this required? Look like in C# it is not necessary.
    public class ArrayWrapper<T,TE> : Collection<T>, IEnumerable where TE : ICollection<T>
    {
        public readonly TE Array;

        public ArrayWrapper(TE array)
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
        private static readonly long serialVersionUID = 6381545746042993261L;
        public PropertyDescription Property;
        public object Object;

        public ReadingPropertyFailed(Exception e, PropertyDescription property, object @object) : base(e)
        {
            Property = property;
            Object = @object;
        }
    }

    public class AssertionError : Exception
    {
	    public AssertionError()
	    {
		    
	    }

        protected AssertionError(Exception e)
        {
            // TODO
            throw new NotImplementedException();
        }
    }

    public class NoAccessorException : Exception
    {
        private static readonly long serialVersionUID = -2828241533257508153L;

        public PropertyDescription Outer()
        {
            // TODO
            //return PropertyDescription.this;
            throw new NotImplementedException();
        }
    }
}