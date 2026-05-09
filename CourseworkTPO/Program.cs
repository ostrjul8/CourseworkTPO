using System.Diagnostics;

namespace CourseworkTPO
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int[] documentSizes = { 10_000, 20_000, 50_000, 100_000, 200_000, 500_000, 1_000_000, 2_000_000 };
            int[] threadConfigs = { 2, 4, 8, 12, 16 };
            int numberOfRuns = 20;

            Console.WriteLine("Ініціалізація NLP-моделей.");
            var processor = new TextProcessor();
            await processor.InitializeAsync();
            Console.WriteLine("NLP-моделі успішно завантажено.\n");

            foreach (int size in documentSizes)
            {
                Console.WriteLine($"\n=====================================================================");
                Console.WriteLine($"[ ЕКСПЕРИМЕНТ: {size} документів ]");
                Console.WriteLine($"=====================================================================");

                string inputFilePath = $"input_{size}.txt";

                DatasetGenerator.GenerateFile(inputFilePath, size);

                Console.WriteLine("Зчитування файлу.");
                string[]? rawDocuments = ReadDocumentsStream(inputFilePath);

                Console.WriteLine("Виконання передобробки текстів.");
                string[][]? processedDocuments = new string[rawDocuments.Length][];

                var preprocessOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
                Parallel.For(0, rawDocuments.Length, preprocessOptions, i =>
                {
                    processedDocuments[i] = processor.ProcessText(rawDocuments[i]);
                });

                rawDocuments = null;
                ForceGarbageCollection();

                Console.WriteLine($"\n--- Послідовний алгоритм (середнє за {numberOfRuns} прогонів) ---");
                double totalSeqTimeMs = 0;

                for (int run = 1; run <= numberOfRuns; run++)
                {
                    ForceGarbageCollection();
                    Stopwatch seqStopwatch = Stopwatch.StartNew();
                    var tfidfResultsSeq = TfIdfSequential.ComputeVectors(processedDocuments);
                    seqStopwatch.Stop();

                    totalSeqTimeMs += seqStopwatch.Elapsed.TotalMilliseconds;
                    Console.Write($"\rПрогін {run}/{numberOfRuns} завершено.");
                }

                double avgSeqTimeMs = totalSeqTimeMs / numberOfRuns;
                Console.WriteLine($"\nСередній час (послідовний): {avgSeqTimeMs:F2} мс");

                Console.WriteLine($"\n--- Паралельний алгоритм (середнє за {numberOfRuns} прогонів) ---");

                foreach (int threads in threadConfigs)
                {
                    double totalParTimeMs = 0;

                    for (int run = 1; run <= numberOfRuns; run++)
                    {
                        ForceGarbageCollection();
                        Stopwatch parStopwatch = Stopwatch.StartNew();
                        var tfidfResultsPar = TfIdfParallel.ComputeVectors(processedDocuments, threads);
                        parStopwatch.Stop();

                        totalParTimeMs += parStopwatch.Elapsed.TotalMilliseconds;
                    }

                    double avgParTimeMs = totalParTimeMs / numberOfRuns;
                    double speedup = avgParTimeMs > 0 ? avgSeqTimeMs / avgParTimeMs : 0;
                    double efficiency = speedup / threads;

                    Console.WriteLine($"Потоків: {threads,2} | Час: {avgParTimeMs,8:F2} мс | Прискорення: {speedup,6:F4} | Ефективність: {efficiency,6:F4}");
                }

                processedDocuments = null;
                ForceGarbageCollection();
            }

            Console.WriteLine("\nВсі експерименти завершено.");
        }

        static void ForceGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        static string[] ReadDocumentsStream(string filePath)
        {
            var documents = new List<string>();
            var currentDoc = new System.Text.StringBuilder();

            foreach (var line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentDoc.Length > 0)
                    {
                        documents.Add(currentDoc.ToString().Trim());
                        currentDoc.Clear();
                    }
                }
                else
                {
                    currentDoc.AppendLine(line);
                }
            }

            if (currentDoc.Length > 0)
            {
                documents.Add(currentDoc.ToString().Trim());
            }

            return documents.ToArray();
        }
    }
}