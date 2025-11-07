using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class IngredientsTableControl : UserControl
    {
        public IngredientsTableControl()
        {
            InitializeComponent();
        }

        public async Task PlayBottomUpByColumnAsync(int staggerMs)
        {
         
            // Let WPF finish building visuals before animating
            await Dispatcher.Yield(DispatcherPriority.Background);

            await AnimateColumnByAddOrder(staggerMs);
            
        }

        // ----------------- ANIMATION CORE -----------------

        private async Task AnimateColumnByAddOrder(int staggerMs)
        {
            if (!(DataContext is IngredientsTableViewModel vm))
                return;

            // Make sure visuals are ready
            await Dispatcher.Yield(DispatcherPriority.Background);

            // Hide all items initially
            var allLists = new[] { Column1List, Column2List, Column3List };
            foreach (var list in allLists)
            {
                if (list == null) continue;
                list.UpdateLayout();

                foreach (var item in list.Items)
                {
                    if (!(list.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)) continue;
                    var grid = FindVisualChild<Grid>(container);
                    if (grid == null) continue;
                    grid.Opacity = 0;
                    grid.RenderTransform = new TranslateTransform { Y = -50 }; // Start above
                }
            }

            // Load colors
            var dict = (ResourceDictionary)Application.LoadComponent(
                new Uri("/RMIDispenseSequenceEditor;component/Styles/IngredientsColors.xaml", UriKind.Relative));

            IEasingFunction ease = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            // Sort by insertion order
            var ordered = vm.InsertSequence.OrderBy(kv => kv.Key).ToList();

            foreach (var kvp in ordered)
            {
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

                // Find container for this ingredient
                if (!(list.ItemContainerGenerator.ContainerFromItem(name) is FrameworkElement container)) continue;
                var grid = FindVisualChild<Grid>(container);
                if (grid == null) continue;

                // Select color based on ingredient name
                var targetColor = Colors.LightGray;
                if (name.Equals("Protein", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["ProteinBrush"]).Color;
                else if (name.Equals("Seed", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["SeedBrush"]).Color;
                else if (name.Equals("Additive", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["AdditiveBrush"]).Color;
                else if (name.Equals("Well", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["WellBrush"]).Color;
                else if (name.Equals("Fragment", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["FragmentBrush"]).Color;

                // Create unfrozen brushes for animation
                var backBrush = new SolidColorBrush(Colors.Transparent);
                var borderBrush = new SolidColorBrush(Colors.Transparent);
                grid.Background = backBrush;
                grid.SetValue(Border.BorderBrushProperty, borderBrush);

                    
                const double startY = -150;
                var tt = new TranslateTransform { Y = startY };
                grid.RenderTransform = tt;
                grid.Opacity = 0;

                // --- Animations ---
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = ease
                };

                var fall = new DoubleAnimation(startY, 0, TimeSpan.FromMilliseconds(500))
                {
                    EasingFunction = new BounceEase
                    {
                        Bounces = 0,
                        Bounciness = 3,
                        EasingMode = EasingMode.EaseOut
                    }
                };

                var colorAnim = new ColorAnimation
                {
                    From = Colors.Transparent,
                    To = targetColor,
                    Duration = new Duration(TimeSpan.FromMilliseconds(400))
                };

                // --- Apply ---
                grid.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                tt.BeginAnimation(TranslateTransform.YProperty, fall);
                backBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

                // Wait before next item starts
                await Task.Delay(staggerMs);
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