using System;
using Fame.Parser;

namespace Fame.Dsl
{
	public class ProtocolChecker : IParseClient
	{
		// TODO

		public ProtocolChecker(IParseClient client)
		{
			throw new NotImplementedException();
		}

		public void BeginAttribute(string name)
		{
			throw new NotImplementedException();
		}

		public void BeginDocument()
		{
			throw new NotImplementedException();
		}

		public void BeginElement(string name)
		{
			throw new NotImplementedException();
		}

		public void Directive(string name, params string[] parameters)
		{
			throw new NotImplementedException();
		}

		public void EndAttribute(string name)
		{
			throw new NotImplementedException();
		}

		public void EndDocument()
		{
			throw new NotImplementedException();
		}

		public void EndElement(string name)
		{
			throw new NotImplementedException();
		}

		public void Primitive(object value)
		{
			throw new NotImplementedException();
		}

		public void Reference(int index)
		{
			throw new NotImplementedException();
		}

		public void Reference(string name)
		{
			throw new NotImplementedException();
		}

		public void Reference(string name, int index)
		{
			throw new NotImplementedException();
		}

		public void Serial(int index)
		{
			throw new NotImplementedException();
		}
	}
}