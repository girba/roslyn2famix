namespace Fame.Parser
{
	using System.Diagnostics;

	public class Token
	{
		public readonly TokenType Type;
		public readonly object Value;

		public Token(bool @bool)
		{
			Type = TokenType.Boolean;
			Value = @bool;
		}

		public Token(double number)
		{
			Type = TokenType.Number;
			Value = number;
		}

		public Token(int number)
		{
			Type = TokenType.Number;
			Value = number;
		}

		public Token(TokenType type, bool @bool)
		{
			Type = type;
			Value = @bool;
		}

		public Token(TokenType type, string @string)
		{
			// TODO which is the better strategy?
			// http://www.codeinstructions.com/2008/09/instance-pools-with-weakhashmap.html
			// http://kohlerm.blogspot.com/2009/01/is-javalangstringintern-really-evil.html
			// http://www.codeinstructions.com/2009/01/busting-javalangstringintern-myths.html

			Type = type;
			Value = ReferenceEquals(@string, null) ? null : string.Intern(@string);
		}

		public bool BooleanValue()
		{
			Debug.Assert(Type == TokenType.Boolean);

			return (bool)Value;
		}

		public double DoubleValue()
		{
			Debug.Assert(Type == TokenType.Number);

			return (double)Value;
		}

		public int IntValue()
		{
			Debug.Assert(Type == TokenType.Number);

			return (int)Value;
		}

		public  string StringValue()
		{
			return (string)Value;
		}

		public override string ToString()
		{
			return Type + " " + Value;
		}
	}
}
