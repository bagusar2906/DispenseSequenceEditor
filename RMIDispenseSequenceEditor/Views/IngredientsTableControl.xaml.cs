using System;
using System.Collections.Generic;
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
            if (!(DataContext is IngredientsTableViewModel))
                return;

            IngredientsTableViewModel vm = (IngredientsTableViewModel)DataContext;

            // Let WPF finish building visuals before animating
            await Dispatcher.Yield(System.Windows.Threading.DispatcherPriority.Background);

            await AnimateColumnByAddOrder( staggerMs);
           // await AnimateColumnByAddOrder(Column2List, vm.Column2Ingredients, staggerMs);
          //  await AnimateColumnByAddOrder(Column3List, vm.Column3Ingredients, staggerMs);
        }

        // ----------------- ANIMATION CORE -----------------

        private async Task AnimateColumnByAddOrder(int staggerMs)
        {

            var vm = DataContext as IngredientsTableViewModel;
            if (vm == null)
                return;

            // Sort dictionary keys ascending (insertion order)
            var ordered = vm.InsertSequence.OrderBy(kv => kv.Key).ToList();

            
            // Load color dictionary
            var dict = (ResourceDictionary)Application.LoadComponent(
                new Uri("/RMIDispenseSequenceEditor;component/Styles/IngredientsColors.xaml", UriKind.Relative));

            IEasingFunction ease = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            // Animate in insertion order (top-down)
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

                    Border border = FindVisualChild<Border>(container);
                    TextBlock text = FindVisualChild<TextBlock>(container);
                    if (border == null || text == null)
                        continue;

                    
                    // Determine color from IngredientsColors.xaml
                    Color targetColor = Colors.LightGray;
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

                    // Use new unfrozen brushes
                    var animBack = new SolidColorBrush(Colors.Transparent);
                    var animBorder = new SolidColorBrush(Colors.Transparent);
                    border.Background = animBack;
                    border.BorderBrush = animBorder;

                    
                    // Set initial Y offset (start above)
                    var borderTT = new TranslateTransform { Y = -50 };
                    var textTT = new TranslateTransform { Y = -50 };
                    border.RenderTransform = borderTT;
                    text.RenderTransform = textTT;

                    border.Opacity = 0;
                    text.Opacity = 0;

                    // Animations
                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
                    {
                        EasingFunction = ease
                    };

                    var fall = new DoubleAnimation(-50, 0, TimeSpan.FromMilliseconds(1000))
                    {
                        EasingFunction = new BounceEase
                        {
                            Bounces = 0,
                            Bounciness = 5,
                            EasingMode = EasingMode.EaseOut
                        }
                    };

                    var colorAnim = new ColorAnimation
                    {
                        From = Colors.Transparent,
                        To = targetColor,
                        Duration = new Duration(TimeSpan.FromMilliseconds(400))
                    };

                    // Apply
                    border.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    borderTT.BeginAnimation(TranslateTransform.YProperty, fall);
                    animBack.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                    animBorder.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

                    text.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                    textTT.BeginAnimation(TranslateTransform.YProperty, fall);

                    await Task.Delay(staggerMs);
                 
                }
            }
        }


        private async Task AnimateColumnBottomUp(ItemsControl list, IEnumerable<string> items, int staggerMs)
        {
            if (list == null || items == null)
                return;

            List<string> itemList = new List<string>(items);
            if (itemList.Count == 0)
                return;

            // Load your shared color dictionary once
            var dict = (ResourceDictionary)Application.LoadComponent(
                new Uri("/RMIDispenseSequenceEditor;component/Styles/IngredientsColors.xaml", UriKind.Relative));

            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                string item = itemList[i];
                list.UpdateLayout();

                FrameworkElement container = null;
                for (int retry = 0; retry < 3 && container == null; retry++)
                {
                    container = list.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                    if (container == null)
                        await Task.Delay(50);
                }

                if (container == null)
                    continue;

                Border border = FindVisualChild<Border>(container);
                TextBlock text = FindVisualChild<TextBlock>(container);
                if (border == null || text == null)
                    continue;

                // Reset visuals
                border.Opacity = 0;
                text.Opacity = 0;

                TranslateTransform borderTT = new TranslateTransform { Y = 15 };
                TranslateTransform textTT = new TranslateTransform { Y = 20 };
                border.RenderTransform = borderTT;
                text.RenderTransform = textTT;

                // Pick color brush from dictionary
                Color targetColor = Colors.LightGray;
                if (item.Equals("Protein", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["ProteinBrush"]).Color;
                else if (item.Equals("Seed", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["SeedBrush"]).Color;
                else if (item.Equals("Additive", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["AdditiveBrush"]).Color;
                else if (item.Equals("Well", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["WellBrush"]).Color;
                else if (item.Equals("Fragment", StringComparison.OrdinalIgnoreCase))
                    targetColor = ((SolidColorBrush)dict["FragmentBrush"]).Color;

                // Assign new unfrozen brushes
                SolidColorBrush animBack = new SolidColorBrush(Colors.Transparent);
                SolidColorBrush animBorder = new SolidColorBrush(Colors.Transparent);
                border.Background = animBack;
                border.BorderBrush = animBorder;

                IEasingFunction ease = new QuadraticEase { EasingMode = EasingMode.EaseOut };

                // Animations
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400)) { EasingFunction = ease };
                var rise = new DoubleAnimation(15, 0, TimeSpan.FromMilliseconds(400))
                {
                    EasingFunction = new BounceEase { Bounces = 1, Bounciness = 2, EasingMode = EasingMode.EaseOut }
                };

                var colorAnim = new ColorAnimation
                {
                    From = Colors.Transparent,
                    To = targetColor,
                    Duration = new Duration(TimeSpan.FromMilliseconds(400))
                };

                var textFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400))
                {
                    BeginTime = TimeSpan.FromMilliseconds(80),
                    EasingFunction = ease
                };
                var textRise = new DoubleAnimation(20, 0, TimeSpan.FromMilliseconds(400))
                {
                    BeginTime = TimeSpan.FromMilliseconds(80),
                    EasingFunction = ease
                };

                // Apply
                border.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                borderTT.BeginAnimation(TranslateTransform.YProperty, rise);
                animBack.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);
                animBorder.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

                text.BeginAnimation(UIElement.OpacityProperty, textFade);
                textTT.BeginAnimation(TranslateTransform.YProperty, textRise);

                await Task.Delay(staggerMs);
            }
        }


        private async Task AnimateColumnBottomUp2(ItemsControl list, IEnumerable<string> items, int staggerMs)
        {
            if (list == null || items == null)
                return;

            List<string> itemList = new List<string>(items);
            if (itemList.Count == 0)
                return;

            // Animate bottom → top (last item first)
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                string item = itemList[i];
                list.UpdateLayout();

                // Retry until container exists
                FrameworkElement container = null;
                for (int retry = 0; retry < 3 && container == null; retry++)
                {
                    container = list.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                    if (container == null)
                        await Task.Delay(50);
                }

                if (container == null)
                    continue;

                // Find both Border and TextBlock
                Border border = FindVisualChild<Border>(container);
                TextBlock text = FindVisualChild<TextBlock>(container);
                if (border == null || text == null)
                    continue;

                // Reset visuals
                border.Opacity = 0;
                text.Opacity = 0;

                TranslateTransform borderTT = new TranslateTransform();
                TranslateTransform textTT = new TranslateTransform();

                borderTT.Y = 15;
                textTT.Y = 20;

                border.RenderTransform = borderTT;
                text.RenderTransform = textTT;

                IEasingFunction ease = new QuadraticEase { EasingMode = EasingMode.EaseOut };

                // Border fade & rise
                // Border fade + rise + color fade
                var borderFade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(400)))
                    { EasingFunction = ease };
                var borderRise = new DoubleAnimation(15, 0, new Duration(TimeSpan.FromMilliseconds(400)))
                {
                    EasingFunction = new BounceEase
                    {
                        Bounces = 1,
                        Bounciness = 2,
                        EasingMode = EasingMode.EaseOut
                    }
                };

                // Fade border brush from transparent → target color
                var colorAnim = new ColorAnimation
                {
                    From = Colors.Transparent,
                    To = ((SolidColorBrush)border.Background).Color,
                    Duration = new Duration(TimeSpan.FromMilliseconds(400))
                };

                // Create a new unfrozen brush for animation
                SolidColorBrush animBrush = new SolidColorBrush(Colors.Transparent);
                border.Background = animBrush;

                // Apply animations
                border.BeginAnimation(UIElement.OpacityProperty, borderFade);
                borderTT.BeginAnimation(TranslateTransform.YProperty, borderRise);
                animBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);


                // Text fade & rise (slight delay)
                var textFade = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(400)))
                {
                    BeginTime = TimeSpan.FromMilliseconds(80),
                    EasingFunction = ease
                };
                var textRise = new DoubleAnimation(20, 0, new Duration(TimeSpan.FromMilliseconds(400)))
                {
                    BeginTime = TimeSpan.FromMilliseconds(80),
                    EasingFunction = ease
                };

                border.BeginAnimation(UIElement.OpacityProperty, borderFade);
                borderTT.BeginAnimation(TranslateTransform.YProperty, borderRise);

                text.BeginAnimation(UIElement.OpacityProperty, textFade);
                textTT.BeginAnimation(TranslateTransform.YProperty, textRise);

                await Task.Delay(staggerMs);
            }
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