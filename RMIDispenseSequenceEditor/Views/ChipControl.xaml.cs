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
            DependencyProperty.Register("Text", typeof(string), typeof(ChipControl));

        public ICommand RemoveCommand
        {
            get => (ICommand)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public object RemoveCommandParameter { get; }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(ChipControl));
    }
}