using System.Windows.Controls;
using RMIDispenseSequenceEditor.ViewModels;

namespace RMIDispenseSequenceEditor.Views
{
    public partial class IngredientsTableControl : UserControl
    {
        public IngredientsTableControl()
        {
            InitializeComponent();
            DataContext = new IngredientsTableViewModel();
        }
    }
}