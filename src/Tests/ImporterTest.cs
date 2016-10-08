namespace Tests
{
    using System;
    using Roslyn2Famix;
    using Xunit;
    using System.IO;

    /// <summary>
    /// Tests the <see cref="Importer"/> public API.
    /// </summary>
    public class ImporterTest
    {
        private readonly string pathToTestSolution;

        private readonly Importer importerUnderTest;

        public ImporterTest()
        {
            const string solutionDirectoryPath = @"..\..\..\..\..\..\";
            this.pathToTestSolution = Path.Combine(solutionDirectoryPath, "Roslyn2Famix.sln");

            this.importerUnderTest = new Importer();
        }

        /// <summary>
        /// Tests the Strategy <see cref="Importer.Run"/> method.
        /// </summary>
        [Fact]
        public void Run_Test()
        {
            // Arrange
            var expectedFamix = "(Class SimpleClassWithMethod)" + Environment.NewLine +
                                "(Method SimpleMethod)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod end)" + Environment.NewLine;

            // Act
            importerUnderTest.Run(this.pathToTestSolution);

            // Assert
            var parsedFamix = importerUnderTest.ExportToString();

            Assert.Equal(parsedFamix, expectedFamix);
        }
    }
}
