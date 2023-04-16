using iRacingPDFLoader.Entity;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace iRacingBusinessLayer.Models
{
    [Serializable]
    public class GroupSeries : BaseModel
    {
        [JsonIgnore]
        public Discipline Parent { get; set; }
        public string Name { get; }
        public ObservableCollection<Serie> Series { get; set; }
        private bool? _bookmarksSeries;
        public bool? BookmarksSeries
        {
            get { return _bookmarksSeries; }
            set 
            {
                if (value != null) SetBookmarsAllSeries(value == true);
                //if (Parent != null) Parent.UpdateBookmarks();
                RaisePropertyChanged();
            }
        }
        public GroupSeries(Discipline parent, iRacingGroupSeries iGroupSeries)
        {
            Name = iGroupSeries.Name;
            Parent = parent;
            Series = new ObservableCollection<Serie>(iGroupSeries.Series.Select(x => new Serie(this, x)));
            _bookmarksSeries = false;
        }

        protected GroupSeries(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Series = (ObservableCollection<Serie>)info.GetValue("Series", typeof(ObservableCollection<Serie>));
            _bookmarksSeries = false;
        }

        public GroupSeries(string name, List<Serie> series)
        {
            Name = name;
            Series = new ObservableCollection<Serie>(series);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Series", Series);
        }

        public void UpdateBookMarksFromSeries()
        {
            int totalBookmarks = Series.Select(x => x.Bookmarks ? 1 : 0).Sum();

            if (totalBookmarks == 0) _bookmarksSeries = false;
            else if (totalBookmarks == Series.Count) _bookmarksSeries = true;
            else _bookmarksSeries = null;

            if (Parent != null) Parent.UpdateBookMarksFromSeries();

            RaisePropertyChanged("BookmarksSeries");
        }

        public void SetBookmarsAllSeries(bool marks)
        {
            foreach (var s in Series)
            {
                s.Bookmarks = marks;
            }
            _bookmarksSeries = marks;
        }

        [JsonIgnore]
        public bool AreAllTheCircuitsAndCarsInTheSeriesFilled
        {
            get
            {
                bool isThereAnEmptyCar = Series.Select(s => s.Cars == null || s.Cars.Count == 0).FirstOrDefault(t => t == true, false);
                bool isThereAnEmptyCircuit = Series.SelectMany(s => s.Weeks.Select(w => w.Track == null || w.Track == "")).FirstOrDefault(t => t == true, false);

                return !(isThereAnEmptyCircuit || isThereAnEmptyCar);
            }
        }

        public void UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled()
        {
            RaisePropertyChanged("AreAllTheCircuitsAndCarsInTheSeriesFilled");
        }

        public void KinSeries()
        {
            foreach (var serie in Series)
            {
                serie.Parent = this;
                serie.KinWeeks();
            }
        }
    }
}
