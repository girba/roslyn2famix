using Fame;
using Xunit;

namespace Tests.Fame.Tests
{
	public class Fm3MetaMetaModelShould
	{
		[Fact]
		public void Have_4_classes()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();
			Assert.Equal(4, m3.AllClassDescriptions().Count);
		}

		[Fact]
		public void Have_1_package()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();
			Assert.Equal(1, m3.AllPackageDescriptions().Count);
		}

		[Fact]
		public void Have_20_properties()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();
			Assert.Equal(20, m3.AllPropertyDescriptions().Count);
		}

		[Fact]
		public void Pass_constraints_check()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();

			m3.CheckConstraints();
		}

		[Fact]
		public void Be_self_described()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();

			Assert.Equal(m3, m3.GetMetamodel());
		}

		[Fact]
		public void Have_correctly_named_descriptions()
		{
			MetaRepository m3 = MetaRepository.CreateFm3();

			Assert.NotNull(m3.DescriptionNamed("FM3.Class"));
			Assert.NotNull(m3.DescriptionNamed("FM3.Package"));
			Assert.NotNull(m3.DescriptionNamed("FM3.Property"));
		}
	}
}