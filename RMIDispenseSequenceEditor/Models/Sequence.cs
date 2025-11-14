using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RMIDispenseSequenceEditor.Models
{
    public class Sequence : INotifyPropertyChanged, IEnumerable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<string> _parallelIngredients = new ObservableCollection<string>();
        private bool _batch;

        public ObservableCollection<string> ParallelIngredients
        {
            get => _parallelIngredients;
            set
            {
                _parallelIngredients = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParallelIngredients)));
            }
        }

        public bool IsAlternate { get; set; }
        public bool Batch
        {
            get => _batch;
            set
            {
                _batch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Batch)));
            }
        }
        
        public void AddParallel(string ingredient)
        {
            if (!ParallelIngredients.Contains(ingredient))
                ParallelIngredients.Add(ingredient);
        }

        public IEnumerator GetEnumerator()
        {
            var stepList = _parallelIngredients.ToList();

            return stepList.GetEnumerator();
        }
    }
}