namespace CourseworkTPO
{
    public static class DatasetGenerator
    {
        private static readonly Random _random = new Random();

        private static readonly Dictionary<string, string[]> _topicSentences = new Dictionary<string, string[]>
        {
            {
                "DataScience", new[]
                {
                    "Machine learning models require clean and structured data.",
                    "Algorithms like gradient boosting and neural networks are extremely powerful.",
                    "Predictive analytics helps in forecasting future trends accurately.",
                    "Parallel computing is often used to process massive datasets efficiently.",
                    "Data engineers build pipelines to automate the flow of information.",
                    "Model evaluation metrics include accuracy, precision, and recall."
                }
            },
            {
                "WebDevelopment", new[]
                {
                    "Modern web development relies heavily on robust frontend frameworks.",
                    "Component-based architecture allows developers to reuse code easily.",
                    "Backend APIs ensure seamless communication between the server and the client.",
                    "Cloud platforms provide scalable infrastructure for web applications.",
                    "Responsive design ensures the application looks good on mobile devices.",
                    "Security protocols must be implemented to protect user authentication."
                }
            },
            {
                "FitnessAndHealth", new[]
                {
                    "Regular physical activity significantly improves cardiovascular health.",
                    "Tracking daily caloric intake helps maintain a healthy energy balance.",
                    "Choreography and dynamic dance routines enhance coordination and flexibility.",
                    "Proper nutrition and hydration are essential for optimal muscle recovery.",
                    "A consistent step count goal is a great way to stay active throughout the day.",
                    "Strength training builds endurance and supports metabolic functions."
                }
            },
            {
                "Cinema", new[]
                {
                    "The cinematic experience combines visual storytelling with immersive sound design.",
                    "Independent films often explore unique narratives and unconventional characters.",
                    "Theater screenings provide a shared emotional environment for the audience.",
                    "Directors carefully use lighting and camera angles to set the mood.",
                    "A well-written screenplay is the foundation of any successful movie.",
                    "Post-production visual effects can completely transform the final scene."
                }
            }
        };

        public static void GenerateFile(string filePath, int numberOfParagraphs)
        {
            var topics = new List<string>(_topicSentences.Keys);

            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < numberOfParagraphs; i++)
                {
                    string selectedTopic = topics[_random.Next(topics.Count)];
                    string[] sentencesPool = _topicSentences[selectedTopic];

                    int sentencesInParagraph = _random.Next(3, 6);

                    var paragraphSentences = sentencesPool
                        .OrderBy(x => _random.Next())
                        .Take(sentencesInParagraph);

                    writer.Write(string.Join(" ", paragraphSentences));

                    if (i < numberOfParagraphs - 1)
                    {
                        writer.Write("\n\n");
                    }
                }
            }

            Console.WriteLine($"Створено файл {filePath} із {numberOfParagraphs} документами.");
        }

        public static string[] GenerateTestDataset(int numberOfDocuments)
        {
            var topics = new List<string>(_topicSentences.Keys);

            var dataset = new string[numberOfDocuments];

            for (int i = 0; i < numberOfDocuments; i++)
            {
                string selectedTopic = topics[_random.Next(topics.Count)];
                string[] sentencesPool = _topicSentences[selectedTopic];

                int sentencesInParagraph = _random.Next(3, 6);

                var paragraphSentences = sentencesPool
                    .OrderBy(x => _random.Next())
                    .Take(sentencesInParagraph);

                dataset[i] = string.Join(" ", paragraphSentences);
            }

            return dataset;
        }
    }
}