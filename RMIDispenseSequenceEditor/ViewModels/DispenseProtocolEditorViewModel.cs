using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using RMIDispenseSequenceEditor.Models;

namespace RMIDispenseSequenceEditor.ViewModels
{
  public class DispenseProtocolEditorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddStepCommand { get; set; }
        
        public ICommand RemoveStepCommand { get; set; }
        public ICommand AddParallelStepCommand { get; set; }
        
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

            
            SelectedIngredient = Ingredients[0];
        }
        
        public bool CanSelectUp => SelectedStep != null && Steps.IndexOf(SelectedStep) > 0;
        public bool CanSelectDown => SelectedStep != null && Steps.IndexOf(SelectedStep) < Steps.Count - 1;

        private void SelectUpCommandHandler(object obj)
        {
            if (SelectedStep == null) return;
            int index = Steps.IndexOf(SelectedStep);
            if (index > 0)
                SelectedStep = Steps[index - 1];
        }

        private void SelectDownCommandHandler(object obj)
        {
            if (SelectedStep == null) return;
            int index = Steps.IndexOf(SelectedStep);
            if (index < Steps.Count - 1)
                SelectedStep = Steps[index + 1];
        }

        private void RemoveStepCommandHandler(object obj)
        {
            if (SelectedStep == null) return;
            foreach (var step in SelectedStep)
            {
                Ingredients.Add(step.ToString());
            }

            SelectedIngredient = Ingredients[0];
            RemoveStep(SelectedStep);
        }

        private void AddParallelStepCommandHandler(object obj)
        {
            if (SelectedStep == null)
                return;
            
            var step = SelectedStep;

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
            if (SelectedStep == null) return;
            var index = Steps.IndexOf(SelectedStep);
            if (index > 0)
            {
                Steps.Move(index, index - 1);
                SelectedStep = Steps[index - 1];
            }
            RaiseNavStateChanged();
        }

        private void MoveDownCommandHandler(object obj)
        {
            if (SelectedStep == null) return;
            var index = Steps.IndexOf(SelectedStep);
            if (index >= 0 && index < Steps.Count - 1)
            {
                Steps.Move(index, index + 1);
                SelectedStep = Steps[index + 1];
            }
            RaiseNavStateChanged();
        }

       

        public ObservableCollection<Step> Steps { get; set; } = new ObservableCollection<Step>();
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
        
        private Step _selectedStep;
        public Step SelectedStep
        {
            get => _selectedStep;
            set
            {
                _selectedStep = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedStep)));
                RaiseNavStateChanged();
            }
        }
        
        // ===== MOVE STATE LOGIC =====

        public bool CanMoveUp => SelectedStep != null && Steps.IndexOf(SelectedStep) > 0;
        public bool CanMoveDown => SelectedStep != null && Steps.IndexOf(SelectedStep) < Steps.Count - 1;

        private void RaiseNavStateChanged()
        {
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
        }

        private void AddStep(string item)
        {
            if (string.IsNullOrEmpty(item)) return;
            
            var step = new Step();
            step.AddParallel(item);
            Steps.Add(step);
            SelectedStep = step;
        }

        public void RemoveStep(Step step)
        {
            if (step != null)
                Steps.Remove(step);
        }

        private void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}