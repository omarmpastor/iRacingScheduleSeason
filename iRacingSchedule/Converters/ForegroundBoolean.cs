using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace iRacingSchedule.Converters
{
    internal class ForegroundBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valBoolean = (bool)value;
            if (valBoolean)
            {
                return Brushes.Green;
            }

            return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
