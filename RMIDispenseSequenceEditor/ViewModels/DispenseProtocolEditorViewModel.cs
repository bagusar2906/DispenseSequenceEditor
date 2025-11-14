using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using RMIDispenseSequenceEditor.Models;

namespace RMIDispenseSequenceEditor.ViewModels
{
    public class DispenseProtocolEditorViewModel : INotifyPropertyChanged
    {
        public event EventHandler PreviewResultCompleted;

        public IngredientsTableViewModel IngredientsTable { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddStepCommand { get; set; }

        public ICommand RemoveStepCommand { get; set; }
        public ICommand AddParallelStepCommand { get; set; }

        public ICommand PreviewCommand { get; set; }

        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }
        public ICommand SelectUpCommand { get; set; }
        public ICommand SelectDownCommand { get; set; }


        public DispenseProtocolEditorViewModel()
        {
            AddStepCommand = new RelayCommand(AddStepCommandHandler);
            AddParallelStepCommand = new RelayCommand(AddParallelStepCommandHandler);
            RemoveStepCommand = new RelayCommand(RemoveStepCommandHandler);

            MoveUpCommand = new RelayCommand(MoveUpCommandHandler, _ => CanMoveUp);
            MoveDownCommand = new RelayCommand(MoveDownCommandHandler, _ => CanMoveDown);

            SelectUpCommand = new RelayCommand(SelectUpCommandHandler, _ => CanSelectUp);
            SelectDownCommand = new RelayCommand(SelectDownCommandHandler, _ => CanSelectDown);
            PreviewCommand = new RelayCommand(PreviewCommandHandler);

            SelectedIngredient = Ingredients[0];
            IngredientsTable = new IngredientsTableViewModel
            {
                Column1Ingredients = new ObservableCollection<Ingredient>(),
                Column2Ingredients = new ObservableCollection<Ingredient>(),
                Column3Ingredients = new ObservableCollection<Ingredient>()
            };
        }

        private void PreviewCommandHandler(object obj)
        {
            IngredientsTable.ClearAll();

            FixInconsistentDispense();

            UpdatePreviewTable();
        }

        private void UpdatePreviewTable()
        {
            var altSequences = new List<Sequence>();
            for (var column = 1; column <= 3; column++)
            {
                foreach (var sequence in Sequences)
                {
                    if (sequence.Batch && column == 1)
                    {
                        DispenseToAllColumns(altSequences);
                        altSequences.Clear();
                        // dispense to all columns
                        DispenseToAllColumns(sequence);
                        continue;
                    }

                    if (!sequence.Batch && !sequence.IsAlternate)
                    {
                        IngredientsTable.AddIngredient(column, sequence.ParallelIngredients.ToArray());
                    }
                    else
                    {
                        altSequences.Add(sequence);
                    }
                }
            }

            PreviewResultCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void FixInconsistentDispense()
        {
            var queue = new Queue<Sequence>();
            foreach (var sequence in Sequences)
            {
                if (!sequence.Batch)
                {
                    queue.Enqueue(sequence);
                }
                else
                {
                    while (queue.Count > 0)
                    {
                        var altSequence = queue.Dequeue();
                        altSequence.IsAlternate = true;
                    }
                }
            }
        }

        private void DispenseToAllColumns(Sequence sequence)
        {
            for (var column = 1; column <= 3; column++)
            {
                // dispense all together

                IngredientsTable.AddIngredient(column, sequence.ParallelIngredients.ToArray());
            }
        }

        private void DispenseToAllColumns(List<Sequence> sequences)
        {
            for (var column = 1; column <= 3; column++)
            {
                // dispense all together
                foreach (var sequence in sequences)
                    IngredientsTable.AddIngredient(column, sequence.ParallelIngredients.ToArray());
            }
        }

        public bool CanSelectUp => SelectedSequence != null && Sequences.IndexOf(SelectedSequence) > 0;

        public bool CanSelectDown =>
            SelectedSequence != null && Sequences.IndexOf(SelectedSequence) < Sequences.Count - 1;

        public bool CanPreview => Sequences.Count > 0;

        private void SelectUpCommandHandler(object obj)
        {
            if (SelectedSequence == null) return;
            var index = Sequences.IndexOf(SelectedSequence);
            if (index > 0)
                SelectedSequence = Sequences[index - 1];
        }

        private void SelectDownCommandHandler(object obj)
        {
            if (SelectedSequence == null) return;
            var index = Sequences.IndexOf(SelectedSequence);
            if (index < Sequences.Count - 1)
                SelectedSequence = Sequences[index + 1];
        }

        private void RemoveStepCommandHandler(object obj)
        {
            if (SelectedSequence == null) return;
            foreach (var step in SelectedSequence)
            {
                Ingredients.Add(step.ToString());
            }

            SelectedIngredient = Ingredients[0];
            RemoveStep(SelectedSequence);
        }

        private void AddParallelStepCommandHandler(object obj)
        {
            if (SelectedSequence == null)
                return;

            var step = SelectedSequence;

            if (!string.IsNullOrEmpty(_selectedIngredient))
            {
                step.AddParallel(_selectedIngredient);
                RemoveItem();
            }
        }

        private void RemoveItem()
        {
            Ingredients.Remove(_selectedIngredient);
            SelectedIngredient = Ingredients.Count > 0 ? Ingredients[0] : string.Empty;
        }

        private void AddStepCommandHandler(object obj)
        {
            AddStep(SelectedIngredient);
            RemoveItem();
        }

        private void MoveUpCommandHandler(object obj)
        {
            if (SelectedSequence == null) return;
            var index = Sequences.IndexOf(SelectedSequence);
            if (index > 0)
            {
                Sequences.Move(index, index - 1);
                SelectedSequence = Sequences[index - 1];
            }

            RaiseNavStateChanged();
        }

        private void MoveDownCommandHandler(object obj)
        {
            if (SelectedSequence == null) return;
            var index = Sequences.IndexOf(SelectedSequence);
            if (index >= 0 && index < Sequences.Count - 1)
            {
                Sequences.Move(index, index + 1);
                SelectedSequence = Sequences[index + 1];
            }

            RaiseNavStateChanged();
        }


        public ObservableCollection<Sequence> Sequences { get; set; } = new ObservableCollection<Sequence>();

        public ObservableCollection<string> Ingredients { get; set; } = new ObservableCollection<string>
        {
            "Protein", "Seed", "Well", "Fragment", "Additive"
        };

        private string _selectedIngredient;

        public string SelectedIngredient
        {
            get => _selectedIngredient;
            set
            {
                if (_selectedIngredient != value)
                {
                    _selectedIngredient = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIngredient)));
                }
            }
        }

        private Sequence _selectedSequence;

        public Sequence SelectedSequence
        {
            get => _selectedSequence;
            set
            {
                _selectedSequence = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSequence)));
                RaiseNavStateChanged();
            }
        }

        // ===== MOVE STATE LOGIC =====

        public bool CanMoveUp => SelectedSequence != null && Sequences.IndexOf(SelectedSequence) > 0;

        public bool CanMoveDown =>
            SelectedSequence != null && Sequences.IndexOf(SelectedSequence) < Sequences.Count - 1;

        private void RaiseNavStateChanged()
        {
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
            OnPropertyChanged(nameof(CanPreview));
        }

        private void AddStep(string item)
        {
            if (string.IsNullOrEmpty(item)) return;

            var step = new Sequence();
            step.AddParallel(item);
            Sequences.Add(step);
            SelectedSequence = step;
        }

        public void RemoveStep(Sequence sequence)
        {
            if (sequence != null)
                Sequences.Remove(sequence);
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}