namespace Fame.Parser
{
	using System.Collections.Generic;
	using System.Linq;

	public class Parser
	{
		private IParseClient _client;
		private Position _pos, _pos2;
		private Token _peek, _peek2;
		private readonly Scanner _stream;

		public Parser(Scanner stream)
		{
			_stream = stream;
			Consume();
			Consume();
		}

		public void Accept(IParseClient newClient)
		{
			_client = newClient;

			if (_peek.Type == TokenType.Eof)
			{
				_client.BeginDocument();
				_client.EndDocument();
			}
			else if (_peek.Type == TokenType.Open)
			{
				while (Directive())
				{
				}

				_client.BeginDocument();
				Consume(TokenType.Open);

				while (ElementNode())
				{
				}

				Consume(TokenType.Close);
				_client.EndDocument();
			}

			Consume(TokenType.Eof);
		}

		private bool AttributeNode()
		{
			if (_peek.Type == TokenType.Open && (_peek2.Type == TokenType.Name || _peek2.Type == TokenType.Keyword))
			{
				Consume();
				string name = Consume().StringValue();
				_client.BeginAttribute(name);

				while (ValueNode())
				{
				}

				if (_peek.Type == TokenType.Name) // nice error message if quotes are missing
				{
					throw new ParseError(TokenType.String, _peek, _pos);
				}

				Consume(TokenType.Close);
				_client.EndAttribute(name);

				return true;
			}

			return false;
		}

		private Token Consume()
		{
			_pos = _pos2;
			_pos2 = _stream.Pos();
			Token current = _peek;
			_peek = _peek2;
			_peek2 = _stream.NextOrEof();

			return current;
		}

		private Token Consume(TokenType type)
		{
			if (_peek.Type != type)
			{
				throw new ParseError(type, _peek, _pos);
			}

			return Consume();
		}

		private bool Directive()
		{
			if (_peek.Type == TokenType.Open && _peek2.Type == TokenType.Keyword)
			{
				Consume();
				string name = Consume(TokenType.Keyword).StringValue();
				ICollection<string> parameters = new LinkedList<string>();

				while (_peek.Type == TokenType.String || _peek.Type == TokenType.Boolean || _peek.Type == TokenType.Number || _peek.Type == TokenType.Keyword || _peek.Type == TokenType.Name)
				{
					parameters.Add(Consume().StringValue());
				}

				Consume(TokenType.Close);
				_client.Directive(name, parameters.ToArray());

				return true;
			}

			return false;
		}

		private bool ElementNode()
		{
			if (_peek.Type == TokenType.Open && _peek2.Type == TokenType.Name)
			{
				Consume();
				string name = Consume(TokenType.Name).StringValue();

				_client.BeginElement(name);
				IdColon();
				while (AttributeNode())
				{
				}

				Consume(TokenType.Close);

				_client.EndElement(name);

				return true;
			}

			return false;
		}

		private bool IdColon()
		{
			if (_peek.Type == TokenType.Open && _peek2.Type == TokenType.Id)
			{
				Consume();
				Consume();
				int index = Consume(TokenType.Number).IntValue();

				_client.Serial(index);
				Consume(TokenType.Close);

				return true;
			}

			return false;
		}

		private bool Primitive()
		{
			if (_peek.Type == TokenType.String || _peek.Type == TokenType.Number || _peek.Type == TokenType.Boolean)
			{
				object value = Consume().Value;
				_client.Primitive(value);

				return true;
			}

			return false;
		}

		private bool RefColon()
		{
			if (_peek.Type == TokenType.Open && _peek2.Type == TokenType.Ref)
			{
				Consume();
				Consume();

				if (_peek.Type == TokenType.Name)
				{
					string name = Consume(TokenType.Name).StringValue();
					_client.Reference(name);
				}
				else if (_peek.Type == TokenType.Number)
				{
					int serial = Consume(TokenType.Number).IntValue();
					_client.Reference(serial);
				}
				else
				{
					throw new ParseError(TokenType.Number, _peek, _pos);
				}

				Consume(TokenType.Close);

				return true;
			}

			return false;
		}

		private bool UnknownColon()
		{
			if (_peek.Type == TokenType.Open && _peek2.Type == TokenType.Keyword)
			{
				throw new ParseError("Unknown selector #" + _peek2.Value, _pos2);
			}

			return false;
		}

		private bool ValueNode()
		{
			return ElementNode() || Primitive() || RefColon() || UnknownColon();
		}
	}
}