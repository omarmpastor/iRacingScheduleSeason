using System.Collections.Generic;
using System.Linq;

namespace iRacingPDFLoader.Entity
{
    public class iRacingSchedule
    {
        public List<iRacingDiscipline> Disciplines { get; set; }

        public iRacingSchedule()
        {
            Disciplines = new List<iRacingDiscipline>();
        }

        public iRacingDiscipline GetDisciplineByName(string name)
        {
            return Disciplines.First(x => x.Name == name);
        }
    }
}
