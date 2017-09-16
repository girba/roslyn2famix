namespace Fame.Internal
{
	using System.IO;
	using Parser;

	public abstract class AbstractPrintClient : AbstractParserClient
	{
		protected internal int Indentation;
		protected internal TextWriter Stream;
		private bool _wasln;

		protected AbstractPrintClient(TextWriter stream)
		{
			Stream = stream;
			Indentation = 0;
			_wasln = true;
		}

		protected internal virtual void Append(char ch)
		{
			Stream.Write(ch);
			_wasln = false;
		}

		protected internal virtual void Append(char[] characters)
		{
			Stream.Write(characters);
			_wasln = false;
		}

		protected internal virtual void Close()
		{
			Stream.Close();
		}

		protected internal virtual void Lntabs()
		{
			if (!_wasln)
			{
				Stream.Write('\n');

				for (int n = 0; n < Indentation; n++)
				{
					Stream.Write('\t');
				}
			}

			_wasln = true;
		}
	}
}