using iRacingPDFLoader.PDFEntry;
using iRacingPDFLoader.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace iRacingPDFLoader
{
    internal class PDFLoader
    {
        // Read the entire PDF and build a structure to unlink it from the PdfPig package
        internal static List<PagePDF> LoadPagesPDF(string pathPDF)
        {
            if(!File.Exists(pathPDF)) throw new ScheduleLoaderException(string.Format(ScheduleLoaderException.MSG_NOTFOUND_PDF, pathPDF));

            List<PagePDF> pagePDFList = new List<PagePDF>();

            using (PdfDocument document = PdfDocument.Open(pathPDF))
            {
                IEnumerable<Page> pages;
                try
                {
                     pages = document.GetPages();
                }
                catch (ObjectDisposedException e)
                {
                    throw new ScheduleLoaderException(e.Message);
                }

                foreach (var page in pages)
                {
                    var wordsList = page.GetWords().GroupBy(x => x.BoundingBox.Bottom);
                    var linesPDF = new List<LinePDF>();
                    foreach (var line in wordsList)
                    {
                        List<WordPDF> words = new List<WordPDF>();
                        foreach (var word in line)
                        {
                            words.Add(new WordPDF(word));
                        }

                        linesPDF.Add(new LinePDF(words));
                    }
                    pagePDFList.Add(new PagePDF(page.Number, linesPDF));
                }
            }

            return pagePDFList;
        }
    }
}
