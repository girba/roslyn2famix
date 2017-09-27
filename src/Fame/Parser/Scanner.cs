namespace Fame.Parser
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// Breaks input-stream into tokens. Accepts the following grammar
	/// 
	/// Tokens
	/// 
	/// <pre>
	/// OPEN       ::= &quot;(&quot;
	/// CLOSE      ::= &quot;)&quot;
	/// ID         ::= &quot;id:&quot;
	/// REF        ::= &quot;ref:&quot;
	/// KEYWORD    ::= &quot;@&quot; NamePart
	/// NAME       ::= NamePart ( &quot;.&quot; NamePart )*
	/// STRING     ::= ( &quot;'&quot; [&circ;']* &quot;'&quot; )+
	/// BOOLEAN    ::= &quot;true&quot; | &quot;false&quot;
	/// UNDEFINED  ::= &quot;nil&quot;
	/// NUMBER     ::= &quot;-&quot;? Digit+ ( &quot;.&quot; Digit+ )? ( &quot;e&quot;  ( &quot;-&quot;? Digit+ ))?
	/// </pre>
	/// 
	/// Terminals and Intermediates
	/// 
	/// <pre>
	/// NamePart   ::= Letter ( Letter | Digit )*
	/// Digit      ::= [0-9]
	/// Letter     ::= [a-zA-Z_]
	/// </pre>
	/// 
	/// Whitespace
	/// 
	/// <pre>
	/// Whitespace ::= \s+
	/// Comment    ::= &quot;\&quot;&quot; [&circ;&quot;]* &quot;\&quot;&quot;
	/// </pre>
	/// 
	/// In order to provide more user friendly error messages, this implementation
	/// imposed the following constraint upon the sequence of tokens: <tt>Name</tt>
	/// must be followed by whitespace; <tt>String</tt>, <tt>Undefined</tt>,
	/// <tt>Boolean</tt>, and <tt>Number</tt> must be followed by <tt>Open</tt>,
	/// <tt>Close</tt> or whitespace.
	/// 
	/// If the input source is in Smalltalk compatibility mode, the grammar is
	/// extended as follows
	/// 
	/// <pre>
	/// Unlimited ::= &quot;*&quot; 
	/// Keyword ::= ... | Letter+ &quot;:&quot;
	/// </pre>
	/// 
	/// </summary>
	public class Scanner : IEnumerator<Token>, IEnumerable<Token>
	{
		private static readonly Token Close = new Token(TokenType.Close, ")");
		private static readonly Token Eof = new Token(TokenType.Eof, null);
		private static readonly Token False = new Token(TokenType.Boolean, false);
		private static readonly Token Open = new Token(TokenType.Open, "(");
		private static readonly Token True = new Token(TokenType.Boolean, true);
		private static readonly Token Ref = new Token(TokenType.Ref, "ref:");
		private static readonly Token Id = new Token(TokenType.Id, "id:");
		private static readonly Token Undefined = new Token(TokenType.Undefined, "nil");

		protected internal readonly InputSource In; // access from parser to query

		public Scanner(char[] @string) : this(InputSource.FromString(@string))
		{
		}

		public Scanner(InputSource @in)
		{
			In = @in;
		}

		private Token ClosingParenthesis()
		{
			In.Inc();

			return Close;
		}

		private void ExpectDelimiterToken()
		{
			if (!In.HasNext())
			{
				return; // EOF is okay!
			}

			char ch = In.Peek();
			if (ch == '\"' || char.IsWhiteSpace(ch) || ch == '(' || ch == ')')
			{
				return;
			}

			throw new ParseError("Whitespace or delimiter expected", Pos());
		}

		private void ExpectDigit()
		{
			if (!char.IsDigit(In.Peek()))
			{
				throw new ParseError("Digit expected", Pos());
			}
		}

		private void ExpectWhitespaceToken()
		{
			if (!In.HasNext())
			{
				return; // EOF is okay!
			}

			char ch = In.Peek();
			if (ch == '\"' || char.IsWhiteSpace(ch))
			{
				return;
			}

			throw new ParseError("Whitespace expected", Pos());

		}

		public bool HasNext()
		{
			SkipWhitespace();

			return In.HasNext();
		}

		private Token Keyword()
		{
			In.Mark();
			if ('@' != In.Peek())
			{
				throw new ParseError("At sign expected", Pos());
			}

			In.Inc();
			LetterExpected();

			In.Inc();
			while (char.IsLetterOrDigit(In.Peek()))
			{
				In.Inc();
			}

			ExpectWhitespaceToken();

			return new Token(TokenType.Keyword, new string(In.Yank()));
		}

		private void LetterExpected()
		{
			if (!char.IsLetter(In.Peek()))
			{
				throw new ParseError("Letter expected", Pos());
			}
		}

		private Token NameOrSomethingLikeThat()
		{
			In.Mark();

			while (true)
			{
				LetterExpected();
				while (char.IsLetterOrDigit(In.Peek()))
				{
					In.Inc();
				}

				char ch = In.Peek();
				if (ch == ':')
				{
					return Reference();
				}

				if (ch != '.')
				{
					break;
				}

				In.Inc();
			}

			ExpectDelimiterToken();
			string name = new string(In.Yank());
			if (name.Equals("nil"))
			{
				return Undefined;
			}

			if (name.Equals("true"))
			{
				return True;
			}

			if (name.Equals("false"))
			{
				return False;
			}

			return new Token(TokenType.Name, name);
		}

		public Token Next()
		{
			SkipWhitespace();

			char ch = In.Peek();
			if (ch == '(')
			{
				return OpeningParenthesis();
			}

			if (ch == ')')
			{
				return ClosingParenthesis();
			}

			if (ch == '\'')
			{
				return String();
			}

			if (ch == '-' || char.IsDigit(ch))
			{
				return Number();
			}

			if (char.IsLetter(ch))
			{
				return NameOrSomethingLikeThat();
			}

			if (ch == '@')
			{
				return Keyword();
			}

			throw new ParseError("Illegal character '" + ch + "'", Pos());
		}

		public Token NextOrEof()
		{
			Token n = HasNext() ? Next() : Eof;

			return n;
		}

		/// <summary>
		/// matches the rule
		/// 
		/// <pre>
		/// 
		/// &lt;number&gt; ::= -? &lt;digit&gt;+ ( . &lt;digit&gt;+ )? ( e ( -? &lt;digit&gt;+ ))?
		/// 
		/// </pre>
		/// 
		/// </summary>
		/// <returns> a NUMBER token </returns>
		private Token Number()
		{
			In.Mark();

			bool isDouble = false;

			// match -? <digit>+
			if (In.Peek() == '-')
			{
				In.Inc();
			}

			ExpectDigit();
			while (char.IsDigit(In.Peek()))
			{
				In.Inc();
			}

			// match ( . <digit>+ )?
			if (In.Peek() == '.')
			{
				isDouble = true;
				In.Inc();
				ExpectDigit();

				while (char.IsDigit(In.Peek()))
				{
					In.Inc();
				}
			}

			// match ( e -? <digit>+ )?
			if (In.Peek() == 'e' || In.Peek() == 'E')
			{
				isDouble = true;
				In.Inc();

				if (In.Peek() == '-')
				{
					In.Inc();
				}

				ExpectDigit();
				while (char.IsDigit(In.Peek()))
				{
					In.Inc();
				}
			}

			ExpectDelimiterToken();
			string str = new string(In.Yank());

			return isDouble ? new Token(double.Parse(str)) : new Token(int.Parse(str));
		}

		private Token OpeningParenthesis()
		{
			In.Inc();

			return Open;
		}

		public Position Pos()
		{
			return In.Position;
		}

		private Token Reference()
		{
			// called from nextNameOrKeywordOrBoolean

			In.Inc(); // consume ':'

			string name = new string(In.Yank());
			ExpectWhitespaceToken();

			if (name.Equals("id:"))
			{
				return Id;
			}

			if (name.Equals("ref:"))
			{
				return Ref;
			}

			throw new ParseError("Illegal character ':'", Pos());
		}

		public void Remove()
		{
			throw new NotSupportedException();
		}

		private void SkipComment()
		{
			Position start = Pos();
		
			if (In.Peek() != '\"')
			{
				return;
			}

			for (In.Inc2(); ; In.Inc2())
			{
				char ch = In.Peek();
				if (ch == InputSource.Eof)
				{
					throw new ParseError("Runaway comment ", start);
				}

				if (ch == '\"')
				{
					break;
				}
			}

			In.Inc2();
		}

		private void SkipWhitespace()
		{
			for (; ; In.Inc2())
			{
				char ch = In.Peek();
				if (ch == InputSource.Eof)
				{
					break;
				}

				if (ch == '\"')
				{
					SkipComment();
					continue;
				}

				if (!char.IsWhiteSpace(ch))
				{
					break;
				}
			}
		}

		private Token String()
		{
			Position start = Pos();
			StringBuilder buffer = new StringBuilder();

			for (In.Inc(); ;)
			{
				char ch = In.Peek();
				if (ch == InputSource.Eof)
				{
					throw new ParseError("Runaway string", start);
				}

				In.Inc();
				if (ch == '\'')
				{
					if (In.Peek() != '\'')
					{
						break;
					}

					In.Inc();
				}

				buffer.Append(ch);
			}

			ExpectDelimiterToken();

			return new Token(TokenType.String, buffer.ToString());
		}

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
			throw new NotImplementedException();
		}

		public Token Current { get; private set; }

		object IEnumerator.Current => Current;

		public IEnumerator<Token> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}