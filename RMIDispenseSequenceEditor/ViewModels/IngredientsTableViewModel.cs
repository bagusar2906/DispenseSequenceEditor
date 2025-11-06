using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RMIDispenseSequenceEditor.ViewModels
{
    public class IngredientsTableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Column1Ingredients { get; set; }
        public ObservableCollection<string> Column2Ingredients { get; set; }
        public ObservableCollection<string> Column3Ingredients { get; set; }

        public IngredientsTableViewModel()
        {
            // Example content for each column
            Column1Ingredients = new ObservableCollection<string>
            {
                "Protein",
                "Well",
                "Seed"
            };

            Column2Ingredients = new ObservableCollection<string>
            {
                "Seed",
                "Protein",
                "Well"
            };

            Column3Ingredients = new ObservableCollection<string>
            {
                "Seed",
                "Protein",
                "Well"
            };
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}