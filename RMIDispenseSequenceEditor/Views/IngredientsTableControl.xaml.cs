using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class IngredientsTableControl : UserControl
    {
        public IngredientsTableControl()
        {
            InitializeComponent();
        }

        public async Task PlayFallingAnimationAsync(int staggerMs = 100)
        {
            var columns = new[] { Column1List, Column2List, Column3List };
            foreach (var list in columns)
            {
                if (list == null) continue;
                await AnimateColumnAsync(list, staggerMs);
            }
        }

        private async Task AnimateColumnAsync(ItemsControl itemsControl, int staggerMs)
        {
            for (int i = itemsControl.Items.Count - 1; i >= 0; i--)
            {
                var item = itemsControl.Items[i];
                itemsControl.UpdateLayout();
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                {
                    var text = FindVisualChild<TextBlock>(container);
                    if (text == null) continue;

                    text.Opacity = 0;
                    var tt = text.RenderTransform as TranslateTransform;
                    if (tt != null)
                    {
                        tt = new TranslateTransform { Y = -15 };
                        text.RenderTransform = tt;
                    }

                    var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400))
                    {
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };

                    var fall = new DoubleAnimation(15, 0, TimeSpan.FromMilliseconds(400)) // instead of -15 → 0
                    {
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };

                   

                    text.BeginAnimation(UIElement.OpacityProperty, fade);
                    tt.BeginAnimation(TranslateTransform.YProperty, fall);

                    await Task.Delay(staggerMs);
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}