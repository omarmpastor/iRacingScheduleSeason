using System.Collections.Generic;

namespace iRacingPDFLoader.PDFEntry
{
    internal class PagePDF
    {
        public int Number { get; }
        public List<LinePDF> Lines { get; }

        public PagePDF(int number, List<LinePDF> lines)
        {
            Number = number;
            Lines = lines;
        }

        public List<LinePDF> GetLinesMatchFromPattern(string pattern)
        {
            return Lines.FindAll(x => x.MatchFromPattern(pattern));
        }

        public bool MatchLineFromPattern(string pattern)
        {
            return Lines.Exists(x => x.MatchFromPattern(pattern));
        }

        public bool MatchLineEqual(string text)
        {
            return Lines.Exists(x => x.Text == text);
        }
    }
}
