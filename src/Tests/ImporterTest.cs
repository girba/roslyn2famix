namespace Tests
{
    using System;
    using Roslyn2FamixImporter;
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
            const string solutionDirectoryPath = @"..\..\..\..\";
            this.pathToTestSolution = Path.Combine(solutionDirectoryPath, "Tests.Fixtures.sln");

            this.importerUnderTest = new Importer();
        }

        /// <summary>
        /// Tests the Strategy <see cref="Importer.Run"/> method.
        /// </summary>
        [Fact]
        public void Run_Test()
        {
            // Arrange
            var expectedFamix = "(Solution)" + Environment.NewLine +
                                "(Project Tests.Fixtures)" + Environment.NewLine +
                                "(Assembly Tests.Fixtures)" + Environment.NewLine +
                                "(Namespace)" + Environment.NewLine +
                                "(Namespace Tests)" + Environment.NewLine +
                                "(Namespace TestData)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod)" + Environment.NewLine +
                                "(Method SimpleMethod)" + Environment.NewLine +
                                "(Method SimpleMethod end)" + Environment.NewLine +
                                "(Class SimpleClassWithMethod end)" + Environment.NewLine +
                                "(Namespace TestData end)" + Environment.NewLine +
                                "(Namespace Tests end)" + Environment.NewLine +
                                "(Namespace end)" + Environment.NewLine +
                                "(Assembly Tests.Fixtures end)" + Environment.NewLine +
                                "(Project Tests.Fixtures end)" + Environment.NewLine +
                                "(Solution end)" + Environment.NewLine;

            // Act
            importerUnderTest.Run(this.pathToTestSolution);

            // Assert
            var parsedFamix = importerUnderTest.ExportToString();

            Assert.Equal(expectedFamix, parsedFamix);
        }
    }
}
