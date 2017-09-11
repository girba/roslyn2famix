namespace Fame.Internal
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Common;

	public class RuntimeElement
	{
		public class Slot : AbstractCollection
		{
			internal IList<object> Values;

			public Slot(string name)
			{
				Name = name;
				Values = new List<object>();
			}

			public override bool Add(object element)
			{
				Debug.Assert(element != null);

				Values.Add(element);

				return true;
			}

			public override void Clear()
			{
				Values.Clear();
			}

			public override bool Contains(object element)
			{
				return Values.Contains(element);
			}

			public string Name { get; set; }

			public override bool Empty => Values.Count == 0;

			public bool SingleValued => Values.Count <= 1;

			public override IEnumerator GetEnumerator()
			{
				return Values.GetEnumerator();
			}

			public override bool Remove(object element)
			{
				return Values.Remove(element);
			}

			public override int Size()
			{
				return Values.Count;
			}
		}

		public IDictionary<string, IList<object>> Slots { get; set; } = new Dictionary<string, IList<object>>();

		public string TypeName { get; set; }

		public object Read(string name)
		{
			if (!Slots.ContainsKey(name))
				return null;

			var slot = Slots[name];

			Debug.Assert(slot.Count <= 1);

			return slot.Count == 0 ? null : slot[0];
		}

		public virtual ICollection<object> ReadAll(string @string)
		{
			if (!Slots.ContainsKey(@string))
				return new List<object>();

			var slot = Slots[@string];

			return slot;
		}

		public IList<object> SlotNamed(string name)
		{
			if (!Slots.ContainsKey(name))
				return new List<object>();

			var slot = Slots[name];

			return slot;
		}

		public override string ToString()
		{
			return "a " + TypeName;
		}

		public void Write(string name, object value)
		{
			IList<object> slot;
			if (!Slots.ContainsKey(name))
			{
				slot = new List<object>();
				Slots[name] = slot;
			}
			else
			{
				slot = Slots[name];
			}

			Debug.Assert(slot.Count <= 1);

			if (slot.Count == 0)
			{
				slot.Add(value);
			}
			else
			{
				slot[0] = value;
			}
		}
	}
}