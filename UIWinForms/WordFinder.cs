using System;
using System.Collections.Generic;
using System.Linq;

namespace UIWinForms
{
    /// <summary>
    /// Represents a finder of specified words.
    /// </summary>
    internal class WordFinder
    {
        private const string HtmlFilePath = @"zones.html";
        private readonly MainForm _form;
        private readonly IEnumerable<HtmlParser.HtmlParser.Word> _words;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordFinder"/> class.
        /// </summary>
        /// <param name="form">A form with image to highlight words.</param>
        public WordFinder(MainForm form)
        {
            _words = HtmlParser.HtmlParser.Parse(HtmlFilePath);
            _form = form ?? throw new ArgumentNullException(nameof(form));
        }
        
        /// <summary>
        /// Finds all occurrences of <paramref name="text"/> phrase.
        /// </summary>
        /// <param name="text">Target phrase to find.</param>
        public void FindEntries(string text)
        {
            var searchWords = text.Split(' ');
            var allWords = _words
                .Join(searchWords, word => word.Value, searchWord => searchWord, (word, searchWord) => word)
                .ToList();

            FilterWords(searchWords, allWords);
            foreach (var word in allWords)
            {
                _form.HighlightWord(word);
            }
        }

        private static void MakePair(
            HtmlParser.HtmlParser.Word prev,
            string nextValue,
            ICollection<HtmlParser.HtmlParser.Word> allWords)
        {
            var next = allWords.FirstOrDefault(word => word.Index == prev.Index + 1 && word.Value == nextValue);
            if (next.Equals(default(HtmlParser.HtmlParser.Word)))
            {
                allWords.Remove(prev);
                return;
            }

            var newWord = new HtmlParser.HtmlParser.Word
            {
                Index = next.Index,
                LeftUp = prev.LeftUp,
                RightBottom = next.RightBottom,
                Value = prev.Value + " " + next.Value
            };

            allWords.Remove(prev);
            allWords.Remove(next);
            allWords.Add(newWord);
        }

        private static void FilterWords(string[] searchWords, List<HtmlParser.HtmlParser.Word> allWords)
        {
            var prevName = String.Empty;
            var nextName = searchWords[0];

            for (int i = 0; i < searchWords.Length - 1; i++)
            {
                var i1 = i;
                var words = allWords.Where(word => word.Value.Contains(prevName) && word.Value.Contains(nextName)).ToList();
                var nextWordValue = searchWords[i1 + 1];
                foreach (var word in words)
                {
                    MakePair(word, nextWordValue, allWords);
                }

                allWords.RemoveAll(word => word.Value == nextWordValue);
                prevName = nextName;
                nextName = nextWordValue;
            }
        }
    }
}
