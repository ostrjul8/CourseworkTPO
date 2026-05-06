namespace CourseworkTPO.Tests
{
    public class TextProcessorTests
    {
        [Fact]
        public async Task ProcessText_ShouldReturnProcessedTokens_AndIgnoreStopWords()
        {
            // Arrange
            var processor = new TextProcessor();
            await processor.InitializeAsync();

            string rawText = "apple is purely a green banana";

            // Act
            string[] result = processor.ProcessText(rawText);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.DoesNotContain("is", result);
            Assert.DoesNotContain("a", result);

            Assert.Contains("apple", result);
            Assert.Contains("banana", result);
        }

        [Fact]
        public async Task ProcessText_EmptyString_ShouldReturnEmptyArray()
        {
            // Arrange
            var processor = new TextProcessor();
            await processor.InitializeAsync();

            // Act
            string[] result = processor.ProcessText("");

            // Assert
            Assert.Empty(result);
        }
    }
}