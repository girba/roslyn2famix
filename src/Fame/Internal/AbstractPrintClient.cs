namespace Fame.Internal
{
	using System.IO;
	using Parser;

	public abstract class AbstractPrintClient : AbstractParserClient
	{
		protected internal int Indentation;
		protected internal TextWriter Stream;
		private bool _wasln;

		public AbstractPrintClient(TextWriter stream)
		{
			Stream = stream;
			Indentation = 0;
			_wasln = true;
		}

		protected internal virtual void Append(char ch)
		{
			try
			{
				Stream.Write(ch);
				_wasln = false;
			}
			catch (IOException)
			{
				throw;
			}
		}

		protected internal virtual void Append(char[] characters)
		{
			try
			{
				Stream.Write(characters);
				_wasln = false;
			}
			catch (IOException)
			{
				throw;
			}
		}

		protected internal virtual void Close()
		{
			Stream.Close();
		}

		protected internal virtual void Lntabs()
		{
			if (!_wasln)
			{
				try
				{
					Stream.Write('\n');

					for (int n = 0; n < Indentation; n++)
					{
						Stream.Write('\t');
					}
				}
				catch (IOException)
				{
					throw;
				}
			}

			_wasln = true;
		}
	}
}