namespace Fame.Common
{
	using System.Collections;

	public abstract class AbstractCollection : IEnumerable
	{
		public abstract bool Add(object element);
		public abstract void Clear();
		public abstract bool Contains(object element);
		public abstract bool Empty { get; }
		public abstract bool Remove(object element);
		public abstract int Size();
		public abstract IEnumerator GetEnumerator();
	}
}