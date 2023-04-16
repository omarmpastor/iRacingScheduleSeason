using iRacingBusinessLayer.Models;
using iRacingSchedule.Base;
using iRacingSchedule.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace iRacingSchedule.ViewModel
{
    public class ScheduleWindowViewModel : BaseViewModel
    {
        private List<Serie> _series;

        public List<Serie> Series
        {
            get { return _series; }
        }

        private List<IRacingContent> _tracks;

        public List<IRacingContent> Tracks
        {
            get { return _tracks; }
        }


        private bool _isRepeatedTracksCheck;

        public bool IsRepeatedTracksCheck
        {
            get { return _isRepeatedTracksCheck; }
            set { _isRepeatedTracksCheck = value; RaisePropertyChanged(); }
        }

        public bool IsAllTracksConfirmed
        {
            get 
            {
                var firstEmptyTrack = _series.SelectMany(s => s.Weeks)
                    .FirstOrDefault(w => w.Track == null || w.Track == "");

                return firstEmptyTrack == null;
            }
        }

        public string TextCheckBox
        {
            get
            {
                string txt = "Marcar circuitos (de pago) repetidos";

                if(!IsAllTracksConfirmed)
                {
                    txt += " (Deshabilitado) ya que no has confirmado todos los circuitos en la ventana anterior";
                }

                return txt;
            }
        }

        public Brush ColorCheckBox
        {
            get
            {
                if (!IsAllTracksConfirmed)
                    return Brushes.Red;
                
                return Brushes.Black;
            }
        }

        public ScheduleWindowViewModel() 
        {
            _tracks = DependencyService.Get<DataLoadWriteService>().Tracks;

            var schedule = DependencyService.Get<DataLoadWriteService>()
                .Schedule.GetCheckedSchedule();

            schedule.SettracksRepeatInWeeks(_tracks);
            _series = schedule.GetAllSeries();
            RaisePropertyChanged("Tracks");
        }

        public async Task ExportSeries(string filename)
        {
            string ext = Path.GetExtension(filename);
            if (ext == ".csv")
            {
                await DependencyService.Get<DataLoadWriteService>().ExportScheduleBookmarksToCsv(filename);
            }
            else if(ext == ".xlsx")
            {
                await DependencyService.Get<DataLoadWriteService>().ExportScheduleBookmarksToExcelAsync(filename, IsRepeatedTracksCheck);
            }
            else if (ext == ".json")
            {
                await DependencyService.Get<DataLoadWriteService>().ExportJsonScheduleBookmarksAsync(filename);
            }
        }
        
    }
}
