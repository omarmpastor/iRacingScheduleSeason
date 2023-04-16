using iRacingBusinessLayer;
using iRacingBusinessLayer.Models;
using iRacingSchedule.Services.Interfaces;
using System.Collections.Generic;

namespace iRacingSchedule.Services
{
    public class DataConverterService : DataConverterInterface
    {
        public void FillLikelyCarsAndTracks(
            Schedule schedule, List<string> carsOriginalList, List<string> tracksOriginalList)
        {
           iParser.FillLikelyCarsAndTracks(schedule, carsOriginalList, tracksOriginalList);
        }

        public void ConfirmAllCircuitsAndCarsThatOnlyHaveOneOption(Schedule schedule)
        {
            var series = schedule.GetAllSeries();

            foreach (var serie in series) 
            {
                if(serie.CarsLoad.Count == serie.PossibleCars.Count)
                {
                    serie.Cars = new List<string>(serie.CarsLoad);
                }

                foreach (var week in serie.Weeks)
                {
                    if(week.PossibleTracks.Count == 1)
                    {
                        week.Track = week.PossibleTracks[0];
                    }
                }
            }
        }
    }
}
