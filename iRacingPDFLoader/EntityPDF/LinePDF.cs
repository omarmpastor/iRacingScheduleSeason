using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace iRacingPDFLoader.PDFEntry
{
    internal class LinePDF
    {
        public string Text
        {
            get
            {
                string text = string.Empty;
                foreach (var word in Words)
                {
                    text += word.Text + " ";
                }
                return text.Trim();
            }
        }

        public List<WordPDF> Words { get; }

        public LinePDF(List<WordPDF> words)
        {
            Words = words;
        }


        public bool MatchFromPattern(string pattern)
        {
            return (Regex.Match(Text, pattern).Success);
        }

        public bool MatchTextEqual(string text)
        {
            return Text == text;
        }

        public double GetLeftBoundingBox()
        {
            if (Words.Count < 1) return 0;
            return Words[0].BoundingBox.Left;
        }

        public double GetRightBoundingBox()
        {
            if (Words.Count < 1) return 0;
            return Words[0].BoundingBox.Right;
        }

        public override string ToString()
        {
            return Text;
        }

        public static LinePDF ConcatLines(List<LinePDF> lines)
        {
            List<WordPDF> words = new List<WordPDF>();
            foreach (LinePDF line in lines)
            {
                words.AddRange(line.Words);
            }

            return new LinePDF(words);
        }
    }
}
