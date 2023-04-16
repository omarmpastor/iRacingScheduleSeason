using iRacingSchedule.Services;
using iRacingSchedule.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iRacingSchedule.Views
{
    /// <summary>
    /// Lógica de interacción para SelectSeries.xaml
    /// </summary>
    public partial class SelectSeries : Page
    {
        SelectSeriesViewModel _viewModel;
        public SelectSeries()
        {
            InitializeComponent();
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            _viewModel = new SelectSeriesViewModel();
            this.DataContext = _viewModel;
        }

        private void btnSelectSeries_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsChechedSchedule())
            {
                MessageBox.Show("No has seleccionado ninguna serie");
                return;
            }
            var mainWindow = Window.GetWindow(App.Current.MainWindow) as MainWindow;
            mainWindow.NavigateTo<FixSeries>();
        }

        private void checkSerie_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateCheckedSerie();
        }

        private async void btDownloadIracingContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wait = new WaitSpinner
                {
                    Height = 70,
                    Width = 70,
                    Foreground = Brushes.SteelBlue,
                    IsEnabled = true
                };
                wait.Show();
                this.IsEnabled = false;
                await _viewModel.DownloadIracingContentAsync();
                this.IsEnabled = true;
                wait.IsEnabled = false;
                wait.Close();
            }
            catch (AggregateException ex)
            {
                MessageBox.Show(ex.Message, "Error de conexion",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
