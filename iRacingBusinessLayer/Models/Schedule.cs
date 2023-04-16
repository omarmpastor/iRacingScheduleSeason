using iRacingPDFLoader.Entity;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace iRacingBusinessLayer.Models
{
    [Serializable]
    public class Schedule : BaseModel
    {
        public ObservableCollection<Discipline> Disciplines { get; set; }
        public Schedule(iRacingSchedule iRacingSchedule)
        {
            Disciplines = new ObservableCollection<Discipline>(iRacingSchedule.Disciplines.Select(x => new Discipline(x)));
        }

        protected Schedule(SerializationInfo info, StreamingContext context)
        {
            Disciplines = (ObservableCollection<Discipline>)info.GetValue("Disciplines", typeof(ObservableCollection<Discipline>));
        }

        private Schedule(List<Discipline> disciplines)
        {
            Disciplines = new ObservableCollection<Discipline>(disciplines);
        }

        public List<Serie> GetAllSeries()
        {
            List<Serie> series = new List<Serie>();
            foreach (var discipline in Disciplines)
            {
                series.AddRange(discipline.GetAllSeries());
            }

            return series;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Disciplines", Disciplines);
        }

        public void KinDisciplines()
        {
            foreach (var discipline in Disciplines)
            {
                discipline.KinSeriesByLicense();
                discipline.UpdateBookMarksFromSeries();
                foreach (var gs in discipline.SeriesByLicense)
                {
                    gs.UpdateBookMarksFromSeries();
                }
            }

        }

        public Schedule GetCheckedSchedule()
        {
            IEnumerable<Serie> seriesBookmarked = Disciplines
                .SelectMany(d => d.SeriesByLicense)
                .SelectMany(gs => gs.Series)
                .Where(s => s.Bookmarks == true);

            IEnumerable<IGrouping<GroupSeries,Serie>> groupSeriesBookmarked = seriesBookmarked.GroupBy(s => s.Parent);

            var disciplinesBookmarked = groupSeriesBookmarked.GroupBy(g => g.Key.Parent);

            List<Discipline> disciplines = new List<Discipline>();
            foreach (var db in disciplinesBookmarked)
            {
                List<GroupSeries> gsList = new List<GroupSeries>();
                foreach (var gsb in db)
                {
                    gsList.Add(new GroupSeries(gsb.Key.Name, gsb.ToList()));
                }
                disciplines.Add(new Discipline(db.Key.Name, gsList));
            }

            var schedule = new Schedule(disciplines);
            schedule.KinDisciplines();

            return schedule;
        }

        public void SettracksRepeatInWeeks(List<IRacingContent> tracks)
        {
            IEnumerable<Week> allWeeks = this.Disciplines.SelectMany(d => d.SeriesByLicense.SelectMany(gs => gs.Series.SelectMany(s => s.Weeks)));

            Dictionary<string, int> tracksRepeat = new Dictionary<string, int>();

            foreach (IRacingContent track in tracks)
            {
                int rep = allWeeks.Count(w => w.Track == track.Name) - 1;
                if(rep < 0) { rep = 0; }
                tracksRepeat.Add(track.Name, rep);
            }

            foreach (Week week in allWeeks)
            {
                if(week.Track != null && week.Track.Length > 0)
                {
                    week.SetTrackRepeatInWeeks(tracksRepeat[week.Track]);
                }
            }
        }
    }
}
