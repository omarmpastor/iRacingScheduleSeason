using iRacingPDFLoader.Entity;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace iRacingBusinessLayer.Models
{
    [Serializable]
    public class Discipline : BaseModel
    {
        public string Name { get; }
        public ObservableCollection<GroupSeries> SeriesByLicense { get; }

        public bool? _bookmarksSeries;
        public bool? BookmarksSeries
        {
            get { return _bookmarksSeries; }
            set 
            {
                //SetBookmarsAllSeries(value == true); RaisePropertyChanged(); 
                if (value != null) SetBookmarsAllSeries(value == true);

                RaisePropertyChanged();
            }
        }

        [JsonIgnore]
        public bool AreAllTheCircuitsAndCarsInTheSeriesFilled
        {
            get
            {
                return SeriesByLicense
                    .Select(x => x.AreAllTheCircuitsAndCarsInTheSeriesFilled)
                    .FirstOrDefault(t => t == false, true);
            }
        }


        public Discipline(iRacingDiscipline iDiscipline)
        {
            Name = iDiscipline.Name;
            SeriesByLicense = new ObservableCollection<GroupSeries>(iDiscipline.SeriesByLicense.Select(x => new GroupSeries(this, x)));
            _bookmarksSeries = false;
        }

        protected Discipline(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            SeriesByLicense = (ObservableCollection<GroupSeries>)info.GetValue("SeriesByLicense", typeof(ObservableCollection<GroupSeries>));
            _bookmarksSeries = false;
        }

        public Discipline(string name, List<GroupSeries> seriesByLicense)
        {
            Name = name;
            SeriesByLicense = new ObservableCollection<GroupSeries>(seriesByLicense);
        }

        public List<Serie> GetAllSeries()
        {
            List<Serie> series = new List<Serie>();
            foreach (var gs in SeriesByLicense)
            {
                series.AddRange(gs.Series);
            }

            return series;
        }

        public void SetBookmarsAllSeries(bool marks)
        {
            foreach (var gs in SeriesByLicense)
            {
                gs.SetBookmarsAllSeries(marks);
                gs.BookmarksSeries = marks;
            }
        }
        
        public void UpdateBookMarksFromSeries()
        {
            int totalSeries = 0;
            int totalBookmarks = 0;
            foreach (GroupSeries gs in SeriesByLicense)
            {
                foreach (Serie serie in gs.Series)
                {
                    totalSeries++;
                    totalBookmarks += serie.Bookmarks ? 1 : 0;
                }
            }

            if (totalBookmarks == totalSeries) _bookmarksSeries = true; 
            else if(totalBookmarks == 0) _bookmarksSeries = false;
            else _bookmarksSeries = null;

            RaisePropertyChanged("BookmarksSeries");
        }

        public void UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled()
        {
            RaisePropertyChanged("AreAllTheCircuitsAndCarsInTheSeriesFilled");
        }

        public void KinSeriesByLicense()
        {
            foreach (var groupSeries in SeriesByLicense)
            {
                groupSeries.Parent = this;
                groupSeries.KinSeries();
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("SeriesByLicense", SeriesByLicense);
        }
    }
}
