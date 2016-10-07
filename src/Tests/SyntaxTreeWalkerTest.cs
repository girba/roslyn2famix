namespace Tests
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Roslyn2Famix;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SyntaxTreeWalker"/> public API.
    /// </summary>
    public class SyntaxTreeWalkerTest
    {
        private readonly SyntaxTree testSyntaxTree;
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

            this.walkerUnderTest = new SyntaxTreeWalker();
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
            var parsedFamix = walkerUnderTest.ToFamixString();

            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
