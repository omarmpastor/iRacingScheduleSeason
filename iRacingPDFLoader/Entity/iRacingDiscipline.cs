using System.Collections.Generic;

namespace iRacingPDFLoader.Entity
{
    public class iRacingDiscipline
    {
        public string Name { get; }
        public List<iRacingGroupSeries> SeriesByLicense { get; }

        public iRacingDiscipline(string name) 
        { 
            Name = name;
            SeriesByLicense = new List<iRacingGroupSeries>();
        }

        public void AddSerieByLicense(iRacingGroupSeries iRacingGroup)
        {
            SeriesByLicense.Add(iRacingGroup);
        }
    }
}
