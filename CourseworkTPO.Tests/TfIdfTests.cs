namespace CourseworkTPO.Tests
{
    public class TfIdfTests
    {
        [Fact]
        public void ComputeVectors_ShouldCalculateTfIdfCorrectly()
        {
            // Arrange
            string[][] processedDocuments = new[]
            {
                new[] { "kyiv", "district", "electricity", "forecast" },  
                new[] { "district", "electricity", "consumption", "model" },
                new[] { "electricity", "model", "model", "catboost" }
            };

            // Act
            var tfidfResults = TfIdfSequential.ComputeVectors(processedDocuments);

            // Assert
            Assert.Equal(3, tfidfResults.Length);

            Assert.True(tfidfResults[0].ContainsKey("electricity"));
            Assert.Equal(0.0, tfidfResults[0]["electricity"], precision: 4);
            Assert.Equal(0.0, tfidfResults[1]["electricity"], precision: 4);
            Assert.Equal(0.0, tfidfResults[2]["electricity"], precision: 4);

            Assert.True(tfidfResults[0]["kyiv"] > 0);

            Assert.True(tfidfResults[2]["model"] > tfidfResults[1]["model"]);
        }

        [Fact]
        public void ComputeVectors_EmptyDocuments_ShouldReturnEmptyResults()
        {
            // Arrange
            string[][] noDocuments = new string[0][];

            // Act
            var results = TfIdfSequential.ComputeVectors(noDocuments);

            // Assert
            Assert.Empty(results);
        }
    }
}