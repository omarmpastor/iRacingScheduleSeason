using iRacingSchedule.Base;
using iRacingSchedule.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iRacingSchedule.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private Frame _frameMainWindow;
        public  bool IsFixSeriesPageActive { get; private set; }
        public MainWindowViewModel(Frame frame) 
        {
            _frameMainWindow = frame;
        }

        public async Task ExportScheduleAsync(string filename, bool onlyCheckedSchedule = false)
        {
            string ext = Path.GetExtension(filename);
            if (ext == ".json")
            {
                if (IsFixSeriesPageActive || onlyCheckedSchedule)
                {
                    await DependencyService.Get<DataLoadWriteService>().ExportJsonScheduleBookmarksAsync(filename);
                }
                else
                {
                    await DependencyService.Get<DataLoadWriteService>().ExportJsonScheduleBookmarksAsync(filename);
                }
            }
            else if (ext == ".csv")
            {
                await DependencyService.Get<DataLoadWriteService>().ExportScheduleBookmarksToCsv(filename);
            }
            else if (ext == ".xlsx")
            {
                await DependencyService.Get<DataLoadWriteService>().ExportBookmarksToExcelAsync(filename);
            }
        }

        public async Task LoadScheduleFromPDFAsync(string pathfile)
        {
            string ext = Path.GetExtension(pathfile);
            if(ext == ".pdf")
            {
                await DependencyService.Get<DataLoadWriteService>().LoadScheduleFromPDFAsync(pathfile);
            }
        }

        public void UpdateIsFixSeriesPageActive(string namePage)
        {
            if (namePage == "FixSeries")
            {
                IsFixSeriesPageActive = true;
            }
            else
            {
                IsFixSeriesPageActive = false;
            }

            RaisePropertyChanged("IsFixSeriesPageActive");
        }

        public void Navigate<T>()
        {
            Page page = (Page)Activator.CreateInstance(typeof(T));
            _frameMainWindow.Navigate(page);
        }
    }
}
