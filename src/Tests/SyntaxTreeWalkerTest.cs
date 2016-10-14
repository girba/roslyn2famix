namespace Tests
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Roslyn2Famix;
    using Xunit;
    using Famix;

    /// <summary>
    /// Tests the <see cref="SyntaxTreeWalker"/> class.
    /// </summary>
    public class SyntaxTreeWalkerTest
    {
        private readonly SyntaxTree testSyntaxTree;

        private readonly FamixTreeBuilder builderUnderTest;
        private readonly SyntaxTreeWalker walkerUnderTest;

        public SyntaxTreeWalkerTest()
        {
            this.testSyntaxTree = CSharpSyntaxTree.ParseText(@"
                public class SimpleClassWithMethod
                {
                    public void SimpleMethod()
                    {
                        Console.WriteLine(""Hello!"");
                    }
                }");

            this.builderUnderTest = new FamixTreeBuilder();
            this.walkerUnderTest = new SyntaxTreeWalker(builderUnderTest);
        }

        /// <summary>
        /// Tests the Strategy <see cref="SyntaxTreeWalker.Visit"/> method.
        /// </summary>
        [Fact]
        public void Visit_Test()
        {
            // Arrange
            var expectedFamix = "(Class SimpleClassWithMethod)" + Environment.NewLine +
                                "(Method SimpleMethod)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod end)" + Environment.NewLine;

            // Act
            walkerUnderTest.Visit(testSyntaxTree.GetRoot());

            // Assert
            var parsedFamix = builderUnderTest.ToFamixString();

            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
