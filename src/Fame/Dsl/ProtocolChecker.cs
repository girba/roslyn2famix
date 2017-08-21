namespace Fame.Dsl
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using Parser;

	public class ProtocolChecker : IParseClient
	{
		private readonly LinkedList<string> _stack = new LinkedList<string>();
		private State[] _expectedState = { State.BeginDocument, State.Directive };

		public enum State
		{
			Directive,

			BeginDocument,
			EndDocument,

			BeginElement,
			EndElement,

			BeginAttribute,
			EndAttribute,

			Reference,
			Primitive,

			Serial
		}

		public IParseClient Client;

		public ProtocolChecker(IParseClient client)
		{
			Client = client;
		}

		public void BeginAttribute(string name)
		{
			_stack.AddLast(name);
			CheckState(State.BeginAttribute);
			ExpectState(State.BeginElement, State.EndAttribute, State.Primitive, State.Reference);
			Client.BeginAttribute(name);
		}

		private void CheckState(State state)
		{
			if (_expectedState.Any(each => each == state))
				return;

			Debug.Assert(false, "Expected " + _expectedState + " but was " + state);
		}

		public void BeginDocument()
		{
			CheckState(State.BeginDocument);
			ExpectState(State.BeginElement, State.EndDocument);
			Client.BeginDocument();
		}

		private void ExpectState(params State[] states)
		{
			_expectedState = states;
		}

		public void BeginElement(string name)
		{
			_stack.AddLast(name);
			CheckState(State.BeginElement);
			ExpectState(State.BeginAttribute, State.Serial, State.EndElement);
			Client.BeginElement(name);
		}

		public void Directive(string name, params string[] @params)
		{
			CheckState(State.Directive);
			ExpectState(State.BeginDocument, State.Directive);
			Client.Directive(name, @params);
		}

		public void EndAttribute(string name)
		{
			Debug.Assert(name.Equals(_stack.Last.Value));
			_stack.RemoveLast();
			CheckState(State.EndAttribute);
			ExpectState(State.BeginAttribute, State.EndElement);
			Client.EndAttribute(name);
		}

		public void EndDocument()
		{
			CheckState(State.EndDocument);
			ExpectState();
			Client.EndDocument();
		}

		public void EndElement(string name)
		{
			Debug.Assert(name.Equals(_stack.Last.Value));

			_stack.RemoveLast();
			CheckState(State.EndElement);

			if (_stack.Count == 0)
				ExpectState(State.BeginElement, State.EndDocument);
			else
				ExpectState(State.BeginElement, State.EndAttribute, State.Primitive, State.Reference);

			Client.EndElement(name);
		}

		public void Primitive(object value)
		{
			CheckState(State.Primitive);
			ExpectState(State.BeginElement, State.EndAttribute, State.Primitive, State.Reference);
			Client.Primitive(value);
		}

		public void Reference(int index)
		{
			CheckState(State.Reference);
			ExpectState(State.BeginElement, State.EndAttribute, State.Primitive, State.Reference);
			Client.Reference(index);
		}

		public void Reference(string name)
		{
			CheckState(State.Reference);
			ExpectState(State.BeginElement, State.EndAttribute, State.Primitive, State.Reference);
			Client.Reference(name);
		}

		public void Reference(string name, int index)
		{
			//throw new AssertionError("Not yet implemented!");
			throw new NotImplementedException();
		}

		public void Serial(int index)
		{
			CheckState(State.Serial);
			ExpectState(State.BeginAttribute, State.EndElement);
			Client.Serial(index);
		}
	}
}