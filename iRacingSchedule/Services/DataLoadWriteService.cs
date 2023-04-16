using iRacingBusinessLayer;
using iRacingBusinessLayer.Models;
using iRacingSchedule.Events;
using iRacingSchedule.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace iRacingSchedule.Services
{
    public class DataLoadWriteService : DataLoadWriteInterface
    {
        private static readonly string CARS_FILE = "cars.json";
        private static readonly string TRACKS_FILE = "tracks.json";
        private static readonly string IRACING_CONTENT_FILE = "iracing_content.json";
        private static readonly string SCHEDULE_FILE = "schedule.dat";

        public List<IRacingContent> Cars { get; set; }
        public List<IRacingContent> Tracks { get; set; }
        public Schedule Schedule { get; set; }
        public event EventHandler ChangeInternetConfigurationEvent;

        public virtual void ChangeInternetConfiguration(EventArgs e)
        {
            ChangeInternetConfigurationEvent?.Invoke(this, e);
        }

        public void LoadLatestVersion()
        {
            if (Schedule != null) return;

            if (File.Exists(IRACING_CONTENT_FILE))
            {
                var content = IOData.DeserializeJson<List<IRacingContent>>(IRACING_CONTENT_FILE);
                Cars = content.FindAll(c => c.Type == IRacingContentType.Car);
                Tracks = content.FindAll(c => c.Type == IRacingContentType.Track);
            }
            else
            {
                Cars = new List<IRacingContent>();
                Tracks = new List<IRacingContent>();
            }
            

            if (File.Exists(SCHEDULE_FILE))
            {
                Schedule = IOData.DeserielizeSchedule(SCHEDULE_FILE);
                Schedule.KinDisciplines();
            }
        }

        public async Task LoadScheduleFromPDFAsync(string pathPDF)
        {
            await Task.Run(() => LoadScheduleFromPDF(pathPDF));
        }

        public void LoadScheduleFromPDF(string pathPDF)
        {
            var schedule = iParser.GetSchedule(pathPDF);
            if (schedule == null) return;
            Schedule = schedule;
            Schedule.KinDisciplines();
            
            IOData.SerielizeSchedule(schedule, SCHEDULE_FILE);
        }

        private List<IRacingContent> DownloadContentType(IRacingContentType contentType)
        {
            List<IRacingContent> content = new List<IRacingContent>();
            string server = string.Empty;
            string user = string.Empty;
            string password = string.Empty;

            if (Properties.Settings.Default.ProxyEnabled)
            {
                server = Properties.Settings.Default.ProxyServer;
                if (Properties.Settings.Default.ProxyUser)
                {
                    user = Properties.Settings.Default.ProxyUsername;
                    password = Properties.Settings.Default.ProxyPassword;
                }
            }

            if (contentType == IRacingContentType.Car)
            {
                content = iRacingLoadPage.GetCars(server, user, password);
            }
            else if (contentType == IRacingContentType.Track)
            {
                content = iRacingLoadPage.GetTracks(server, user, password);
            }

            return content;
        }

        public void DownloadIracingContent()
        {
            Cars = DownloadContentType(IRacingContentType.Car);
            Tracks = DownloadContentType(IRacingContentType.Track);
            List<IRacingContent> contents = new List<IRacingContent>();
            contents.AddRange(Cars);
            contents.AddRange(Tracks);
            IOData.SerializeJsonAsync(contents, IRACING_CONTENT_FILE);
        }

        public async Task DownloadIracingContentAsync()
        {
            await Task.Run(() => DownloadIracingContent());
        }

        public void DownloadCars()
        {
            Cars = DownloadContentType(IRacingContentType.Car);
            IOData.SerializeJsonAsync(Cars, CARS_FILE);
        }

        public void DownloadTracks()
        {
            Tracks = DownloadContentType(IRacingContentType.Track);
            IOData.SerializeJsonAsync(Tracks, TRACKS_FILE);
        }

        public bool CheckConnection()
        {
            return iRacingLoadPage.CheckConnection();
        }

        public async Task ExportJsonAllScheduleAsync(string pathfile)
        {
            await IOData.SerializeJsonAsync<Schedule>(Schedule, pathfile);
        }

        public async Task ExportJsonScheduleBookmarksAsync(string pathfile)
        {
            await IOData.SerializeJsonAsync<Schedule>(Schedule.GetCheckedSchedule(), pathfile);
        }

        public async Task ExportAllScheduleToCsv(string pathfile)
        {
            await IOData.SerializeCsvAsync(Schedule.GetAllSeries(), pathfile);
        }

        public void ExportAllToExcel(string pathfile)
        {
            IOData.SerializeExcelAsync(Schedule.GetAllSeries(), pathfile);
        }

        public async Task ExportScheduleBookmarksToCsv(string pathfile)
        {
            await IOData.SerializeCsvAsync(Schedule.GetCheckedSchedule().GetAllSeries(), pathfile);
        }

        public async Task ExportBookmarksToExcelAsync(string pathfile)
        {
            await IOData.SerializeExcelAsync(Schedule.GetCheckedSchedule().GetAllSeries(), pathfile);
        }
        
        public async Task ExportScheduleBookmarksToExcelAsync(string pathfile, bool isRepeatedTracksCheck = false)
        {
            await IOData.SerializeScheduleExcelAsync(Schedule.GetCheckedSchedule().GetAllSeries(), pathfile, isRepeatedTracksCheck, Tracks);
        }
    }
}
