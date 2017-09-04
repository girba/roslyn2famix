namespace Fame.Common
{
	using System;

	public class AssertionError : Exception
	{
		public AssertionError()
		{
		}

		public AssertionError(Exception ex) : base (string.Empty, ex)
		{
		}
	}
}