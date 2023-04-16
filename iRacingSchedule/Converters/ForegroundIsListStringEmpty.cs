using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace iRacingSchedule.Converters
{
    internal class ForegroundIsListStringEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> lines = (List<string>)value;
            if (lines == null || lines.Count == 0)
            {
                return Brushes.Red;
            }

            return Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
