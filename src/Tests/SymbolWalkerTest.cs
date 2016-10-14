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
    using Famix;

    /// <summary>
    /// Tests the <see cref="SymbolWalker"/> class.
    /// </summary>
    public class SymbolWalkerTest
    {
        private readonly INamedTypeSymbol testFixtureClass;

        private readonly FamixTreeBuilder builderUnderTest;
        private readonly SymbolWalker walkerUnderTest;

        public SymbolWalkerTest()
        {
            const string testFixturesProjectName = "Tests.Fixtures";
            const string solutionDirectoryPath = @"..\..\..\..\..\..\";
            string pathToSolution = Path.Combine(solutionDirectoryPath, "Roslyn2Famix.sln");

            var workspace = MSBuildWorkspace.Create();

            var solution = workspace.OpenSolutionAsync(pathToSolution).Result;

            var testFixturesProject = solution.Projects
                    .Where(project => project.Name == testFixturesProjectName)
                    .First();

            var compilation = testFixturesProject.GetCompilationAsync().Result;

            this.testFixtureClass = compilation.GetTypeByMetadataName(typeof(SimpleClassWithMethod).FullName);

            this.builderUnderTest = new FamixTreeBuilder();
            this.walkerUnderTest = new SymbolWalker(builderUnderTest);
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
            var parsedFamix = builderUnderTest.ToFamixString();

            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
