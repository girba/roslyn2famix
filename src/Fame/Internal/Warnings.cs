namespace Fame.Internal
{
	using System.Collections.Generic;
	using System.Text;
	using Fm3;

	public class Warnings
	{
		private class Warn
		{
			private readonly string _message;
			private readonly Element _element;

			public Warn(Element element, string message)
			{
				_message = message;
				_element = element;
			}

			public override string ToString()
			{
				return _message + ": " + _element;
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