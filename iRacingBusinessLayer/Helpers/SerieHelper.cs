using iRacingBusinessLayer.Models;

namespace iRacingBusinessLayer.Helpers
{
    public class SerieHelper
    {
        /*
        public static Tuple<List<string>, List<DateTime>, Week[,]> SeriesToWeeksTable(List<Serie> series)
        {
            IEnumerable<IGrouping<DateTime, Week>> weeksgroup = series.SelectMany(s => s.Weeks).GroupBy(w => w.Date).OrderBy(k => k.Key);

            List<string> headerNameSerieCol = series.Select(x => x.Name).ToList();
            List<DateTime> headerDateWeekRow = weeksgroup.Select(wg => wg.Key).ToList();
            Week[,] weeksBuffer = new Week[headerDateWeekRow.Count, headerNameSerieCol.Count];

            foreach (IGrouping<DateTime, Week> wg in weeksgroup)
            {
                int indexdate = headerDateWeekRow.IndexOf(wg.Key);
                foreach (Week week in wg)
                {
                    int indexSerie = headerNameSerieCol.IndexOf(week.Parent.Name);
                    weeksBuffer[indexdate, indexSerie] = week;
                }
            }

            return Tuple.Create(headerNameSerieCol, headerDateWeekRow, weeksBuffer);
        }
        */

        public static Tuple<List<string>, List<DateTime>, Week[,]> SeriesToWeeksTableOrderRowsDate(List<Serie> series)
        {
            IEnumerable<IGrouping<DateTime, Week>> weeksgroup = series.SelectMany(s => s.Weeks).GroupBy(w => w.Date).OrderBy(k => k.Key);
            List<DateTime> headerDateWeekRow = new List<DateTime>();

            GridTable<Week> gridTable = new GridTable<Week>();
        
            foreach (IGrouping<DateTime, Week> wg in weeksgroup)
            {
                headerDateWeekRow.Add(wg.Key);
                foreach (Week week in wg)
                {
                    string nameSerie = week.Parent.Name;
                    gridTable.WriteField(nameSerie, week);
                }
                gridTable.NextRecord();
            }

            gridTable.Flush();

            return Tuple.Create(gridTable.Headers, headerDateWeekRow, gridTable.Table);
        }

        public static List<List<string>> SeriesToTableListOfListString(List<Serie> series)
        {
            GridTable<string> gridTable = new GridTable<string>();
            gridTable.Headers = new List<string>
            {
                "Discipline",
                "GroupLicense",
                "SerieName",
                "Cars",
                "CarsLoad",
                "WeekNumber",
                "WeekDate",
                "Track",
                "TrackLoad",
                "Duration"
            };

            foreach (Serie serie in series)
            {
                foreach (Week week in serie.Weeks)
                {
                    gridTable.WriteField("Discipline", serie.Parent.Parent.Name);
                    gridTable.WriteField("GroupLicense", serie.Parent.Name);
                    gridTable.WriteField("SerieName", serie.Name);
                    if(serie.Cars != null && serie.Cars.Count > 0)
                    {
                        gridTable.WriteField("Cars", string.Join(';', serie.Cars));
                    }
                    else
                    {
                        gridTable.WriteField("Cars", "");
                    }
                    gridTable.WriteField("CarsLoad", string.Join(';', serie.CarsLoad));
                    gridTable.WriteField("WeekNumber", week.Number.ToString());
                    gridTable.WriteField("WeekDate", week.Date.ToString("dd'-'MM'-'yyyy"));
                    gridTable.WriteField("Track", week.Track);
                    gridTable.WriteField("TrackLoad", week.TrackLoad);
                    gridTable.WriteField("Duration", week.Duration);

                    gridTable.NextRecord();
                }
            }

            gridTable.Flush();

            List<List<string>> tableList = new List<List<string>>(gridTable.NumRows + 1) { gridTable.Headers };

            string[,] table = gridTable.Table;
            int numrows = gridTable.NumRows;
            int numcols = gridTable.NumCols;
            for (int i = 0; i < numrows; i++)
            {
                List<string> colsList = new List<string>();
                for (int j = 0; j < numcols; j++)
                {
                    colsList.Add(table[i,j]);
                }
                tableList.Add(colsList);
            }

            return tableList;
        }
    }
}
