namespace Fame.Parser
{
	// TODO
	public interface IParseClient
	{
		void BeginAttribute(string name);

		void BeginDocument();

		void BeginElement(string name);

		void Directive(string name, params string[] parameters);

		void EndAttribute(string name);

		void EndDocument();

		void EndElement(string name);

		void Primitive(object value);

		void Reference(int index);

		void Reference(string name);

		void Reference(string name, int index);

		void Serial(int index);
	}
}