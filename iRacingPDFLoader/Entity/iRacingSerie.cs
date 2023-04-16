using System.Collections.Generic;

namespace iRacingPDFLoader.Entity
{
    public class iRacingSerie
    {
        public string Name { get; }
        public List<string> Cars { get; set; }
        public List<iRacingWeek> Weeks;

        public iRacingSerie(string name)
        {
            Name = name;
            Cars = new List<string>();
            Weeks = new List<iRacingWeek>();
        }
    }
}
