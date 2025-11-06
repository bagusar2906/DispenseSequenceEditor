using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RMIDispenseSequenceEditor.ViewModels
{
    public class IngredientsTableViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _column1Ingredients ;
        public ObservableCollection<string> Column1Ingredients
        {
            get => _column1Ingredients;
            set
            {
                _column1Ingredients = value;
                OnPropertyChanged(nameof(Column1Ingredients));
            }
        }

        private ObservableCollection<string> _column2Ingredients ;
        public ObservableCollection<string> Column2Ingredients
        {
            get => _column2Ingredients;
            set
            {
                _column2Ingredients = value;
                OnPropertyChanged(nameof(Column2Ingredients));
            }
        }

        private ObservableCollection<string> _column3Ingredients ;
        public ObservableCollection<string> Column3Ingredients
        {
            get => _column3Ingredients;
            set
            {
                _column3Ingredients = value;
                OnPropertyChanged(nameof(Column3Ingredients));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}