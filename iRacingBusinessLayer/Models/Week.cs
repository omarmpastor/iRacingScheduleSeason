using iRacingPDFLoader.Entity;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace iRacingBusinessLayer.Models
{
    [Serializable]
    public class Week : BaseModel
    {
        [JsonIgnore]
        public Serie Parent { get; set; }
        public int Number { get; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
        private string _track;

        public string Track
        {
            get { return _track; }
            set { _track = value; RaisePropertyChanged(); }
        }


        private string _trackLoad;

        public string TrackLoad
        {
            get { return _trackLoad; }
        }
        public List<string> PossibleTracks { get; set; }
        public Week(Serie parent, iRacingWeek iWeek)
        {
            Number = iWeek.Number;
            Parent = parent;
            Date = iWeek.Date;
            _trackLoad = iWeek.Track;
            Duration = iWeek.Duration;
            PossibleTracks = new List<string>();
        }

        public Week(SerializationInfo info, StreamingContext context)
        {
            Number = info.GetInt32("Number");
            Date = info.GetDateTime("Date");
            _trackLoad = info.GetString("_trackLoad");
            Duration = info.GetString("Duration");
            PossibleTracks = (List<string>)info.GetValue("PossibleTracks", typeof(List<string>));
        }

        public void ChangeTrackChecked(string track)
        {
            _trackLoad = track;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Number", Number);
            info.AddValue("Date", Date);
            info.AddValue("Duration", Duration);
            info.AddValue("_trackLoad", TrackLoad);
            info.AddValue("PossibleTracks", PossibleTracks);
        }
    }
}
