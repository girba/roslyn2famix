using System;

namespace Fame.Parser
{
	/// <summary>
	/// Empty implementation of {@link ParseClient}.
	/// 
	/// @author akuhn
	/// </summary>
	public class AbstractParserClient : IParseClient
	{
		public void BeginAttribute(string name)
		{
		}

		public void BeginDocument()
		{
			// TODO Auto-generated method stub

		}

		public void BeginElement(string name)
		{
		}

		public void Directive(string name, params string[] parameters)
		{
			// TODO Auto-generated method stub

		}

		public void EndAttribute(string name)
		{
		}

		public void EndDocument()
		{
			// TODO Auto-generated method stub

		}

		public void EndElement(string name)
		{
		}

		public void Primitive(object value)
		{
		}

		public void Reference(int index)
		{
		}

		public void Reference(string name)
		{
		}

		public void Reference(string name, int index)
		{
			// TODO Auto-generated method stub

		}

		public void Serial(int index)
		{
		}
	}
}