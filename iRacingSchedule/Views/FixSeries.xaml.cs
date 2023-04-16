using iRacingBusinessLayer.Models;
using iRacingSchedule.ViewModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace iRacingSchedule.Views
{
    public partial class FixSeries : Page
    {
        private FixSeriesViewModel _viewModel;
        public FixSeries()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new FixSeriesViewModel();
            this.DataContext = _viewModel;
        }

        void navigateBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("No entries in back navigation history.");
            }
        }

        private void treeview_disciplines_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Week)
            {
                Week w = (Week)e.NewValue;
                listboxLoaded.DataContext = new List<string> { w.TrackLoad };
                listboxSelect.DataContext = w.PossibleTracks;
                listboxSelect.SelectionMode = SelectionMode.Single;
                lbLoad.Content = "Circuito leido";
                lbSelect.Content = "Selecciona y confirma un circuito";
                _viewModel.SelectedModel = w;
            }
            else if(e.NewValue is Serie)
            {
                Serie s = (Serie)e.NewValue;
                listboxLoaded.DataContext = s.CarsLoad;
                listboxSelect.DataContext = s.PossibleCars;
                listboxSelect.SelectionMode = SelectionMode.Multiple;
                lbLoad.Content = "Coches leidos de la serie";
                lbSelect.Content = "Selecciona y confirma los coches de la serie";
                _viewModel.SelectedModel = s;
            }

            if (listboxSelect.Items.Count > 0)
            {
                listboxSelect.SelectedItem = listboxSelect.Items[0];
            }

            _viewModel.UpdateSelectItemInListView(listboxSelect.SelectedItems.Count);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedItems = new List<string>();
            foreach (string item in listboxSelect.SelectedItems)
            {
                selectedItems.Add(item);
            }

            if (selectedItems.Count < 1)
            {
                MessageBox.Show("Selecciona por lo menos uno");
                return;
            }

            if (_viewModel.SelectedModel is Week)
            {
                ((Week)_viewModel.SelectedModel).Track = selectedItems[0] as string;
            }
            else if (_viewModel.SelectedModel is Serie)
            {
                ((Serie)_viewModel.SelectedModel).Cars = selectedItems;
            }

            _viewModel.UpdateAreAllTheCircuitsAndCarsInTheSeriesFilledProperties();
        }

        private void btnViewSchedule_Click(object sender, RoutedEventArgs e)
        {
            ScheduleWindow scheduleWindow = new ScheduleWindow();
            scheduleWindow.Show();
        }


        private void listboxSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.UpdateSelectItemInListView(listboxSelect.SelectedItems.Count);
        }

        private void btnConfirmAll_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmAllCircuitsAndCarsThatOnlyHaveOneOption();
            _viewModel.UpdateAreAllTheCircuitsAndCarsInTheSeriesFilledProperties();
        }
    }
}
