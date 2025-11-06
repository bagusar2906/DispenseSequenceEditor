using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RMIDispenseSequenceEditor.Converters
{
    public class IngredientToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ingredient = value as string;
            var bgBrush = (Brush)new IngredientToColorConverter().Convert(ingredient, typeof(Brush), null, culture);
            var bgColor = ((SolidColorBrush)bgBrush).Color;
            return IngredientToColorConverter.GetTextBrushForBackground(bgColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}