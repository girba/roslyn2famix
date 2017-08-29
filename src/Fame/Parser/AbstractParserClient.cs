namespace Fame.Parser
{
	/// <summary>
	/// Empty implementation of {@link ParseClient}.
	/// 
	/// @author akuhn
	/// </summary>
	public class AbstractParserClient : IParseClient
	{
		// TODO

		public virtual void BeginAttribute(string name)
		{
		}

		public virtual void BeginDocument()
		{
			// TODO Auto-generated method stub
		}

		public virtual void BeginElement(string name)
		{
		}

		public virtual void Directive(string name, params string[] parameters)
		{
			// TODO Auto-generated method stub
		}

		public virtual void EndAttribute(string name)
		{
		}

		public virtual void EndDocument()
		{
			// TODO Auto-generated method stub
		}

		public virtual void EndElement(string name)
		{
		}

		public virtual void Primitive(object value)
		{
		}

		public virtual void Reference(int index)
		{
		}

		public virtual void Reference(string name)
		{
		}

		public virtual void Reference(string name, int index)
		{
			// TODO Auto-generated method stub
		}

		public virtual void Serial(int index)
		{
		}
	}
}