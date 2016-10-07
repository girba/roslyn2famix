namespace Tests
{
    using System;
    using Roslyn2Famix;
    using Xunit;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.MSBuild;
    using System.Linq;
    using System.IO;
    using TestData;

    /// <summary>
    /// Tests the <see cref="SyntaxTreeWalker"/> public API.
    /// </summary>
    public class SymbolWalkerTest
    {
        private INamedTypeSymbol testFixtureClass;
        private SymbolWalker walkerUnderTest;

        public SymbolWalkerTest()
        {
            const string testFixturesProjectName = "Tests.Fixtures";
            string pathToSolution = Path.Combine(@"..\..\..\..\..\..\", "Roslyn2Famix.sln");
            
            var workspace = MSBuildWorkspace.Create();

            var solution = workspace.OpenSolutionAsync(pathToSolution).Result;

            var testFixturesProject = solution.Projects
                    .Where(project => project.Name == testFixturesProjectName)
                    .First();

            var compilation = testFixturesProject.GetCompilationAsync().Result;

            this.testFixtureClass = compilation.GetTypeByMetadataName(typeof(SimpleClassWithMethod).FullName);
            
            this.walkerUnderTest = new SymbolWalker();
        }

        /// <summary>
        /// Tests the Strategy <see cref="SymbolWalker.Visit"/> method.
        /// </summary>
        [Fact]
        public void Visit_Test()
        {
            // Arrange
            var expectedFamix = "(Class SimpleClassWithMethod)" + Environment.NewLine +
                                "(Method SimpleMethod)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod end)" + Environment.NewLine;

            // Act
            walkerUnderTest.Visit(testFixtureClass);
            
            // Assert
            var parsedFamix = walkerUnderTest.ToFamixString();

            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
