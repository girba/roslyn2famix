namespace Fame.Dsl
{
	using System;
	using System.Text;
	using Internal;
	using Parser;

	class ModelBuilder
	{
		internal interface IEndable
		{
			void EndDocument();
		}

		internal interface IDocument : IEndable
		{
			IElementWithMaybeSerial<IDocument> Element(string @string);
		}

		internal interface IElement<T> : IEndable where T : IEndable //where T2 : IElement<T1, T2>
		{
			IAttribute<IElement<T>> With(string attribute);
			IAttribute<IElement<T>> With(string attribute, params object[] values);
			T EndElement();
		}

		internal interface IElementWithMaybeSerial<T> : IElement<T> where T : IEndable
		{
			IElement<T> Index(int index);
		}

		internal interface IAttribute<T> : IEndable  where T : IEndable
		{
			IAttribute<IElement<T>> Value(object value);

			IAttribute<IElement<T>> Reference(int index);

			IAttribute<IElement<T>> Reference(string name);

			IElementWithMaybeSerial<IAttribute<IElement<T>>> Element(string name);

			IElement<T> EndAttribute();

			IAttribute<IElement<T>> With(string attribute);

			IAttribute<IElement<T>> With(string attribute, params object[] b);
		}

		private readonly IParseClient _client;

		public ModelBuilder(IParseClient client)
		{
			_client = client;
		}

		public virtual IDocument BeginDocument()
		{
			return new DocumentImpl(_client);
		}

		internal class DocumentImpl : IDocument
		{
			private readonly IParseClient _client;

			public DocumentImpl(IParseClient client)
			{
				_client = client;
				_client.BeginDocument();
			}

			public virtual IElementWithMaybeSerial<IDocument> Element(string name)
			{
				return new ElementImpl<IDocument>(name, this, _client);
			}

			public virtual void EndDocument()
			{
				_client.EndDocument();
			}
		}

		internal class ElementImpl<T> : IElementWithMaybeSerial<T> where T : IEndable
		{
			private readonly IParseClient _client;

			internal readonly string Name;
			internal readonly T Outer;

			public ElementImpl(string name, T outer, IParseClient client)
			{
				_client = client;
				_client.BeginElement(name);
				Name = name;
				Outer = outer;
			}

			public IElement<T> Index(int index)
			{
				_client.Serial(index);

				return this;
			}

			public IAttribute<IElement<T>> With(string attribute)
			{
				return new AttributeImpl<IElement<T>>(attribute, this, _client);
			}

			public T EndElement()
			{
				_client.EndElement(Name);

				return Outer;
			}

			public void EndDocument()
			{
				EndElement().EndDocument();
			}

			public virtual IAttribute<IElement<T>> With(string attribute, params object[] values)
			{
				var a = With(attribute);

				foreach (var each in values)
					a.Value(each);

				return a;
			}
		}

		internal class AttributeImpl<T> : IAttribute<T> where T : IEndable
		{
			private readonly IParseClient _client;

			internal readonly string Name;
			internal readonly T Outer;

			public AttributeImpl(string name, T outer, IParseClient client)
			{
				_client = client;
				_client.BeginAttribute(name);
				Name = name;
				Outer = outer;
			}

			public IElementWithMaybeSerial<IAttribute<IElement<T>>> Element(string name)
			{
				return new ElementImpl<IAttribute<IElement<T>>>(name, (IAttribute<IElement<T>>)this, _client);
			}

			public IElement<T> EndAttribute()
			{
				_client.EndAttribute(Name);

				return (IElement<T>)Outer;
			}

			public IAttribute<IElement<T>> Reference(int index)
			{
				_client.Reference(index);

				return (IAttribute<IElement<T>>)this;
			}

			public IAttribute<IElement<T>> Reference(string name)
			{
				_client.Reference(name);

				return (IAttribute<IElement<T>>)this;
			}

			public IAttribute<IElement<T>> Value(object value)
			{
				_client.Primitive(value);

				return (IAttribute<IElement<T>>)this;
			}

			public IAttribute<IElement<T>> With(string attribute)
			{
				return EndAttribute().With(attribute);
			}

			public void EndDocument()
			{
				EndAttribute().EndDocument();
			}

			public IAttribute<IElement<T>> With(string attribute, params object[] values)
			{
				var a = With(attribute);

				foreach (object each in values)
					a.Value(each);

				return a;
			}
		}

		public static void Main(string[] args)
		{
			// TODO: Verify correctness
			IParseClient pc = new DebugClient();
			var sb = new StringBuilder();
			pc = new MSEPrinter(sb);

			var attr0 = new ModelBuilder(pc)
				.BeginDocument()
					.Element("FM3.Package").Index(1)
						.With("name", "HAPAX")
						.With("classes");
							var attr1 = attr0.Element("FM3.Class").Index(2)
								.With("name", "Document")
								.With("attributes");
								attr1.Element("FM3.Property")
										.With("name", "content")
										.With("type").Reference(3)
										.With("multivalued", true)
										.EndAttribute()
									.EndElement();
								attr1.EndAttribute()
							.EndElement();
							var attr2 = attr0.Element("FM3.Class").Index(3)
								.With("name", "Occurrence")
								.With("attributes");
									attr2.Element("FM3.Property")
										.With("name", "term")
										.With("type").Reference("String")
										.EndAttribute()
									.EndElement();
									attr2.Element("FM3.Property")
										.With("name", "frequency")
										.With("type").Reference("Number")
				.EndDocument();

			//new ModelBuilder(pc)
			//	.BeginDocument()
			//		.Element("FM3.Package").Index(1)
			//			.With("name", "HAPAX")
			//			.With("classes")
			//				.Element("FM3.Class").Index(2)
			//					.With("name", "Document")
			//					.With("attributes")
			//						.Element("FM3.Property")
			//							.With("name", "content")
			//							.With("type").Reference(3)
			//							.With("multivalued", true)
			//							.EndAttribute()
			//						.EndElement()
			//					.EndAttribute()
			//				.EndElement()
			//				.Element("FM3.Class").Index(3)
			//					.With("name", "Occurrence")
			//					.With("attributes")
			//						.Element("FM3.Property")
			//							.With("name", "term")
			//							.With("type").Reference("String")
			//							.EndAttribute()
			//						.EndElement()
			//						.Element("FM3.Property")
			//							.With("name", "frequency")
			//							.With("type").Reference("Number")
			//	.EndDocument();

			Console.WriteLine(sb.ToString());
		}
	}
}