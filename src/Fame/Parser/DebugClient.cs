namespace Fame.Parser
{
	using System.Collections.Generic;
	using System.Text;

	public class DebugClient : IParseClient
	{
		public readonly IList<object[]> Log = new List<object[]>();
		public IParseClient Client;

		public DebugClient(IParseClient client)
		{
			Client = client;
		}

		public DebugClient() : this(null)
		{
		}

		public void BeginAttribute(string name)
		{
			Log.Add(new object[] { "beginAttribute", name });

			Client?.BeginAttribute(name);
		}

		public void BeginDocument()
		{
			Log.Add(new object[] { "beginDocument" });

			Client?.BeginDocument();
		}

		public void BeginElement(string name)
		{
			Log.Add(new object[] { "beginElement", name });

			Client?.BeginElement(name);
		}

		public void Directive(string name, params string[] @params)
		{
			throw new System.NotSupportedException();
		}

		public void EndAttribute(string name)
		{
			Log.Add(new object[] { "endAttribute", name });

			Client?.EndAttribute(name);
		}

		public void EndDocument()
		{
			Log.Add(new object[] { "endDocument" });

			Client?.EndDocument();
		}

		public void EndElement(string name)
		{
			Log.Add(new object[] { "endElement", name });

			Client?.EndElement(name);
		}

		public virtual void Primitive(object value)
		{
			Log.Add(new[] { "primitive", value });

			Client?.Primitive(value);
		}

		public void Reference(int index)
		{
			Log.Add(new object[] { "reference(int)", index });

			Client?.Reference(index);
		}

		public void Reference(string name)
		{
			Log.Add(new object[] { "reference(String)", name });

			Client?.Reference(name);
		}

		public void Reference(string name, int index)
		{
			throw new System.NotSupportedException();
		}

		public void Serial(int index)
		{
			Log.Add(new object[] { "serial", index });

			Client?.Serial(index);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			foreach (object[] line in Log)
			{
				var s = ", ";

				foreach (object each in line)
				{
					sb.Append(s).Append(each);
				}

				sb.Append('\n');
			}

			return sb.ToString();
		}
	}
}