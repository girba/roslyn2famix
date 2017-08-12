namespace Fame.Parser
{
	/// <summary>
	/// Interface for reading MSE documents using callbacks. This interface allows an
	/// application to register for MSE document parsing.The sequence of callbacks
	/// is limited to the following protocol
	/// 
	/// <ul>
	/// <li><code>MAIN := directive* beginDocument ELEM* endDocument</code>
	/// <li><code>ELEM := beginElement serial? (beginAttribute (primitive | reference | ELEM )* endAttribute) endElement</code>
	/// </ul>
	/// 
	/// @see Parser
	/// </summary>
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