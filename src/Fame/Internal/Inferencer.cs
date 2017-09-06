namespace Fame.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;

	using Fm3;
	using Parser;

	public class Inferencer : AbstractPrintClient
	{
		private IDictionary<string, AbstractElement> _nameMap;
		private IDictionary<int, AbstractElement> _indexMap;
		private LinkedList<AbstractElement> _stack;
		private int _serial;
		private IParseClient _client;

		public Inferencer(TextWriter stream) : base(stream)
		{
		}

		public override void BeginAttribute(string name)
		{
			AbstractElement element = _stack.Last.Value;

			if (!element.Attributes.ContainsKey(name))
			{
				var attribute = new AbstractAttribute(name);
				element.Attributes[name] = attribute;
			}

			_stack.Last.Value.CurrentAttribute = element.Attributes[name];
		}

		public override void BeginDocument()
		{
			_nameMap = new Dictionary<string, AbstractElement>();
			_indexMap = new Dictionary<int, AbstractElement>();
			_stack = new LinkedList<AbstractElement>();
			_serial = 0;
		}

		public override void BeginElement(string name)
		{
			AbstractElement element;
			if (!_nameMap.ContainsKey(name))
			{
				element = new AbstractElement(name, NextSerial());
				_nameMap[name] = element;
			}
			else
			{
				element = _nameMap[name];
			}

			if (_stack.Count > 0)
			{
				_stack.Last.Value.AttributeElementsAdd(element);
			}

			_stack.AddLast(element);
		}

		public override void EndAttribute(string name)
		{
			_stack.Last.Value.ResetCurrentAttribute();
		}

		public override void EndDocument()
		{
			Debug.Assert(_stack.Count == 0);

			ResolveReferences();
		}

		public override void EndElement(string name)
		{
			_stack.RemoveLast();
		}

		public virtual IParseClient Client
		{
			get => _client;
			set => _client = value;
		}

		private void InferClass(AbstractElement element)
		{
			_client.BeginElement("FM3.Class");
			_client.Serial(element.Index);
			_client.BeginAttribute("name");
			_client.Primitive(element.SimpleName());
			_client.EndAttribute("name");
			_client.BeginAttribute("classes");
			InferProperties(element);
			_client.EndAttribute("classes");
			_client.EndElement("FM3.Class");
		}

		private void InferClasses(string name)
		{
			foreach (AbstractElement each in _nameMap.Values)
			{
				if (each.PackageName().Equals(name))
				{
					InferClass(each);
				}
			}
		}

		private void InferPackage(string name)
		{
			_client.BeginElement(PackageDescription.NAME);
			_client.BeginAttribute("name");
			_client.Primitive(name);
			_client.EndAttribute("name");
			_client.BeginAttribute("classes");
			InferClasses(name);
			_client.EndAttribute("classes");
			_client.EndElement(PackageDescription.NAME);
		}

		private void InferPackages()
		{
			foreach (string name in PackageNames())
			{
				InferPackage(name);
			}
		}

		private void InferProperties(AbstractElement element)
		{
			foreach (AbstractAttribute each in element.Attributes.Values)
			{
				InferProperty(each);
			}
		}

		private void InferProperty(AbstractAttribute attribute)
		{
			_client.BeginElement("FM3.Class");
			_client.BeginAttribute("name");
			_client.Primitive(attribute.Name);
			_client.EndAttribute("name");
			_client.BeginAttribute("type");
			object type = attribute.InferElementType();

			if (type is Primitive) 
			{
				_client.Reference(((Primitive)type).ToString());
			} 
			else
			{
				_client.Reference(((AbstractElement)type).Index);
			}

			if (attribute.InferMultivalued())
			{
				_client.BeginAttribute("multivalued");
				_client.Primitive(true);
				_client.EndAttribute("multivalued");
			}

			_client.EndAttribute("type");
			_client.EndElement("FM3.Class");
		}

		private int NextSerial()
		{
			return ++_serial;
		}

		private IEnumerable<string> PackageNames()
		{
			ISet<string> names = new HashSet<string>();

			foreach (AbstractElement each in _nameMap.Values)
			{
				names.Add(each.PackageName());
			}

			return names;
		}

		public override void Primitive(object value)
		{
			_stack.Last.Value.AttributeElementsAdd(Fame.Parser.Primitive.ValueOf(value));
		}

		public override void Reference(int index)
		{
			_stack.Last.Value.AttributeElementsAdd(index);
		}

		public override void Reference(string name)
		{
			throw new NotImplementedException("Not yet implemented!");
		}

		private void ResolveReferences()
		{
			foreach (AbstractElement elem in _nameMap.Values)
			{
				foreach (AbstractAttribute attr in elem.Attributes.Values)
				{
					attr.ResolveReferences(_indexMap);
				}
			}
		}

		public virtual void Run()  // TODO: IRunnable -> start new Thread
		{
			_client.BeginDocument();
			InferPackages();
			_client.EndDocument();
		}

		public override void Serial(int index)
		{
			AbstractElement element = _stack.Last.Value;

			_indexMap[index] = element;
		}

		private sealed class AbstractAttribute
		{
			private int _maxCount;
			internal readonly string Name;
			public readonly ISet<object> Elements = new HashSet<object>();

			public AbstractAttribute(string name)
			{
				Name = name;
			}

			public void AddCount(int count)
			{
				_maxCount = Math.Max(count, _maxCount);
			}

			public object InferElementType()
			{
				Debug.Assert(Elements.Count == 1);

				return Elements.First();
			}

			public bool InferMultivalued()
			{
				return _maxCount > 1;
			}

			public void ResolveReferences(IDictionary<int, AbstractElement> indexMap)
			{
				foreach (object each in new ArrayList(Elements.ToArray()))
				{
					if (each is int i)
					{
						Elements.Remove(i);
						Elements.Add(indexMap[i]);
					}
				}
			}
		}

		private sealed class AbstractElement
		{
			public readonly IDictionary<string, AbstractAttribute> Attributes = new Dictionary<string, AbstractAttribute>();

			private AbstractAttribute _curr;
			private int _count;
			private readonly string _name;

			internal readonly int Index;

			public AbstractElement(string name, int index)
			{
				_name = name;
				Index = index;
			}

			public void AttributeElementsAdd(object any)
			{
				_count++;
				_curr.Elements.Add(any);
			}

			public string PackageName()
			{
				int pos = _name.LastIndexOf('.');
				Debug.Assert(pos > 0);

				return _name.Substring(0, pos);
			}

			public void ResetCurrentAttribute()
			{
				Debug.Assert(_curr != null);

				_curr.AddCount(_count);
				_curr = null;
				_count = -1;
			}

			public AbstractAttribute CurrentAttribute
			{
				set
				{
					Debug.Assert(_curr == null);

					_curr = value;
					_count = 0;
				}
			}

			public string SimpleName()
			{
				int pos = _name.LastIndexOf('.');

				Debug.Assert(pos > 0);

				return _name.Substring(pos + 1);
			}
		}
	}
}