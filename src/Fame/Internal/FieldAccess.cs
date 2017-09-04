namespace Fame.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Common;
	using Fm3;

	public class FieldAccess : Access
	{
		private readonly FieldInfo _field;

		public FieldAccess(FieldInfo f) : base(f.FieldType)
		{
			_field = f;
			//this.field.Accessible = true; // TODO: Required? In C#?
		}

		public override FamePropertyAttribute Annotation => (FamePropertyAttribute)_field.GetCustomAttributes(typeof(FamePropertyAttribute)).FirstOrDefault();
		public override string Name => _field.Name;

		public override object Read(object element)
		{
			try
			{
				return _field.GetValue(element);
			}
			catch (ArgumentException ex)
			{
				throw new AssertionError(ex);
			}
			catch (FieldAccessException ex)
			{
				throw new AssertionError(ex);
			}
		}

		public override void Write(object element, object value)
		{
			try
			{
				_field.SetValue(element, Adapt(value));
			}
			catch (ArgumentException ex)
			{
				throw new AssertionError(ex);
				//throw Throw.exception(ex); // TODO
			}
			catch (FieldAccessException ex)
			{
				throw new AssertionError(ex);
				//throw Throw.exception(ex); // TODO
			}
		}

		private object Adapt(object element)
		{
			if (Array && element is ArrayWrapper<object, ICollection<object>>)
			{
				return ((ArrayWrapper<object, ICollection<object>>)element).Array;
			}

			if (Array && element is ICollection)
			{
				Type kind = _field.FieldType.GetElementType();

				if (kind == typeof(float))
				{
					ICollection<object> collection = (ICollection<object>)element;
					float[] array = new float[collection.Count];
					int index = 0;

					foreach (object each in collection)
					{
						array[index++] = (float)each;
					}

					return array;
				}

				return ((ICollection<object>)element).ToArray();
			}

			return element;
		}
	}
}