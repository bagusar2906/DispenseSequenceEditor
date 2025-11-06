using System;
using System.Globalization;
using System.Windows.Data;

namespace RMIDispenseSequenceEditor.Converters
{
    public class DelayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)value;
            // each item delayed by 100 ms
            return TimeSpan.FromMilliseconds(index * 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}