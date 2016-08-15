namespace Tests
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;
    using Roslyn2Famix;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SyntaxTreeWalker"/> public API.
    /// </summary>
    public class SyntaxTreeWalkerTest
    {
        /// <summary>
        /// Tests that the Strategy is initialized correctly.
        /// </summary>
        [Fact]
        public void Method_Test()
        {
            // Arrange
            var tree = CSharpSyntaxTree.ParseText(@"
                public class SimpleClassWithMethod
                {
                    public void SimpleMethod()
                    {
                        Console.WriteLine(""Hello!"");
                    }
                }");

            var walker = new SyntaxTreeWalker();
            walker.Visit(tree.GetRoot());
            
            var expectedFamix = "(Class SimpleClassWithMethod)" + Environment.NewLine +
                                "(Method SimpleMethod)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod end)" + Environment.NewLine;

            // Act
            var parsedFamix = walker.ToFamixString();

            Console.WriteLine(parsedFamix);

            // Assert
            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
