using iRacingPDFLoader.Entity;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace iRacingBusinessLayer.Models
{
    [Serializable]
    public class Serie : BaseModel
    {
        [JsonIgnore]
        public GroupSeries Parent { get; set; }
        public string Name { get; }
        public ObservableCollection<Week> Weeks { get; }

        private List<string> _cars;

        public List<string> Cars
        {
            get { return _cars; }
            set { _cars = value; RaisePropertyChanged(); }
        }


        private List<string> _carsload;

        public List<string> CarsLoad
        {
            get { return _carsload; }
        }

        public List<string> PossibleCars { get; set; }
        private bool _bookmarks;

        public bool Bookmarks
        {
            get { return _bookmarks; }
            set {
                _bookmarks = value;
                if (Parent != null) Parent.UpdateBookMarksFromSeries();
                RaisePropertyChanged();
            }
        }

        public Serie(GroupSeries parent, iRacingSerie iSerie)
        {
            Name = iSerie.Name;
            Parent = parent;
            _carsload = iSerie.Cars;
            Weeks = new ObservableCollection<Week>(iSerie.Weeks.Select(x => new Week(this, x)));
            PossibleCars = new List<string>();
        }

        public Serie(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            _carsload = (List<string>)info.GetValue("_carsload", typeof(List<string>));
            Weeks = (ObservableCollection<Week>)info.GetValue("Weeks", typeof(ObservableCollection<Week>));
            PossibleCars = (List<string>)info.GetValue("PossibleCars", typeof(List<string>));
            _bookmarks = info.GetBoolean("Bookmarks");
        }

        public void KinWeeks()
        {
            foreach (var week in Weeks)
            {
                week.Parent = this;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("_carsload", _carsload);
            info.AddValue("Weeks", Weeks);
            info.AddValue("PossibleCars", PossibleCars);
            info.AddValue("Bookmarks", Bookmarks);
        }

        [JsonIgnore]
        public bool AreAllTheCircuitsAndCarsInTheSeriesFilled
        {
            get
            {
                bool isThereAnEmptyCar = (Cars == null || Cars.Count == 0);
                bool isThereAnEmptyCircuit = Weeks.Select(w => w.Track == null || w.Track == "").FirstOrDefault(t => t == true, false);

                return !(isThereAnEmptyCircuit || isThereAnEmptyCar);
            }
        }

        public void UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled()
        {
            RaisePropertyChanged("AreAllTheCircuitsAndCarsInTheSeriesFilled");
        }
    }
}
