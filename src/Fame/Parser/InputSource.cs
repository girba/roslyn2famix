namespace Fame.Parser
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Text;

	public class InputSource : IEnumerator<char>, IEnumerable<char>
	{
		public static readonly char Eof = unchecked((char)-1);  //TODO: Required in C#

		public static InputSource FromFilename(string filename)
		{
			return FromString(File.ReadAllText(filename, Encoding.GetEncoding("ISO-8859-1")).ToCharArray());
		}

		public static InputSource FromInputStream(Stream stream)
		{
			using (stream)
			{
				using (var reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))
				{
					var buffer = reader.ReadToEnd();

					return FromString(buffer.ToCharArray());
				}
			}
		}

		public static InputSource FromResource(string name)
		{
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(name);

			return FromInputStream(stream);
		}

		public static InputSource FromString(char[] @string)
		{
			return new InputSource(@string);
		}

		private int _index;
		private int _start;
		private readonly int _length;
		private readonly char[] _string;
		private int _line;
		private int _prevLineBreak;

		private InputSource(char[] @string)
		{
			_index = 0;
			_start = -1;
			_line = 1;
			_prevLineBreak = -1;
			_length = @string.Length;
			_string = @string;
		}

		public Position Position => new Position(_line, _index - _prevLineBreak, _index);

		public bool HasNext()
		{
			return Peek() != Eof;
		}

		public void Inc()
		{
			_index++;
		}

		public void Inc2()  // TODO nicer name
		{ 
			if (_string[_index] == '\n')
			{
				_prevLineBreak = _index;
				_line++;
			}

			_index++;
		}

		public void Mark()
		{
			_start = _index;
		}

		public char Next()
		{
			Inc();

			return Peek();
		}

		public char Peek()
		{
			return _index < _length ? _string[_index] : Eof;
		}

		public void Remove()
		{
			throw new NotSupportedException();
		}

		public void Rewind()
		{
			_index = 0;
			_start = -1;
		}

		public char[] Yank()
		{
			var str = new string(_string);

			return str.Substring(_start, _index - _start).ToCharArray();
		}

		// TODO: Check correctness
		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			var hasNext = HasNext();

			if (hasNext)
				Current = Next();

			return hasNext;
		}

		public void Reset()
		{
			Rewind();
		}

		public char Current { get; private set; }

		object IEnumerator.Current => Current;

		public IEnumerator<char> GetEnumerator()
		{
			Reset();

			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}