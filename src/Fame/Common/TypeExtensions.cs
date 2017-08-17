using System;

namespace Fame.Common
{
	public static class TypeExtensions
	{
		public static bool IsNumber(this Type value)
		{
			return value == typeof(sbyte) ||
			       value == typeof(byte) ||
			       value == typeof(short) ||
			       value == typeof(ushort) ||
			       value == typeof(int) ||
			       value == typeof(uint) ||
			       value == typeof(long) ||
			       value == typeof(ulong) ||
			       value == typeof(float) ||
			       value == typeof(double) ||
			       value == typeof(decimal);
		}
	}
}