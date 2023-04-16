using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace iRacingSchedule.Converters
{
    internal class ForegroundIsStringEmpty : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            if(str == null || str.Length == 0)
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
