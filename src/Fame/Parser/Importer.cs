namespace Fame.Parser
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using Fm3;

	/// <inheritdoc />
	/// <summary>
	/// Reads MSE document from input-stream (in one pass).
	/// </summary>
	public class Importer : AbstractParserClient
	{
		/// <summary>
		/// Retains information about parsing an element.
		/// </summary>
		private class Elem
		{
			/// <summary>
			/// Retains information about parsing an attribute.
			/// </summary>
			public class Attr
			{
				private readonly Elem _outerInstance;
				private readonly MetaRepository _metamodel;
				private readonly Index _index;

				/// <summary>
				/// Retains information about a dangling reference.
				/// </summary>
				public class Rem
				{
					private readonly Attr _outerInstance;
					private readonly int _pos;
				
					public Rem(Attr outerInstance)
					{
						_outerInstance = outerInstance;
						_pos = outerInstance._values.Count;
						outerInstance._openReferences++;
					}

					public void Resolve(object element)
					{
						Debug.Assert(element != null);

						_outerInstance._values[_pos] = element;
						_outerInstance._openReferences--;
						_outerInstance.MaybeEnd();
					}
				}

				private readonly IList<object> _values;
				private readonly string _attributeName;
				private int _openReferences;

				public Attr(string name, Elem outerInstance, MetaRepository metamodel, Index index)
				{
					_attributeName = name;
					_outerInstance = outerInstance;
					_metamodel = metamodel;
					_index = index;
					_openReferences = 0;
					_values = new List<object>();
				}

				public void Add(object value)
				{
					_values.Add(value);
				}

				public void EndAttribute(string name)
				{
					Debug.Assert(_attributeName == name);

					MaybeEnd();
				}

				private void MaybeEnd()
				{
					if (_openReferences > 0)
					{
						return;
					}

					object parent = _outerInstance.Element;
					MetaDescription meta = _metamodel.GetDescription(parent.GetType());
					PropertyDescription property = meta.AttributeNamed(_attributeName);

					Debug.Assert(property != null, "'" + _attributeName + "' in " + meta);

					property.WriteAll(parent, _values);
				}

				public void Reference(int serial)
				{
					var element = _index.Retrieve(serial);
					_values.Add(element ?? _index.KeepReminder(new Rem(this), serial));
				}
			}

			private readonly string _elementName;
			private Attr _currentAttribute;
			private object _actualElement;
			private readonly MetaRepository _metamodel;
			private readonly Index _index;

			public Elem(string name, MetaRepository metamodel, Index index)
			{
				_elementName = name;
				_actualElement = null;
				_currentAttribute = null;
				_metamodel = metamodel;
				_index = index;
			}

			public void Add(object value)
			{
				_currentAttribute.Add(value);
			}

			public void BeginAttribute(string name)
			{
				Debug.Assert(_currentAttribute == null);

				_currentAttribute = new Attr(name, this, _metamodel, _index);
			}

			public void EndAttribute(string name)
			{
				_currentAttribute.EndAttribute(name);
				_currentAttribute = null;
			}

			public object Element
			{
				get
				{
					if (_actualElement != null)
					{
						return _actualElement;
					}

					MetaDescription meta = _metamodel.DescriptionNamed(_elementName);
					Debug.Assert(meta != null, _elementName);
					_actualElement = meta.NewInstance();

					return _actualElement;
				}
			}

			public void Reference(int serial)
			{
				_currentAttribute.Reference(serial);
			}

			public void Serial(int serial)
			{
				_index.Assign(serial, Element);
			}
		}

		/// <summary>
		/// Keeps track of assigned indices and open references.
		/// </summary>
		private class Index
		{
			private readonly IDictionary<int, object> _serials = new SortedDictionary<int, object>();
			private readonly IDictionary<int, ICollection<Elem.Attr.Rem>> _reminders = new SortedDictionary<int, ICollection<Elem.Attr.Rem>>();
			private int _openReferences;

			public void Assign(int serial, object element)
			{
				Debug.Assert(element != null);
				Debug.Assert(!_serials.ContainsKey(serial));

				_serials[serial] = element;
				ResolveReminders(serial, element);
			}

			public bool HasDanglingReferences()
			{
				return _openReferences > 0;
			}

			public Elem.Attr.Rem KeepReminder(Elem.Attr.Rem reminder, int serial)
			{
				Debug.Assert(!_serials.ContainsKey(serial));

				var todo = _reminders[serial];
				if (todo == null)
				{
					_reminders[serial] = todo = new LinkedList<Elem.Attr.Rem>();
				}

				todo.Add(reminder);
				_openReferences++;

				return reminder;
			}

			private void ResolveReminders(int serial, object element)
			{
				if (!_reminders.ContainsKey(serial))
					return;

				var todo = _reminders[serial];
				_reminders.Remove(serial);

				foreach (Elem.Attr.Rem each in todo)
				{
					each.Resolve(element);
					_openReferences--;
					Debug.Assert(_openReferences >= 0);
				}
			}

			public object Retrieve(int serial)
			{
				return _serials[serial];
			}
		}

		private Stack<Elem> _elementStack;
		private ICollection<object> _elements;
		private Index _index;
		private readonly MetaRepository _metamodel;
		private readonly Repository _model;

		public Importer(MetaRepository metamodel) : this(metamodel, new Repository(metamodel))
		{
		}

		public Importer(MetaRepository metamodel, Repository model)
		{
			_metamodel = metamodel;
			_model = model;
		}

		public override void BeginAttribute(string name)
		{
			_elementStack.Peek().BeginAttribute(name);
		}

		public override void BeginDocument()
		{
			_index = new Index();
			_elements = new List<object>();
			_elementStack = new Stack<Elem>();
		}

		public override void BeginElement(string name)
		{
			_elementStack.Push(new Elem(name, _metamodel, _index));
		}

		public override void EndAttribute(string name)
		{
			_elementStack.Peek().EndAttribute(name);
		}

		public override void EndDocument()
		{
			Debug.Assert(_elementStack.Count == 0);

			_elementStack = null;

			Debug.Assert(!_index.HasDanglingReferences());

			_index = null;
			foreach (object element in _elements)
			{
				_model.Add(element);
			}

			_elements = null;
		}

		public override void EndElement(string name)
		{
			var frame = _elementStack.Pop();
			var element = frame.Element;
			_elements.Add(element);

			if (_elementStack.Count > 0)
			{
				_elementStack.Peek().Add(element);
			}
		}

		public Repository Result => _model;

		public override void Primitive(object value)
		{
			_elementStack.Peek().Add(value);
		}

		public void ReadFrom(InputSource @in)
		{
			var parser = new Parser(new Scanner(@in));
			parser.Accept(this);
		}

		public override void Reference(int serial)
		{
			_elementStack.Peek().Reference(serial);
		}

		public override void Reference(string name)
		{
			// TODO find nice solution for this hack

			var type = MetaDescription.PrimitiveNamed(name);
			Debug.Assert(type != null, name);
			_elementStack.Peek().Add(type);
		}

		public override void Serial(int serial)
		{
			_elementStack.Peek().Serial(serial);
		}
	}
}