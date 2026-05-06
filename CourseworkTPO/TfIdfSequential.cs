namespace CourseworkTPO
{
    public static class TfIdfSequential
    {
        public static Dictionary<string, double>[] ComputeVectors(string[][] documents)
        {
            int documentsCount = documents.Length;

            var termFrequency = new Dictionary<string, int>[documentsCount];
            var documentFrequency = new Dictionary<string, int>();

            for (int i = 0; i < documentsCount; i++)
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
                    if (documentFrequency.TryGetValue(uniqueWord, out int dfCount))
                    {
                        documentFrequency[uniqueWord] = dfCount + 1;
                    }
                    else
                    {
                        documentFrequency[uniqueWord] = 1;
                    }
                }
            }

            var tfIdfVectors = new Dictionary<string, double>[documentsCount];

            for (int i = 0; i < documentsCount; i++)
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
            }

            return tfIdfVectors;
        }
    }
}
