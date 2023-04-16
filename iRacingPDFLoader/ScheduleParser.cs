using iRacingPDFLoader.Entity;
using iRacingPDFLoader.PDFEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace iRacingPDFLoader
{
    public class ScheduleParser
    {
        private static readonly string PATTERN_NUM_WEEK_LINE = @"^Week (?<week>\d+) \((?<date>(\d{4}-\d{2}-\d{2}))\)";
        private static readonly string PATTERN_CIRCUIT_LINE = @"(?<circuit>.*) \(\d{4}";

        private static string NormalizeString(string text)
        {
            Regex reg = new Regex("[^a-zA-Z0-9\\-\\(\\)\\[\\] ]");
            return reg.Replace(text.Normalize(NormalizationForm.FormD), "");
        }

        public static iRacingSchedule LoadSchedule(string pathPDF)
        {
            var entries = PDFParser.GetEntriesFromPages(pathPDF);
            return EntriesToSchedule(entries);
        }

        private static iRacingSerie FillSerie(EntryPDF entry)
        {
            iRacingSerie serie = new iRacingSerie(entry.Title);
            string[] cars = entry.HeaderSerie.ElementAt(1).Text.Split(',');
            foreach (var car in cars)
            {
                serie.Cars.Add(car.Trim());
            }

            foreach (var week in entry.WeeksSerie)
            {
                var firstLine = Regex.Match(week.ElementAt(0).Text, PATTERN_NUM_WEEK_LINE);
                int numWeek = Convert.ToInt32(firstLine.Groups["week"].Value);

                iRacingWeek newWeek = new iRacingWeek(numWeek);

                newWeek.Date = DateTime.Parse(firstLine.Groups["date"].Value);

                var secondLine = Regex.Match(week.ElementAt(1).Text, PATTERN_CIRCUIT_LINE);
                newWeek.Track = NormalizeString(secondLine.Groups["circuit"].Value);

                newWeek.Duration = week.ElementAt(3).Text;

                serie.Weeks.Add(newWeek);
            }


            return serie;
        }

        private static iRacingSchedule EntriesToSchedule(List<EntryPDF> entries)
        {
            iRacingSchedule schedule = new iRacingSchedule();
            iRacingDiscipline currentDiscipline = null;
            iRacingGroupSeries currentGroupSeries = null;

            foreach (EntryPDF entry in entries)
            {
                switch (entry.Depth)
                {
                    case ((int)DepthOption.Discipline):
                        iRacingDiscipline discipline = new iRacingDiscipline(entry.Title);
                        schedule.Disciplines.Add(discipline);
                        currentDiscipline = discipline;
                        break;
                    case ((int)DepthOption.GroupSeries):
                        iRacingGroupSeries groupSeries = new iRacingGroupSeries(entry.Title);
                        currentDiscipline.AddSerieByLicense(groupSeries);
                        currentGroupSeries = groupSeries;
                        break;
                    case ((int)DepthOption.Serie):
                        currentGroupSeries.AddSerie(FillSerie(entry));
                        break;
                }
            }

            return schedule;
        }
    }
}
