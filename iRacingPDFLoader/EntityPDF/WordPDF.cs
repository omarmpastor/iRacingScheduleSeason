using UglyToad.PdfPig.Content;

namespace iRacingPDFLoader.PDFEntry
{
    internal class WordPDF
    {
        public string Text { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public WordPDF(Word word)
        {
            Text = word.Text;
            BoundingBox = new BoundingBox(
                word.BoundingBox.Left,
                word.BoundingBox.Top,
                word.BoundingBox.Right,
                word.BoundingBox.Bottom
                );
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
