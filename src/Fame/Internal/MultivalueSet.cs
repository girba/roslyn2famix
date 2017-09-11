namespace Fame.Internal
{
	using System.Collections;
	using System.Collections.Generic;

	public abstract class MultivalueSet<T> : IEnumerable<T> //: AbstractSet<T>  // TODO: Required in C# ?... would be ISet<T>
	{
		private readonly ISet<T> _values = new HashSet<T>();

		public bool Add(T e)
		{
			if (e == null)
			{
				throw new System.ArgumentNullException(nameof(e), "Element must not be null.");
			}

			if (!_values.Contains(e))
			{
				_values.Add(e);
				Opposite = e;

				return true;
			}

			return false;
		}

		protected internal abstract void ClearOpposite(T e);

		public bool Contains(object o)
		{
			return _values.Contains((T)o);
		}

		public bool Empty => _values.Count == 0;

		public bool Remove(object o)
		{
			if (o == null)
			{
				throw new System.ArgumentNullException(nameof(o), "Element must not be null.");
			}

			if (_values.Contains((T)o))
			{
				_values.Remove((T)o);
				ClearOpposite((T)o);

				return true;
			}

			return false;
		}

		protected internal abstract T Opposite { set; }

		public int Size()
		{
			return _values.Count;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}