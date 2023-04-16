using iRacingBusinessLayer.Models;

namespace iRacingBusinessLayer
{
    public class iParser
    {
        public static void FillLikelyCarsAndTracks(
            Schedule schedule, List<string> carsOriginalList, List<string> tracksOriginalList)
        {
            foreach (Discipline disc in schedule.Disciplines)
            {
                foreach (GroupSeries group in disc.SeriesByLicense)
                {
                    foreach (Serie serie in group.Series)
                    {
                        List<string> listCars = new List<string>();
                        foreach (string car in serie.CarsLoad)
                        {
                            string carFound = carsOriginalList.FirstOrDefault(x => x == car, string.Empty);
                            if (carFound != string.Empty)
                            {
                                listCars.Add(carFound);
                            }
                            else
                            {
                                listCars.AddRange(ResolveNames.GetLikelyCars(car, carsOriginalList));
                            }
                                
                        }

                        serie.PossibleCars = listCars;
                        //serie.PossibleCars.AddRange(listCars);

                        foreach (Week week in serie.Weeks)
                        {
                            week.PossibleTracks = ResolveNames.GetLikelyTracks(week.TrackLoad, tracksOriginalList);
                            //week.PossibleTracks.AddRange(ResolveNames.GetLikelyTracks(week.TrackLoad, tracksOriginalList));
                        }
                    }
                }
            }
        }

        public static Schedule GetSchedule(
            string pathPdf, List<string> carsOriginalList, List<string> tracksOriginalList)
        {
            var iSchedule = iRacingPDFLoader.ScheduleParser.LoadSchedule(pathPdf);
            Schedule schedule = new Schedule(iSchedule);
            FillLikelyCarsAndTracks(schedule, carsOriginalList, tracksOriginalList);

            return schedule;
        }

        public static Schedule GetSchedule(string pathPdf)
        {
            var iSchedule = iRacingPDFLoader.ScheduleParser.LoadSchedule(pathPdf);
            Schedule schedule = new Schedule(iSchedule);

            return schedule;
        }
    }
}
