namespace Fame.Internal
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Common;

	public class MsePrinter : AbstractPrintClient
	{
		public static readonly object Unlimited = new object();

		public MsePrinter(TextWriter stream) : base(stream)
		{
		}

		public override void BeginAttribute(string name)
		{
			Indentation++;
			Lntabs();
			Append('(');
			Append(name.ToCharArray());
		}

		public override void BeginDocument()
		{
			// indentation++;
			Append('(');
		}

		public override void BeginElement(string name)
		{
			Indentation++;
			Lntabs();
			Append('(');
			Append(name.ToCharArray());
		}

		public override void EndAttribute(string name)
		{
			Append(')');
			Indentation--;
		}

		public override void EndDocument()
		{
			Append(')');
			Close();
		}

		public override void EndElement(string name)
		{
			Append(')');
			Indentation--;
		}

		public override void Primitive(object value)
		{
			Append(' ');

			if (value == Unlimited)
			{
				Append('*');
			}
			else if (value is string)
			{
				string @string = (string)value;
				Append('\'');

				foreach (char ch in @string)
				{
					if (ch == '\'')
					{
						Append('\'');
					}

					Append(ch);
				}

				Append('\'');
			}
			else if (value is bool || value.IsNumber())
			{
				Append(value.ToString().ToCharArray());
			}
			else if (value is DateTime)
			{				
				Append(((DateTime)value).ToString("yyyy-MM-dd,hh:mm:ss").ToCharArray());
			}
			else if (value is char)
			{
				Append(("'" + value + "'").ToCharArray());
			}
			else if (value is char[])
			{
				Primitive(new string((char[])value));
			}
			else
			{
				Debug.Assert(false, "Unknown primitive: " + value + " of type: " + value.GetType().FullName);
			}
		}

		public override void Reference(int index)
		{
			Stream.Write(" (ref: "); // must prepend space!
			Stream.Write(index);
			Stream.Write(')');
		}

		public override void Reference(string name)
		{
			Stream.Write(" (ref: "); // must prepend space!
			Stream.Write(name);
			Stream.Write(')');
		}

		public override void Serial(int index)
		{
			Stream.Write(" (id: "); // must prepend space!
			Stream.Write(Convert.ToString(index));
			Stream.Write(')');
		}
	}
}