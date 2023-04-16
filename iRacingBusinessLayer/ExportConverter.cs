using iRacingBusinessLayer.Helpers;
using iRacingBusinessLayer.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace iRacingBusinessLayer
{
    public class ExportConverter
    {
        public static Excel.XlRgbColor COLOR_HIGHLIGHTS = Excel.XlRgbColor.rgbWhiteSmoke;//rgbLightSteelBlue;
        public static Excel.XlRgbColor COLOR_REPEATS_2_TIMES = Excel.XlRgbColor.rgbPaleGoldenrod;
        public static Excel.XlRgbColor COLOR_REPEATS_MORE_THAN_2_TIMES = Excel.XlRgbColor.rgbSandyBrown;
        public static Excel.XlRgbColor COLOR_FREE_TRACK = Excel.XlRgbColor.rgbTeal;//rgbCadetBlue;

        public static string SerializeJson<T>(T t)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(t, options);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static JsonDocument GetSerializeToJsonDocument<T>(T t)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            return JsonSerializer.SerializeToDocument(t, options);
        }

        public static List<string> SerializeToCSV<T>(T t)
        {
            List<List<string>> listOfList;

            if (t is List<Serie>)
            {
                listOfList = SerieHelper.SeriesToTableListOfListString(t as List<Serie>);
            }
            else
            {
                throw new ArgumentException("Tipo de datos no soportado");
            }

            List<string> linesCsv = new List<string>();
            listOfList.ForEach(colsList => {
                linesCsv.Add(string.Join(',', colsList));
            });

            return linesCsv;
        }

        private static void FormatExcelCells(Excel.Range cells, bool highlights, bool isHeader)
        {
            Excel.Borders border = cells.Borders;
            border.LineStyle = Excel.XlLineStyle.xlContinuous;
            border.Weight = 2d;

            if(highlights)
            {
                cells.Interior.Color = COLOR_HIGHLIGHTS;
            }

            if(isHeader)
            {
                cells.Style.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                cells.Font.Bold = true;
            }
        }

        public static void SerializeScheduleExcel(List<Serie> series, string filename, bool isRepeatedTracksCheck, List<IRacingContent> tracks)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Add();
            Excel.Worksheet ws = wb.Worksheets[1];

            var tuple = SerieHelper.SeriesToWeeksTableOrderRowsDate(series);
            var headers = tuple.Item1;
            var dates = tuple.Item2;
            var weeksMatrix = tuple.Item3;

            List<string> tracksRepeats2Times;
            List<string> tracksRepeatsMoreThan2Times;
            List<string> tracksFree = tracks.Where(c => c.IsFree).Select(c => c.Name).ToList();

            if(isRepeatedTracksCheck)
            {
                tracksRepeats2Times = series.SelectMany(s => s.Weeks)
                    .GroupBy(w => w.Track)
                    .Where(t => t.Count() == 2)
                    .SelectMany(g => g.Select(w => w.Track))
                    .Distinct()
                    .ToList();

                tracksRepeatsMoreThan2Times = series.SelectMany(s => s.Weeks)
                    .GroupBy(w => w.Track)
                    .Where(t => t.Count() > 2)
                    .SelectMany(g => g.Select(w => w.Track))
                    .Distinct()
                    .ToList();
            }
            else
            {
                tracksRepeats2Times = new List<string>();
                tracksRepeatsMoreThan2Times = new List<string>();
            }

            // Cell 1,1
            Excel.Range firstCell = ws.UsedRange;
            FormatExcelCells(firstCell.Cells[1, 1], true, true);

            // Write serie names
            for (int i = 0; i < headers.Count; i++)
            {
                ws.Cells[1, i + 2] = headers.ElementAt(i);
                Excel.Range range = ws.UsedRange;
                FormatExcelCells(range.Cells[1, i + 2], true, true);
            }

            // Write dates
            for (int i = 0; i < dates.Count; i++)
            {
                ws.Cells[i + 2, 1] = dates.ElementAt(i);
                bool highlights = i % 2 == 0;
                Excel.Range range = ws.UsedRange;
                FormatExcelCells(range.Cells[i + 2, 1], !highlights, true);
            }

            // Write tracks
            int numrows = dates.Count;
            int numcols = headers.Count;
            for (int i = 0; i < numrows; i++)
            {
                for (int j = 0; j < numcols; j++)
                {
                    ws.Cells[i + 2, j + 2] = weeksMatrix[i, j].TrackLoad;// + " (" + weeksMatrix[i, j].Duration + ")";

                    bool highlights = i % 2 == 0;
                    Excel.Range range = ws.UsedRange;
                    Excel.Range cell = range.Cells[i + 2, j + 2];
                    FormatExcelCells(cell, !highlights, false);
                    if(isRepeatedTracksCheck && weeksMatrix[i, j].Track != null && weeksMatrix[i, j].Track != "")
                    {
                        if (!tracksFree.Contains(weeksMatrix[i, j].Track))
                        {

                            if (tracksRepeats2Times.Contains(weeksMatrix[i, j].Track))
                            {
                                cell.Interior.Color = COLOR_REPEATS_2_TIMES;
                            }
                            else if (tracksRepeatsMoreThan2Times.Contains(weeksMatrix[i, j].Track))
                            {
                                cell.Interior.Color = COLOR_REPEATS_MORE_THAN_2_TIMES;
                            }
                        }
                        /*
                        if(tracksFree.Contains(weeksMatrix[i, j].Track))
                        {
                            cell.Offset.Font.Color = COLOR_FREE_TRACK;
                        }
                        */
                    }
                }
            }

            if (isRepeatedTracksCheck)
            {
                // Write Info
                ws.Cells[numrows + 4, 1] = "Repetido";
                ws.Cells[numrows + 5, 1] = "Repetido varias veces";
                //ws.Cells[numrows + 6, 1] = "Gratuito";
                Excel.Range range2 = ws.UsedRange;
                range2.Cells[numrows + 4, 1].Interior.Color = COLOR_REPEATS_2_TIMES;
                range2.Cells[numrows + 5, 1].Interior.Color = COLOR_REPEATS_MORE_THAN_2_TIMES;
                //range2.Cells[numrows + 6, 1].Font.Color = COLOR_FREE_TRACK;
            }

            List<string> repeatTracks = series.SelectMany(s => s.Weeks)
                    .GroupBy(w => w.Track)
                    .Where(t => t.Count() > 1)
                    .SelectMany(g => g.Select(w => w.Track))
                    .Distinct()
                    .ToList();

            List<string> repeatFree = tracks.Where(c => repeatTracks.Contains(c.Name) && c.IsFree).Select(x => x.Name).ToList();
            ws.Cells[numrows + 4, 3] = "Circuitos contenido base repetidos";
            FormatExcelCells(ws.Cells[numrows + 4, 3], true, true);
            for (int i = 0; i < repeatFree.Count; i++)
            {
                ws.Cells[numrows + 5 + i, 3] = repeatFree.ElementAt(i);
                FormatExcelCells(ws.Cells[numrows + 5 + i, 3], false, false);
            }


            int rownum = numrows + 4;
            ws.Cells[rownum, 4] = "Circuitos de pago repetidos";
            FormatExcelCells(ws.Cells[rownum, 4], true, true);
            foreach (string track in tracksRepeats2Times)
            {
                if(!repeatFree.Contains(track))
                {
                    rownum++;
                    ws.Cells[rownum, 4] = track;
                    FormatExcelCells(ws.Cells[rownum, 4], false, false);
                    ws.Cells[rownum, 4].Interior.Color = COLOR_REPEATS_2_TIMES;
                }
            }

            foreach (string track in tracksRepeatsMoreThan2Times)
            {
                if (!repeatFree.Contains(track))
                {
                    rownum++;
                    ws.Cells[rownum, 4] = track;
                    FormatExcelCells(ws.Cells[rownum, 4], false, false);
                    ws.Cells[rownum, 4].Interior.Color = COLOR_REPEATS_MORE_THAN_2_TIMES;
                }
            }

            // Ajust cols
            for (int i = 1; i < numcols + 2; i++)
            {
                ws.Columns[i].AutoFit();
            }

            if(numcols < 3)
            {
                ws.Columns[3].AutoFit();
                ws.Columns[4].AutoFit();
            }
            
            excelApp.DisplayAlerts = false;
            wb.SaveAs(Filename: filename, FileFormat: Excel.XlFileFormat.xlWorkbookDefault);
            wb.Close();
        }

        public static void SerializeExcel(List<Serie> series, string filename)
        {
            List<List<string>> listOfList = SerieHelper.SeriesToTableListOfListString(series);

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook wb = excelApp.Workbooks.Add();
            Excel.Worksheet ws = wb.Worksheets[1];

            int counterRow = 1;
            int counterCol = 1;
            foreach (List<string> listCols in listOfList)
            {
                counterCol = 1;
                foreach (string col in listCols)
                {
                    ws.Cells[counterRow, counterCol] = col;
                    counterCol++;
                }
                counterRow++;
            }

            for (int i = 1; i < counterCol - 1; i++)
            {
                ws.Columns[i].AutoFit();
            }

            excelApp.DisplayAlerts = false;
            wb.SaveAs(Filename: filename, FileFormat: Excel.XlFileFormat.xlWorkbookDefault);
            wb.Close();
        }
    }
}
