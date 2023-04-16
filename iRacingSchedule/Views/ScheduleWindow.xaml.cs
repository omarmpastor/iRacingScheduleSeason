using iRacingSchedule.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media;

namespace iRacingSchedule.Views
{
    /// <summary>
    /// Lógica de interacción para ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private ScheduleWindowViewModel _viewModel;
        public ScheduleWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new ScheduleWindowViewModel();
            DataContext = _viewModel;
        }

        private async void btnSaveSchedule_Click(object sender, RoutedEventArgs e)
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
                await _viewModel.ExportSeries(saveFileDialog.FileName);
                this.IsEnabled = true;
                wait.IsEnabled = false;
                wait.Close();
            }
            
        }
    }
}
