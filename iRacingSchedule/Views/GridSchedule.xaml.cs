using iRacingBusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iRacingSchedule.Views
{
    public partial class GridSchedule : UserControl
    {
        public List<Serie> Series
        {
            get { return (List<Serie>)GetValue(SeriesProperty); }
            set
            {
                SetValue(SeriesProperty, value);
            }
        }

        public List<IRacingContent> Tracks
        {
            get { return (List<IRacingContent>)GetValue(TracksProperty); }
            set
            {
                SetValue(TracksProperty, value);
            }
        }

        public bool IsRepeatedTracksCheck
        {
            get { return (bool)GetValue(IsRepeatedTracksCheckProperty); }
            set
            {
                SetValue(IsRepeatedTracksCheckProperty, value);
            }
        }

        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
            "Series",
            typeof(List<Serie>),
            typeof(GridSchedule),
            new PropertyMetadata(new List<Serie>(), OnSeriesPropertyChanged));

        public static readonly DependencyProperty IsRepeatedTracksCheckProperty = DependencyProperty.Register(
           "IsRepeatedTracksCheck",
           typeof(bool),
           typeof(GridSchedule),
           new PropertyMetadata(false, OnIsRepeatedTracksCheckPropertyChanged));

        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register(
            "Tracks",
            typeof(List<IRacingContent>),
            typeof(GridSchedule),
            new PropertyMetadata(new List<IRacingContent>(), OnSeriesPropertyChanged));

        public GridSchedule()
        {
            InitializeComponent();
        }

        private static void OnSeriesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gs = d as GridSchedule;
            if (gs != null)
            {
                gs.CreateGrid();
                gs.CheckRepeatedTracks();
            }
        }

        private static void OnIsRepeatedTracksCheckPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gs = d as GridSchedule;
            if (gs != null)
            {
                gs.CheckRepeatedTracks();
            }
        }

        private void CreateTable(int numCols, int numRows)
        {
            for (int i = 0; i < numRows + 1; i++)
            {
                GridScheduleRoot.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < numCols + 1; i++)
            {
                GridScheduleRoot.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (RowDefinition row in GridScheduleRoot.RowDefinitions)
            {
                row.SetResourceReference(StyleProperty, "RowStyleSchedule");
            }

            foreach (ColumnDefinition col in GridScheduleRoot.ColumnDefinitions)
            {
                col.SetResourceReference(StyleProperty, "ColumnStyleSchedule");
            }

            GridScheduleRoot.ColumnDefinitions[0].SetResourceReference(StyleProperty, "FirstColumnStyleSchedule");
        }

        private Border CreateHeaderCell(string text)
        {
            Border border = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            border.SetResourceReference(StyleProperty, "CellHeaderStyle");

            TextBlock txt = new TextBlock { Text = text };

            border.Child = txt;

            return border;
        }

        private Border CreateCell(string text)
        {
            Border border = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            border.SetResourceReference(StyleProperty, "CellStyle");

            TextBlock txt = new TextBlock { Text = text };

            border.Child = txt;

            return border;
        }

        private void CreateGrid()
        {
            GridScheduleRoot.RowDefinitions.Clear();
            GridScheduleRoot.ColumnDefinitions.Clear();
            var tupleBuffer = iRacingBusinessLayer.Helpers.SerieHelper.SeriesToWeeksTableOrderRowsDate(Series);
            List<string> headerNameSerieCol = tupleBuffer.Item1;
            List<DateTime> headerDateWeekRow = tupleBuffer.Item2;
            Week[,] weeksBuffer = tupleBuffer.Item3;

            // Create Grid
            CreateTable(headerNameSerieCol.Count, headerDateWeekRow.Count);

            // First cell
            var emptyCell = CreateHeaderCell("");
            Grid.SetRow(emptyCell, 0);
            Grid.SetColumn(emptyCell, 0);
            GridScheduleRoot.Children.Add(emptyCell);

            // Series name header
            for (int i = 0; i < headerNameSerieCol.Count; i++)
            {
                var cell = CreateHeaderCell(headerNameSerieCol[i]);
                Grid.SetRow(cell, 0);
                Grid.SetColumn(cell, i + 1);
                GridScheduleRoot.Children.Add(cell);
            }

            // Dates left header
            for (int i = 0; i < headerDateWeekRow.Count; i++)
            {
                var cell = CreateHeaderCell(headerDateWeekRow[i].ToString("dd-MM-yyy"));

                Grid.SetRow(cell, i + 1);
                Grid.SetColumn(cell, 0);
                GridScheduleRoot.Children.Add(cell);
            }

            // All tracks
            for (int i = 0; i < headerDateWeekRow.Count; i++)
            {
                for (int j = 0; j < headerNameSerieCol.Count; j++)
                {
                    string track = "";
                    if (weeksBuffer[i, j] != null)
                    {
                        track = weeksBuffer[i, j].TrackLoad;
                    }

                    var cell = CreateCell(track);

                    Grid.SetRow(cell, i + 1);
                    Grid.SetColumn(cell, j + 1);
                    GridScheduleRoot.Children.Add(cell);
                }
            }

            // Format table color
            foreach (var child in GridScheduleRoot.Children)
            {
                if(child is Border)
                {
                    var border = (Border) child;
                    if (Grid.GetRow(border) % 2 == 0) 
                    {
                        border.Background = Brushes.LightSteelBlue;
                    }
                }
            }
        }

        private void MarkTrackRepeat(IGrouping<string, Week> weeksGroup)
        {
            List<string> tracks = weeksGroup.Select(x => x.TrackLoad).ToList();

            foreach (var child in GridScheduleRoot.Children)
            {
                if (child is Border)
                {
                    var border = (Border) child;
                    var textBlock = (TextBlock) border.Child;
                    if (textBlock != null && tracks.Exists(x => x == textBlock.Text))
                    {
                        if (weeksGroup.Count() == 2)
                        {
                            textBlock.Background = Brushes.LightSalmon;
                        }
                        else if (weeksGroup.Count() > 2)
                        {
                            textBlock.Background = Brushes.Salmon;
                        }
                    }
                }
            }
        }

        private void UnmarkTrackRepeat()
        {
            foreach (var child in GridScheduleRoot.Children)
            {
                if (child is Border)
                {
                    var border = (Border)child;
                    var textBlock = (TextBlock)border.Child;
                    if (textBlock != null)
                    {
                        textBlock.Background = Brushes.Transparent;
                    }
                }
            }
        }

        public void CheckRepeatedTracks()
        {
            if (!IsRepeatedTracksCheck) { UnmarkTrackRepeat(); return; }
            //var weeks = Series.SelectMany(s => s.Weeks);
            if(Tracks.Count == 0)
            {
                MessageBox.Show("No se han detectado circuitos, se marcarán como repetidos los de contenido base también");
            }
            
            IEnumerable<IGrouping<string, Week>> weeksgroups = Series.SelectMany(s => s.Weeks).GroupBy(w => w.Track).Where(t => t.Count() > 1);
            var tracksFree = Tracks.Where(c => c.IsFree).ToList();
            

            foreach (IGrouping<string, Week> wg in weeksgroups)
            {
                if(wg.Key != null && wg.Key != "")
                {
                    if (!tracksFree.Exists(x => x.Name == wg.Key))
                    {
                        MarkTrackRepeat(wg);
                    }
                }
                    
            }
        }
    }
}
