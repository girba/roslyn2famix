using System;

namespace Fame.Parser
{
	// TODO
	public class Importer : AbstractParserClient
	{
		private MetaRepository metamodel;
		private Repository model;

		public Importer(MetaRepository metamodel) : this(metamodel, new Repository(metamodel))
		{
		}

		public Importer(MetaRepository metamodel, Repository model)
		{
			this.metamodel = metamodel;
			this.model = model;
		}

		public void ReadFrom(InputSource input)
		{
			throw new NotImplementedException();
		}

		public Repository GetResult()
		{
			throw new NotImplementedException();
		}
	}
}