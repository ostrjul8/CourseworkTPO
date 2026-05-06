using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;
using Catalyst;
using Mosaik.Core;

namespace CourseworkTPO
{
    public class TextProcessor
    {
        private HashSet<string> _stopWords;
        private WordList _hunspellDict;
        private Pipeline _nlpPipeline;

        public async Task InitializeAsync()
        {
            _stopWords = new HashSet<string>(StopWord.StopWords.GetStopWords("en"));
            _hunspellDict = WordList.CreateFromFiles("en_US.dic", "en_US.aff");

            Catalyst.Models.English.Register();

            _nlpPipeline = await Pipeline.ForAsync(Language.English);
        }

        public string[] ProcessText(string rawText)
        {
            string text = Regex.Replace(rawText, "<.*?>", " ");

            var document = new Document(text, Language.English);
            _nlpPipeline.ProcessSingle(document);

            var validTokens = new List<string>();

            foreach (var sentence in document)
            {
                foreach (var token in sentence)
                {
                    string word = (token.Lemma ?? token.Value).ToLower();

                    if (string.IsNullOrWhiteSpace(word) || word.Length <= 1 || char.IsPunctuation(word[0]))
                        continue;

                    if (_stopWords.Contains(word))
                        continue;

                    if (!_hunspellDict.Check(word))
                    {
                        var suggestions = _hunspellDict.Suggest(word);
                        if (suggestions.Any())
                        {
                            word = suggestions.First().ToLower();
                        }
                    }

                    validTokens.Add(word);
                }
            }

            return validTokens.ToArray();
        }
    }
}