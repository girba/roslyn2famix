namespace Fame.Internal
{
	using System.Collections.Generic;
	using System.Text;
	using Fm3;

	public class Warnings
	{
		private class Warn
		{
			public readonly string Message;
			public readonly Element Element;

			public Warn(Element element, string message)
			{
				Message = message;
				Element = element;
			}

			public override string ToString()
			{
				return Message + ": " + Element;
			}
		}

		private readonly ICollection<Warn> _warnings = new List<Warn>();

		public void Add(string message, Element element)
		{
			_warnings.Add(new Warn(element, message));
		}

		public void PrintOn(StringBuilder stream)
		{
			foreach (Warn each in _warnings)
			{
				stream.Append(each);
				stream.Append('\n');
			}
		}
	}
}