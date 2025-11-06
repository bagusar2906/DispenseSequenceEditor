using System;
using System.Linq;
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

        public async Task PlayStackingAnimationAsync(int staggerMs = 100)
        {
            var vm = DataContext as IngredientsTableViewModel;
            if (vm == null)
                return;

            // Sort dictionary keys ascending (insertion order)
            var ordered = vm.InsertSequence.OrderBy(kv => kv.Key).ToList();

            // Reverse iteration so items stack from bottom first
           //for (int i = ordered.Count - 1; i >= 0; i--)
           for (int i = 0; i < ordered.Count; i++)
            {
                var kvp = ordered[i];
                var parts = kvp.Value.Split(':');
                if (parts.Length != 2) continue;

                int col = int.Parse(parts[0]);
                string name = parts[1];

                ItemsControl list = null;
                switch (col)
                {
                    case 1:
                        list = Column1List;
                        break;
                    case 2:
                        list = Column2List;
                        break;
                    case 3:
                        list = Column3List;
                        break;
                    default: continue;
                }

                list.UpdateLayout();

                if (list.ItemContainerGenerator.ContainerFromItem(name) is FrameworkElement container)
                {
                    var text = FindVisualChild<TextBlock>(container);
                    if (text == null) continue;

                    text.Opacity = 0;
                    var tt = text.RenderTransform as TranslateTransform;
                    if (tt != null)
                    {
                        tt = new TranslateTransform { Y = 15 }; // Start below, not above
                        text.RenderTransform = tt;
                    }

                    // Fade-in and lift-up animation
                    var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400))
                    {
                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                    };

                    var rise = new DoubleAnimation(15, 0, TimeSpan.FromMilliseconds(400))
                    {
                        EasingFunction = new BounceEase
                        {
                            Bounces = 1,
                            Bounciness = 2,
                            EasingMode = EasingMode.EaseOut
                        }
                    };

                    text.BeginAnimation(UIElement.OpacityProperty, fade);
                    tt.BeginAnimation(TranslateTransform.YProperty, rise);

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