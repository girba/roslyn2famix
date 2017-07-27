namespace Fame
{
	using System;

	using Fm3;

	//TODO : Continue here
	public class MetaRepository : Repository
    {
		private object p;

		public MetaRepository(object p)
		{
			this.p = p;
		}

		public static bool IsValidName(string name)
        {
            throw new System.NotImplementedException();
        }

	    public static MetaRepository CreateFm3()
	    {
		    throw new System.NotImplementedException();
	    }

	    public MetaDescription GetDescription(Type type)
	    {
		    throw new NotImplementedException();
	    }

	    public MetaDescription DescriptionNamed(string qname)
	    {
		    throw new NotImplementedException();
	    }
    }
}