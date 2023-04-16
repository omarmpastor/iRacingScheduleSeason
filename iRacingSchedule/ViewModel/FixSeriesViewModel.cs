using iRacingBusinessLayer.Models;
using iRacingSchedule.Base;
using iRacingSchedule.Services;
using System.Linq;

namespace iRacingSchedule.ViewModel
{
    public class FixSeriesViewModel : BaseViewModel
    {
        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; RaisePropertyChanged(); }
        }

        private BaseModel _selectedModel;

        public BaseModel SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; }
        }

        private bool _selectItemInListView;

        public bool SelectItemInListView
        {
            get { return _selectItemInListView; }
            set { _selectItemInListView = value; RaisePropertyChanged(); }
        }

        public bool _forceExport;
        public bool ForceExport 
        {
            get { return _forceExport; }
            set { _forceExport = value; RaisePropertyChanged();RaisePropertyChanged("AllDisciplinesFix"); }
        }

        public bool AllDisciplinesFix 
        { 
            get
            {
                if (ForceExport) return true;
                return _schedule.Disciplines
                    .FirstOrDefault(x => x.AreAllTheCircuitsAndCarsInTheSeriesFilled == false) == null;
            }
        }

        public FixSeriesViewModel()
        {
            _schedule = DependencyService.Get<DataLoadWriteService>().Schedule.GetCheckedSchedule();
            var cars = DependencyService.Get<DataLoadWriteService>().Cars;
            var tracks = DependencyService.Get<DataLoadWriteService>().Tracks;
            DependencyService.Get<DataConverterService>()
                .FillLikelyCarsAndTracks(_schedule, cars.Select(c => c.Name).ToList(), tracks.Select(t => t.Name).ToList());
        }

        public void UpdateSelectItemInListView(int countItemsSelected)
        {
            SelectItemInListView = countItemsSelected > 0;
        }

        public void UpdateAreAllTheCircuitsAndCarsInTheSeriesFilledProperties()
        {
            foreach (var d in _schedule.Disciplines)
            {
                d.UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled();
                foreach (var gs in d.SeriesByLicense)
                {
                    gs.UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled();
                    foreach (var s in gs.Series)
                    {
                        s.UpdateAreAllTheCircuitsAndCarsInTheSeriesFilled();
                    }
                }
            }

            RaisePropertyChanged("AllDisciplinesFix");
        }

        public void ConfirmAllCircuitsAndCarsThatOnlyHaveOneOption()
        {
            DependencyService.Get<DataConverterService>().ConfirmAllCircuitsAndCarsThatOnlyHaveOneOption(_schedule);
        }
    }
}
