using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famix.Language;
using Microsoft.CodeAnalysis.CSharp;
using Roslyn2Famix;
using Xunit;

namespace Tests
{
    /// <summary>
    /// Tests the <see cref="Class"/> public API.
    /// </summary>
    public class ClassTest
    {
        private Class1 instanceUnderTest;

        /// <summary>
        /// Common set of functions that are performed just before each test
        /// method is called.
        /// </summary>
        public ClassTest()
        {
            this.instanceUnderTest = new Class1();   
        }

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
                                "(Class SimpleClassWithMethod)" + Environment.NewLine;

            // Act
            var parsedFamix = walker.ToFamixString();

            Console.WriteLine(parsedFamix);

            // Assert
            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
