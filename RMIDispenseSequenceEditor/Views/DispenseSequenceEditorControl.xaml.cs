using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class DispenseSequenceEditorControl : UserControl
    {
        public DispenseSequenceEditorControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is DispenseSequenceViewModel vm)
            {
                vm.PreviewResultCompleted += async (_, __) =>
                {
                    //await IngredientsTable.PlayStackingAnimationAsync(500);
                    await IngredientsTable.PlayBottomUpByColumnAsync(500);
                };
            }
        }
        
        // ============================================================
        // SOURCE INGREDIENT DRAG (Left Column)
        // ============================================================
        private void SourceChip_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var element = sender as FrameworkElement;
            if (element?.DataContext is string chip)
            {
                // Prepare drag
                var data = new DataObject();
                data.SetData("ChipData", chip);

                // REMOVE from source BEFORE drag
                if (DataContext is DispenseSequenceViewModel vm)
                {
                    vm.Ingredients.Remove(chip);
                }

                DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
            }
        }



        // ============================================================
        // SLOT CHIP DRAG (Right Side Chips)
        // ============================================================
        private void SlotChip_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var element = sender as FrameworkElement;
            if (element?.DataContext is string chip)
            {
                var data = new DataObject();
                data.SetData("ChipData", chip);
                DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
            }
        }


        // ============================================================
        // DROP AREA VISUAL FEEDBACK
        // ============================================================
        private void RowPlaceholder_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ChipData"))
                return;

            var grid = sender as Grid;
            if (grid?.Children[0] is Rectangle rect)
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(40, 100, 200, 255)); // light blue highlight
            }

            e.Handled = true;
        }

        private void RowPlaceholder_DragLeave(object sender, DragEventArgs e)
        {
            var grid = sender as Grid;
            if (grid?.Children[0] is Rectangle rect)
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(4, 255, 255, 255)); // original faint white
            }

            e.Handled = true;
        }

        private void RowPlaceholder_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("ChipData"))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }


        // ============================================================
        // FINAL DROP OPERATION (MOVE CHIP TO ROW)
        // ============================================================
        private void RowPlaceholder_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ChipData"))
                return;

            string chip = (string)e.Data.GetData("ChipData");

            // determine which row this drop occurred on
            var grid = sender as Grid;
            int slotIndex = (int)grid.Tag;

            if (DataContext is DispenseSequenceViewModel vm)
            {
                vm.AddOrMoveChip(chip, slotIndex);
            }

            // restore background
            if (grid.Children[0] is Rectangle rect)
            {
                rect.Fill = new SolidColorBrush(Color.FromArgb(4, 255, 255, 255));
            }

            e.Handled = true;
        }
    }
}
