using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RMIDispenseSequenceEditor.Converters
{
    public class CheckBoxVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string ingredient)) return Visibility.Collapsed;
            if (ingredient == "Protein" || ingredient == "Seed")
                return Visibility.Visible;

            return Visibility.Collapsed;
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;

            return false;
        }
    }
}