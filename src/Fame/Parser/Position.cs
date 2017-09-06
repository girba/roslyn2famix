namespace Fame.Parser
{
	public class Position
	{
		public readonly int Line;
		public readonly int Column;
		public readonly int Index;

		public Position(int line, int column, int index)
		{
			Line = line;
			Column = column;
			Index = index;
		}

		public override string ToString()
		{
			return Line + ":" + Column;
		}
	}
}