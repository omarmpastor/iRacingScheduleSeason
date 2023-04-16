using iRacingSchedule.Services;
using System.Windows;

namespace iRacingSchedule
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() 
        {
            DependencyService.Register<DataConverterService>();
            DependencyService.Register<DataLoadWriteService>();
            DependencyService.Get<DataLoadWriteService>().LoadLatestVersion();
        }
    }
}
