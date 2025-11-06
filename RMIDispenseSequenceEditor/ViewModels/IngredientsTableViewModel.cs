using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RMIDispenseSequenceEditor.ViewModels
{
    public class IngredientsTableViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _column1Ingredients ;

        private int _insertCounter = 0;

        public IngredientsTableViewModel()
        {
            InsertSequence = new SortedDictionary<int, string>();
        }

        public void AddIngredient(int column, string name)
        {
            switch (column)
            {
                case 1:
                    Column1Ingredients.Add(name);
                    break;
                case 2:
                    Column2Ingredients.Add(name);
                    break;
                case 3:
                    Column3Ingredients.Add(name);
                    break;
            }

            // store with sequence number
            InsertSequence[_insertCounter++] = $"{column}:{name}";
        }
        
        public void ClearAll()
        {
            Column1Ingredients.Clear();
            Column2Ingredients.Clear();
            Column3Ingredients.Clear();
            InsertSequence.Clear();
            _insertCounter = 0;
        }

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

        public  IDictionary<int, string>  InsertSequence { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}