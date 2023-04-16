using iRacingSchedule.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace iRacingSchedule.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel(frameMainWindow);
            this.DataContext = _viewModel;
        }

        private async void MenuOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (openFileDialog.ShowDialog() == true)
            {
                var wait = new WaitSpinner
                {
                    Height = 70,
                    Width = 70,
                    Foreground = Brushes.SteelBlue,
                    IsEnabled = true,
                    Owner = this
                };
                wait.Show();
                this.IsEnabled = false;
                await _viewModel.LoadScheduleFromPDFAsync(openFileDialog.FileName);
                this.IsEnabled = true;
                wait.IsEnabled = false;
                wait.Close();
                
                //_viewModel.LoadScheduleFromFile(openFileDialog.FileName);
                NavigateTo<SelectSeries>();
            }
        }

        public void NavigateTo<T>()
        {
            _viewModel.Navigate<T>();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuConfigProxy_Click(object sender, RoutedEventArgs e)
        {
            ConfigProxyWindow configProxyWindow = new ConfigProxyWindow();
            configProxyWindow.Show();
        }

        private async void MenuExportar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos Excel (*.xlsx)|*.xlsx|Archivos JSON (*.json)|*.json|Archivos CSV (*.csv)|*.csv";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (saveFileDialog.ShowDialog() == true)
            {
                var wait = new WaitSpinner
                {
                    Height = 70,
                    Width = 70,
                    Foreground = Brushes.SteelBlue,
                    IsEnabled = true,
                    Owner = this
                };
                wait.Show();
                this.IsEnabled = false;
                await _viewModel.ExportScheduleAsync(saveFileDialog.FileName);
                this.IsEnabled = true;
                wait.IsEnabled = false;
                wait.Close();
                
            }
        }

        private void frameMainWindow_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var type = frameMainWindow.Content.GetType();
            _viewModel.UpdateIsFixSeriesPageActive(type.Name);
            
        }
    }
}
