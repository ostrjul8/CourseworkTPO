namespace CourseworkTPO.Tests
{
    public class TfidfSequentialParallelTests
    {
        [Fact]
        public async Task ParallelAlgorithm_ShouldMatchSequentialResults_UnderHeavyLoad()
        {
            var textProcessor = new TextProcessor();
            await textProcessor.InitializeAsync();

            int documentCount = 100000;

            string[] rawDocuments = DatasetGenerator.GenerateTestDataset(documentCount);

            string[][] testCorpus = new string[documentCount][];
            for (int i = 0; i < documentCount; i++)
            {
                testCorpus[i] = textProcessor.ProcessText(rawDocuments[i]);
            }

            var sequentialResults = TfIdfSequential.ComputeVectors(testCorpus);
            var parallelResults = TfIdfParallel.ComputeVectors(testCorpus);

            Assert.Equal(sequentialResults.Length, parallelResults.Length);

            for (int i = 0; i < documentCount; i++)
            {
                Assert.Equal(sequentialResults[i].Count, parallelResults[i].Count);

                foreach (var kvp in sequentialResults[i])
                {
                    string word = kvp.Key;
                    double expectedWeight = kvp.Value;

                    Assert.True(parallelResults[i].ContainsKey(word));

                    double actualWeight = parallelResults[i][word];
                    Assert.Equal(expectedWeight, actualWeight, precision: 6);
                }
            }
        }
    }
}