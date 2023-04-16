using iRacingBusinessLayer;
using iRacingBusinessLayer.Models;
using iRacingSchedule.Base;
using iRacingSchedule.Events;
using iRacingSchedule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iRacingSchedule.ViewModel
{
    public class SelectSeriesViewModel : BaseViewModel
    {
        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; RaisePropertyChanged(); }
        }

        public bool CheckedSerie
        {
            get 
            {
                if (_schedule == null) return false;
                var series = _schedule.GetAllSeries();
                return series.Exists(item => item.Bookmarks == true);
            }
            private set { }
        }

        private List<string> _cars;

        public List<string> Cars
        {
            get { return _cars; }
            set { _cars = value; RaisePropertyChanged(); }
        }

        private List<string> _tracks;

        public List<string> Tracks
        {
            get { return _tracks; }
            set { _tracks = value; RaisePropertyChanged(); }
        }

        public bool CheckConnection { get; private set; }

        public bool IsIRacingContentDownload { get; private set; }

        public SelectSeriesViewModel()
        {
            CheckConnection = true; // El primero para que no se vea el checkbox de no hay conexion mientras carga todo
            IsIRacingContentDownload = true;
            _schedule = DependencyService.Get<DataLoadWriteService>().Schedule;
            _cars = DependencyService.Get<DataLoadWriteService>().Cars.Select(x => x.Name).ToList();
            _tracks = DependencyService.Get<DataLoadWriteService>().Tracks.Select(x => x.Name).ToList();
            string proxyServer = string.Empty;
            string proxyUsername = string.Empty;
            string proxyPassword =  string.Empty;

            if (Properties.Settings.Default.ProxyEnabled)
            {
                proxyServer = Properties.Settings.Default.ProxyServer;
                if(Properties.Settings.Default.ProxyUser)
                {
                    proxyUsername = Properties.Settings.Default.ProxyUsername;
                    proxyPassword = Properties.Settings.Default.ProxyPassword;
                }
            }
            
            UpdateInternetConnection(proxyServer, proxyUsername, proxyPassword);
            IsIRacingContentDownload = (_tracks.Count > 0 && _cars.Count > 0);
            RaisePropertyChanged("IsIRacingContentDownload");
            DependencyService.Get<DataLoadWriteService>().ChangeInternetConfigurationEvent += OnChangeInternetConfiguration;
        }

        private void UpdateInternetConnection(string proxyAddress = "", string proxyUserName = "", string proxyPassword = "")
        {
            CheckConnection = iRacingLoadPage.CheckConnection(proxyAddress, proxyUserName, proxyPassword);
            RaisePropertyChanged("CheckConnection");
        }

        private void OnChangeInternetConfiguration(object? sender, EventArgs e)
        {
            ProxyEventArgs args = (ProxyEventArgs)e;
            
            string proxyServer = string.Empty;
            string proxyUsername = string.Empty;
            string proxyPassword = string.Empty;

            if (args.ProxyEnabled == true)
            {
                proxyServer = args.ProxyServer;
                if (args.ProxyUser == true)
                {
                    proxyUsername = args.ProxyUsername;
                    proxyPassword = args.ProxyPassword;
                }
            }

            UpdateInternetConnection(proxyServer, proxyUsername, proxyPassword);
        }

        public void UpdateCheckedSerie()
        {
            RaisePropertyChanged("CheckedSerie");
        }

        public void LoadScheduleFromPDF(string pathPDF)
        {
            Schedule = DependencyService.Get<DataLoadWriteService>().Schedule;
        }

        public bool IsChechedSchedule()
        {
            var series = _schedule.GetAllSeries();

            return series.Exists(x => x.Bookmarks);
        }

        public async Task DownloadIracingContentAsync()
        {
            await DependencyService.Get<DataLoadWriteService>().DownloadIracingContentAsync();
            Cars = DependencyService.Get<DataLoadWriteService>().Cars.Select(x => x.Name).ToList();
            Tracks = DependencyService.Get<DataLoadWriteService>().Tracks.Select(x => x.Name).ToList();
            IsIRacingContentDownload = (_tracks.Count > 0 && _cars.Count > 0);
            RaisePropertyChanged("IsIRacingContentDownload");
        }
    }
}
