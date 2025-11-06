using System.Windows;

namespace RMIDispenseSequenceEditor.Helpers
{
    public static class IngredientHelper
    {
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.RegisterAttached(
                "IsAnimated",
                typeof(bool),
                typeof(IngredientHelper),
                new PropertyMetadata(false, OnIsAnimatedChanged));

        public static void SetIsAnimated(DependencyObject element, bool value)
        {
            element.SetValue(IsAnimatedProperty, value);
        }

        public static bool GetIsAnimated(DependencyObject element)
        {
            return (bool)element.GetValue(IsAnimatedProperty);
        }

        private static void OnIsAnimatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Optional: could handle runtime changes here if needed
        }
    }
}