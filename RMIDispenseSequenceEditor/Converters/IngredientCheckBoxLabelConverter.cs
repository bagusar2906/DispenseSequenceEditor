using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace RMIDispenseSequenceEditor.Converters
{
    public class IngredientCheckBoxLabelConverter : IValueConverter
    {
        // If any item equals "Seed" (case-insensitive) -> "Allocating"
        // Otherwise -> "Across Column First"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<string> items)
            {
                foreach (var o in items)
                {
                    if (o?.ToString().Equals("Seed", StringComparison.OrdinalIgnoreCase) == true)
                        return "Aliquoting";
                }
            }

            return "Batch";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}