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
        private bool _isDispensedAcrossColumnsFirst;

        public ObservableCollection<string> ParallelIngredients
        {
            get => _parallelIngredients;
            set
            {
                _parallelIngredients = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ParallelIngredients)));
            }
        }

        public bool IsDispensedAcrossColumnsFirst
        {
            get => _isDispensedAcrossColumnsFirst;
            set
            {
                _isDispensedAcrossColumnsFirst = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDispensedAcrossColumnsFirst)));
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