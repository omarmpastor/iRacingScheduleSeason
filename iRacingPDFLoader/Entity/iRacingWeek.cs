using System;

namespace iRacingPDFLoader.Entity
{
    public class iRacingWeek
    {
        public int Number { get; }
        public DateTime Date { get; set; }
        public string Track { get; set; }
        public string Duration { get; set; }

        public iRacingWeek(int number)
        {
            Number = number;
        }
    }
}
