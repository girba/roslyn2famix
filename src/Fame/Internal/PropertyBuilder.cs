namespace Fame.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Fm3;
	using Parser;

	/// <inheritdoc />
	/// <summary>
	/// Reads MSE document as <seealso cref="T:Fame.Internal.RuntimeElement" /> elements.
	/// </summary>
	public class PrototypeBuilder : AbstractParserClient
	{
		private sealed class Ref
		{
			private readonly PrototypeBuilder _outerInstance;
			private readonly int _serial;

			public Ref(PrototypeBuilder outerInstance, int serial)
			{
				_outerInstance = outerInstance;
				_serial = serial;
			}

			public RuntimeElement Resolve()
			{
				RuntimeElement value = _outerInstance._index[_serial];
				Debug.Assert(value != null, _serial.ToString());

				return value;
			}
		}

		private Stack<RuntimeElement> _elemenStack;
		private Stack<IList<object>> _slotStack;
		private Dictionary<int, RuntimeElement> _index;
		private List<RuntimeElement> _elements;

		public override void BeginAttribute(string name)
		{
			var slot = _elemenStack.Peek().SlotNamed(name);
			_slotStack.Push(slot);
		}

		public override void BeginDocument()
		{
			_elemenStack = new Stack<RuntimeElement>();
			_slotStack = new Stack<IList<object>>();
			_index = new Dictionary<int, RuntimeElement>();
			_elements = new List<RuntimeElement>();
		}

		public override void BeginElement(string name)
		{
			RuntimeElement element = new RuntimeElement();
			element.TypeName = name;
			_elemenStack.Push(element);
		}

		public override void EndAttribute(string name)
		{
			_slotStack.Pop();
		}

		public override void EndDocument()
		{
			foreach (RuntimeElement each in _elements)
			{
				foreach (var values in each.Slots.Values)
				{
					for (int n = 0; n < values.Count; n++)
					{
						object value = values[n];

						if (value is Ref)
						{
							values[n] = ((Ref)value).Resolve();
						}
					}
				}
			}

			_elemenStack = null;
			_slotStack = null;
			_index = null;
		}

		public override void EndElement(string name)
		{
			RuntimeElement element = _elemenStack.Pop();
			if (_slotStack.Count != 0)
			{
				_slotStack.Peek().Add(element);
			}

			_elements.Add(element);
		}

		public virtual ICollection<RuntimeElement> Prototypes => _elements;

		public override void Primitive(object value)
		{
			Debug.Assert(value != null);

			_slotStack.Peek().Add(value);
		}

		public override void Reference(int serial)
		{
			_slotStack.Peek().Add(new Ref(this, serial));
		}

		public override void Reference(string n)
		{
			// TODO find nicer solution (discarded anyway, once moved to FAMIX 3)
			string name = n;
			if (name.EndsWith("Timestamp", StringComparison.Ordinal))
			{
				name = "Date";
			}

			_slotStack.Peek().Add(MetaDescription.PrimitiveNamed(name));
		}

		public override void Serial(int serial)
		{
			_index[serial] = _elemenStack.Peek();
		}
	}
}