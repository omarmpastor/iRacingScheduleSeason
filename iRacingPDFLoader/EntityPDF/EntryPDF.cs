using System.Collections.Generic;

namespace iRacingPDFLoader.PDFEntry
{
    internal class EntryPDF
    {
        public string Title { get; }
        public int Depth { get; }
        public int Page { get; set; }
        public int NumPages { get; set; }
        public List<LinePDF> HeaderSerie { get; set; }

        public List<List<LinePDF>> WeeksSerie { get; set; }

        public EntryPDF(string title, int depth)
        {
            Title = title;
            Depth = depth;
            //HeaderSerie = new List<LinePDF>();
        }
    }
}
