using System;
using System.Collections.Generic;
using Fame.Parser;

namespace Fame.Dsl
{
	public class MetamodelBuilder
	{
		// TODO

		private IParseClient client;
		private Dictionary<string, int> indexDict;

		public MetamodelBuilder(IParseClient client)
		{
			this.client = client;
			this.indexDict = new Dictionary<string, int>();
		}
	}
}