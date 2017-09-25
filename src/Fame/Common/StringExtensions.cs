using System;

namespace Fame.Common
{
	public static class StringExtensions
	{
		public static string ToUpperFirstChar(this string text)
		{
			if (text == null)
				return null;

			switch (text.Length)
			{
				case 0:
					return string.Empty;
				case 1:
					return text.ToUpperInvariant();
			}

			return text.Substring(0, 1).ToUpperInvariant() + text.Substring(1);
		}
	}
}
