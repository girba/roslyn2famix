using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fame.Internal
{
	using System;
	using Parser;

	// TODO
	public class RepositoryVisitor
	{
		private Repository _repo;
		private Dictionary<object, int> _index;
		private IParseClient _visitor;

		public RepositoryVisitor(Repository repository, IParseClient visitor)
		{
			throw new NotImplementedException();
		}


		public void Run()
		{
			Debug.Assert(_index != null, "Can not run the same visitor twice.");

			AcceptVisitor();
			_index = null;
		}

		private void AcceptVisitor()
		{
			throw new NotImplementedException();
		}
	}
}