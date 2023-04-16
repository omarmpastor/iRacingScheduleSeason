using System.Collections.Generic;

namespace iRacingPDFLoader.Entity
{
    public class iRacingGroupSeries
    {
        public string Name { get; }
        public List<iRacingSerie> Series { get; }

        public iRacingGroupSeries(string name)
        {
            Name = name;
            Series = new List<iRacingSerie>();
        }

        public void AddSerie(iRacingSerie serie)
        {
            Series.Add(serie);
        }
    }
}
