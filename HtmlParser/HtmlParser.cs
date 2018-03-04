using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CsQuery;

namespace HtmlParser
{
    /// <summary>
    /// Represents a parser of html file.
    /// </summary>
    public static class HtmlParser
    {
        /// <summary>
        /// A word in parsed html file.
        /// </summary>
        public struct Word
        {
            public int Index;
            public Point LeftUp;
            public Point RightBottom;
            public string Value;
        }

        /// <summary>
        /// Gets an enumerable of <see cref="Word"/> from html file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>An enumerable of <see cref="Word"/>.</returns>
        public static IEnumerable<Word> Parse(string path)
        {
            var result = new List<Word>();
            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                var cq = CQ.Create(stream);
                int i = 0;
                result.AddRange(from word in cq.Find(".ocrx_word")
                    let coords = GetCoords(word.GetAttribute("title"))
                    select new Word
                    {
                        Index = i++,
                        LeftUp = new Point {X = coords[0], Y = coords[1]},
                        RightBottom = new Point {X = coords[2], Y = coords[3]},
                        Value = string.IsNullOrEmpty(word.InnerText) ? word.FirstChild.InnerText : word.InnerText
                    });

                return result;
            }
        }

        private static int[] GetCoords(string title)
        {
            var coords = new int[4];
            var splitted = title.Split(' ', ';');
            for (int i = 0; i < coords.Length; i++)
            {
                coords[i] = int.Parse(splitted[i + 1]);
            }

            return coords;
        }
    }
}
