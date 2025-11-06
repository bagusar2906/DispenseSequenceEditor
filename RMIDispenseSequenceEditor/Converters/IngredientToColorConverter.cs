using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RMIDispenseSequenceEditor.Converters
{
    public class IngredientToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ingredient = value as string;
            if (string.IsNullOrEmpty(ingredient))
                return new SolidColorBrush(Color.FromRgb(224, 224, 224)); // grey 300

            Color bgColor;

            switch (ingredient.ToLower())
            {
                case "protein":
                    bgColor = (Color)ColorConverter.ConvertFromString("#90CAF9"); // Blue 300
                    break;
                case "seed":
                    bgColor = (Color)ColorConverter.ConvertFromString("#A5D6A7"); // Green 300
                    break;
                case "buffer":
                    bgColor = (Color)ColorConverter.ConvertFromString("#FFF59D"); // Yellow 300
                    break;
                case "well":
                    bgColor = (Color)ColorConverter.ConvertFromString("#F48FB1"); // Pink 300
                    break;
                case "fragment":
                    bgColor = (Color)ColorConverter.ConvertFromString("#CE93D8"); // Purple 300
                    break;
                case "additive":
                    bgColor = (Color)ColorConverter.ConvertFromString("#FFCC80"); // Orange 300
                    break;
                default:
                    bgColor = (Color)ColorConverter.ConvertFromString("#E0E0E0"); // Grey 300
                    break;
            }

            return new SolidColorBrush(bgColor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Brush GetTextBrushForBackground(Color bgColor)
        {
            // Perceived brightness (YIQ)
            double brightness = (bgColor.R * 0.299 + bgColor.G * 0.587 + bgColor.B * 0.114) / 255;
            return brightness > 0.6 ? Brushes.Black : Brushes.White;
        }
    }
}