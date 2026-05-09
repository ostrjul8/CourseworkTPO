using System.Collections.Concurrent;

namespace CourseworkTPO
{
    public static class TfIdfParallel
    {
        public static Dictionary<string, double>[] ComputeVectors(string[][] documents, int maxThreads)
        {
            int documentsCount = documents.Length;

            var termFrequency = new Dictionary<string, int>[documentsCount];

            var documentFrequency = new ConcurrentDictionary<string, int>();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxThreads
            };

            Parallel.For(0, documentsCount, options, i =>
            {
                var currentDocTf = new Dictionary<string, int>();
                string[] words = documents[i];

                foreach (string word in words)
                {
                    if (currentDocTf.TryGetValue(word, out int count))
                    {
                        currentDocTf[word] = count + 1;
                    }
                    else
                    {
                        currentDocTf[word] = 1;
                    }
                }
                termFrequency[i] = currentDocTf;

                foreach (string uniqueWord in currentDocTf.Keys)
                {
                    documentFrequency.AddOrUpdate(uniqueWord, 1, (key, oldValue) => oldValue + 1);
                }
            });

            var tfIdfVectors = new Dictionary<string, double>[documentsCount];

            Parallel.For(0, documentsCount, options, i =>
            {
                tfIdfVectors[i] = new Dictionary<string, double>();
                var currentDocTf = termFrequency[i];

                foreach (var kvp in currentDocTf)
                {
                    string word = kvp.Key;
                    int termFreq = kvp.Value;
                    int docFreq = documentFrequency[word];

                    double idf = Math.Log((double)documentsCount / docFreq);
                    tfIdfVectors[i][word] = termFreq * idf;
                }
            });

            return tfIdfVectors;
        }
    }
}