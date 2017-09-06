namespace Fame.Parser
{
	using System;

	public class ParseError : Exception
	{
		public readonly Position Pos;
		public TokenType Expected;
		public Token Found;

		public ParseError(string message, Position pos) : base(message + " at " + pos)
		{
			Pos = pos;
		}

		public ParseError(TokenType expected, Token found, Position pos) : base("Expected " + expected + ", found " + found.Type + " at " + pos)
		{
			Expected = expected;
			Found = found;
			Pos = pos;
		}
	}
}