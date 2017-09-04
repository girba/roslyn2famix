namespace Fame.Parser
{
	using System;
	using System.Collections.Generic;
	using Common;

	// TODO: Check for correctness
	public sealed class Primitive
	{
		public static readonly Primitive Object = new Primitive("OBJECT", InnerEnum.Object, typeof(object));
		public static readonly Primitive String = new Primitive("STRING", InnerEnum.String, typeof(string));
		public static readonly Primitive Number = new Primitive("NUMBER", InnerEnum.Number, typeof(int));  // TODO: typeof(Number) does not exist in C#
		public static readonly Primitive Boolean = new Primitive("BOOLEAN", InnerEnum.Boolean, typeof(bool));

		private static readonly IList<Primitive> ValueList = new List<Primitive>();

		static Primitive()
		{
			ValueList.Add(Object);
			ValueList.Add(String);
			ValueList.Add(Number);
			ValueList.Add(Boolean);
		}

		public enum InnerEnum
		{
			Object,
			String,
			Number,
			Boolean
		}

		public readonly InnerEnum InnerEnumValue;
		private readonly string _nameValue;
		private readonly int _ordinalValue;
		private static int _nextOrdinal;

		public static Primitive ValueOf(object value)
		{
			if (value is string)
			{
				return String;
			}

			if (value.IsNumber())
			{
				return Number;
			}

			if (value is bool)
			{
				return Boolean;
			}

			throw new Exception("Unknown type of primitive");
		}

		internal Primitive(string name, InnerEnum innerEnum, Type type)
		{
			Type = type;
			_nameValue = name;
			_ordinalValue = _nextOrdinal++;
			InnerEnumValue = innerEnum;
		}

		public Type Type { get; }

		public static IList<Primitive> Values()
		{
			return ValueList;
		}

		public int Ordinal()
		{
			return _ordinalValue;
		}

		public override string ToString()
		{
			return _nameValue;
		}
	}
}