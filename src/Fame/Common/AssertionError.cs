namespace Fame.Common
{
	using System;

	public class AssertionError : Exception
	{
		public AssertionError()
		{
		}

		public AssertionError(string message) : base(message)
		{
		}

		public AssertionError(Exception ex) : base (string.Empty, ex)
		{
		}
	}
}