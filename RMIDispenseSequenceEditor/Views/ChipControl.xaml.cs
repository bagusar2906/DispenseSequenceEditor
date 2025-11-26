using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class ChipControl : UserControl
    {
        public ChipControl()
        {
            InitializeComponent();
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ChipControl));

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }
        
        // inside ChipControl class
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ChipControl), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(nameof(RemoveCommand), typeof(ICommand), typeof(ChipControl));
        
        public ICommand MoveCommand
        {
            get => (ICommand)GetValue(MoveCommandProperty);
            set => SetValue(MoveCommandProperty, value);
        }

        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register(nameof(MoveCommand), typeof(ICommand),
                typeof(ChipControl));

        public static readonly DependencyProperty SlotIndexProperty =
            DependencyProperty.Register(nameof(SlotIndex), typeof(int), typeof(ChipControl));
        
        public int SlotIndex
        {
            get => (int)GetValue(SlotIndexProperty);
            set => SetValue(SlotIndexProperty, value);
        }

        public bool IsFromSource
        {
            get => (bool)GetValue(IsFromSourceProperty);
            set => SetValue(IsFromSourceProperty, value);
        }

        public static readonly DependencyProperty IsFromSourceProperty =
            DependencyProperty.Register(nameof(IsFromSource), typeof(bool), typeof(ChipControl),
                new PropertyMetadata(false));


        private void Chip_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.GetPosition(null);
        }

        private void Chip_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (IsClickOnCloseButton(e))
            {
                RemoveCommand?.Execute(Text);
                return;
            }

            var data = new DataObject();
            data.SetData("ChipData", this.Text);   // <-- THE KEY
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        
        }

        private void Chip_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("ChipData"))
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                ChipBorder.Opacity = 0.5; // Highlight
            }
        }

        private bool _isPrevStateDragLeave = false;
        private void Chip_DragLeave(object sender, DragEventArgs e)
        {
            ChipBorder.Opacity = 1;
            _isPrevStateDragLeave = true;
        }

        private void Chip_Drop(object sender, DragEventArgs e)
        {
            ChipBorder.Opacity = 1;

            if (!e.Data.GetDataPresent("ChipData"))
                return;

            if (!_isPrevStateDragLeave)
                return;
            _isPrevStateDragLeave = false;
            
            string draggedText = (string)e.Data.GetData("ChipData");
            string targetText = this.Text;

            // Fire parent command (reorder)
            MoveCommand?.Execute(new DragDropPair(draggedText, targetText));
        }
        
        private bool IsClickOnCloseButton(MouseEventArgs e)
        {
            if (CloseButton == null)
                return false;

            var pos = e.GetPosition(CloseButton);
            return pos.X >= 0 && pos.X <= CloseButton.ActualWidth &&
                   pos.Y >= 0 && pos.Y <= CloseButton.ActualHeight;
        }
        
    }
    
    public class DragDropPair
    {
        public string From { get; }
        public string To { get; }

        public DragDropPair(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}