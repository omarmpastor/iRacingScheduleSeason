using iRacingPDFLoader.PDFEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace iRacingPDFLoader
{
    public enum DepthOption
    {
        Discipline,
        GroupSeries,
        Serie
    }

    internal class PDFParser
    {
        private static readonly int MAX_SPACE_FOR_DETECT_COLUMNS = 3;
        private static readonly int MAX_RIGHT_POS_FOR_BOUNDING_BOX = 800;
        private static readonly string PATTERN_ENTRY_INDEX = @" \.";
        private static readonly string PATTERN_SPACE_AND_POINT = @"( \.)+";
        private static readonly string PATTERN_HEADER_SERIE_CLASS_LINE_AFTER_CARS = @"-->";
        private static readonly string PATTERN_START_LINE_WEEK = @"^Week ";

        private static List<LinePDF> FormatOneWeekSerie(List<LinePDF> weekLines)
        {
            List<LinePDF> newLines = new List<LinePDF>();
            LinePDF firstLine = weekLines.ElementAt(0);
            List<int> muchSpaceBetweenWords = new List<int>(); // Columns

            int firstColumnLeftPos = Convert.ToInt32(firstLine.Words.ElementAt(0).BoundingBox.Left);
            muchSpaceBetweenWords.Add(firstColumnLeftPos);

            // Get the position of the columns from the distance between words
            for (int i = 0; i < firstLine.Words.Count - 1; i++)
            {
                int rightCurrentWord = Convert.ToInt32(firstLine.Words.ElementAt(i).BoundingBox.Right);
                int leftNextWord = Convert.ToInt32(firstLine.Words.ElementAt(i + 1).BoundingBox.Left);
                if (MAX_SPACE_FOR_DETECT_COLUMNS < (leftNextWord - rightCurrentWord))
                {
                    muchSpaceBetweenWords.Add(rightCurrentWord);
                }
            }

            int lastColumnRightPos = Convert.ToInt32(MAX_RIGHT_POS_FOR_BOUNDING_BOX);
            muchSpaceBetweenWords.Add(lastColumnRightPos);

            // Fix errors in lines when reading them from columns
            // using the position of each column calculated by the line hyphenation approximation
            for (int i = 0; i < muchSpaceBetweenWords.Count - 1; i++)
            {
                List<WordPDF> wordsNewLine = new List<WordPDF>();
                foreach (var weekLine in weekLines)
                {
                    foreach (var word in weekLine.Words)
                    {
                        if (word.BoundingBox.Left >= muchSpaceBetweenWords.ElementAt(i)
                            && word.BoundingBox.Left < muchSpaceBetweenWords.ElementAt(i + 1))
                        {
                            wordsNewLine.Add(word);
                        }
                    }
                }
                newLines.Add(new LinePDF(wordsNewLine));
            }

            return newLines;
        }

        private static List<List<LinePDF>> FormatWeeksSerie(List<List<LinePDF>> weeksList)
        {
            List<List<LinePDF>> newWeeksList = new List<List<LinePDF>>();

            foreach (var linesWeek in weeksList)
            {
                newWeeksList.Add(FormatOneWeekSerie(linesWeek));
            }

            return newWeeksList;
        }

        private static List<LinePDF> FormatHeaderSerie(List<LinePDF> headerLines, string serieName)
        {
            List<LinePDF> newListLines = new List<LinePDF>();

            int classLine = -1;
            int counter = 0;
            bool found = false;
            // Get the index of the class line since it marks the end of the cars
            while (!found && counter < headerLines.Count)
            {
                if (headerLines[counter].MatchFromPattern(PATTERN_HEADER_SERIE_CLASS_LINE_AFTER_CARS)
                    || headerLines[counter].MatchTextEqual("Restricted to select members")) //)
                {
                    classLine = counter;
                    found = true;
                }
                counter++;
            }

            // Load Cars and
            List<LinePDF> tempCarsLines = new List<LinePDF>();
            for (int i = 1; i < classLine; i++)
            {
                tempCarsLines.Add(headerLines.ElementAt(i));
            }

            newListLines.Add(headerLines.ElementAt(0));
            newListLines.Add(LinePDF.ConcatLines(tempCarsLines));
            newListLines.Add(headerLines.ElementAt(classLine));

            // Add the following lines to the class 
            for (int i = classLine + 1; i < headerLines.Count; i++)
            {
                newListLines.Add(headerLines.ElementAt(i));
            }

            return newListLines;
        }

        private static void FillEntrySeriesFromLines(EntryPDF entry, List<LinePDF> lines)
        {
            int counter = 0;
            bool found = false;
            int indexStartSerie = -1;
            while (!found && counter < lines.Count)
            {
                if (lines.ElementAt(counter).MatchTextEqual(entry.Title))
                {
                    indexStartSerie = counter;
                    found = true;
                }

                counter++;
            }

            var linesEntry = lines.GetRange(indexStartSerie, lines.Count - indexStartSerie);

            List<int> numLinesStartWeek = new List<int>();
            List<string> linesTemp = new List<string>();
            // Search first line Week
            for (int i = 0; i < linesEntry.Count; i++)
            {
                if (linesEntry[i].MatchFromPattern(PATTERN_START_LINE_WEEK))
                {
                    numLinesStartWeek.Add(i);
                }
            }

            List<LinePDF> headerLines = new List<LinePDF>();
            List<List<LinePDF>> weeks = new List<List<LinePDF>>();

            if (numLinesStartWeek.Count > 0)
            {
                // Lines Header
                for (int i = 0; i < numLinesStartWeek[0]; i++)
                {
                    headerLines.Add(linesEntry[i]);
                }

                // Lines Week
                for (int i = 0; i < numLinesStartWeek.Count; i++)
                {
                    int startLineWeek = numLinesStartWeek[i];
                    int endLineWeek = (i < numLinesStartWeek.Count - 1) ? numLinesStartWeek[i + 1] : linesEntry.Count - 1;
                    List<LinePDF> weekLines = new List<LinePDF>();
                    for (int j = startLineWeek; j < endLineWeek; j++)
                    {
                        weekLines.Add(linesEntry[j]);
                    }
                    weeks.Add(weekLines);
                }
            }
            else
            {
                headerLines.AddRange(lines);
            }

            entry.HeaderSerie = FormatHeaderSerie(headerLines, entry.Title);
            entry.WeeksSerie = FormatWeeksSerie(weeks);
        }

        private static void FillEntriesFromPages(List<EntryPDF> entries, List<PagePDF> pages)
        {
            foreach (EntryPDF entry in entries)
            {
                List<PagePDF> pagesEntry = pages.GetRange(entry.Page, entry.NumPages);
                List<LinePDF> linesEntry = new List<LinePDF>();
                foreach (var p in pagesEntry)
                {
                    linesEntry.AddRange(p.Lines);
                }

                // Only Fill series
                //if (entry.Depth > 1)
                if (entry.Depth >= (int)DepthOption.Serie)
                {
                    FillEntrySeriesFromLines(entry, linesEntry);
                }
            }
        }

        // Read the lines of the index distinguishing them because they are a text followed by [space][point]( .) one or more times
        private static List<EntryPDF> BuildTreeEntries(List<PagePDF> pages)
        {
            List<EntryPDF> entries = new List<EntryPDF>();
            List<LinePDF> linesIndex = new List<LinePDF>();
            List<int> allMarginLeftList = new List<int>();

            // Load only lines of index
            foreach (var page in pages)
            {
                linesIndex.AddRange(page.GetLinesMatchFromPattern(PATTERN_ENTRY_INDEX));
                // Get left margin of 1st word
                foreach (var line in linesIndex)
                {
                    int margin = Convert.ToInt32(line.GetLeftBoundingBox());
                    allMarginLeftList.Add(margin);
                }
            }

            int[] allMarginLeft = allMarginLeftList.Distinct().ToArray();
            Array.Sort(allMarginLeft);

            // Add entry and pos of indented line
            foreach (var line in linesIndex)
            {
                int leftLineBoundingBox = Convert.ToInt32(line.GetLeftBoundingBox());
                int indexMargin = Array.FindIndex(allMarginLeft, x => x == leftLineBoundingBox);
                string text = line.Text;
                int posPoints = text.IndexOf(Regex.Match(text, PATTERN_SPACE_AND_POINT).Value);
                EntryPDF entryPDF = new EntryPDF(text.Substring(0, posPoints).Trim(), indexMargin);
                entries.Add(entryPDF);
            }

            // Search serie title in all document for know page
            foreach (var entry in entries)
            {
                int i = 0;
                bool found = false;
                while (!found && i < pages.Count)
                {
                    if (pages[i].MatchLineEqual(entry.Title))
                    {
                        found = true;
                        entry.Page = i;
                    }
                    i++;
                }
            }

            // Calculate the number of pages of the Serie
            int lastPage = pages.Count;
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                entries[i].NumPages = lastPage - entries[i].Page;
                lastPage = entries[i].Page;
                // If there is more than one entry on a page - Fix
                if (entries[i].Depth >= ((int)DepthOption.Serie) && entries[i].NumPages == 0)
                {
                    entries[i].NumPages = 1;
                }
            }

            return entries;
        }

        // Receives the parsed pdf pages converts everything to inputs to be able to create a structure from here
        public static List<EntryPDF> GetEntriesFromPages(string pathPDF)
        {
            List<PagePDF> pages = PDFLoader.LoadPagesPDF(pathPDF);
            List<EntryPDF> entries = BuildTreeEntries(pages);
            FillEntriesFromPages(entries, pages);

            return entries;
        }
    }
}
