namespace Tests.FamixTests
{
    using Famix;
    using Famix.Language;
    using System;
    using Xunit;

    public class FamixTreeBuilderTest
    {
        private readonly FamixTreeBuilder builderUnderTest;

        public FamixTreeBuilderTest()
        {
            this.builderUnderTest = new FamixTreeBuilder();
        }

        /// <summary>
        /// Tests the <see cref="FamixTreeBuilder.EndClass"/> method.
        /// </summary>
        [Fact]
        public void EndClass_Test()
        {
            // Arrange
            var expectedFamix = "(Class TestClass)" + Environment.NewLine +
                                "(Class TestClass end)" + Environment.NewLine;

            builderUnderTest.CreateClass("TestClass");

            // Act
            builderUnderTest.EndClass("TestClass");

            // Assert
            var parsedFamix = builderUnderTest.ToFamixString();

            Assert.Equal(parsedFamix, expectedFamix);
        }

        /// <summary>
        /// Tests the <see cref="FamixTreeBuilder.EndClass"/> method.
        /// </summary>
        [Fact]
        public void EndClass_UnexpectedClassName_Test()
        {
            // Arrange
            builderUnderTest.CreateClass("TestClass");

            // Act
            // Assert
            Assert.Throws<UnexpectedNodeNameException<Class>>( () => 
                builderUnderTest.EndClass("UnexpectedTestClass"));
        }
    }
}
