namespace Fame
{
	using System.Diagnostics;

	public class Tower
	{
		public Repository model;
    	public MetaRepository metamodel;
    	public MetaRepository metaMetamodel;
    
		private Tower(MetaRepository m3, MetaRepository m2, Repository m1)
		{
			metaMetamodel = m3;
			metamodel = m2 ?? new MetaRepository(metaMetamodel);
			model = m1 ?? new Repository(metamodel);

			Debug.Assert(metaMetamodel.GetMetamodel().Equals(metamodel));
			Debug.Assert(metamodel.GetMetamodel().Equals(metaMetamodel));
			Debug.Assert(model.GetMetamodel().Equals(metamodel));
		}

		public Tower() : this(MetaRepository.CreateFM3(), null, null)
		{
		}

		public MetaRepository GetMetaMetamodel()
		{
			return metaMetamodel;
		}

		public MetaRepository GetMetamodel()
		{
			return metamodel;
		}

		public Repository GetModel()
		{
			return model;
		}
	}
}