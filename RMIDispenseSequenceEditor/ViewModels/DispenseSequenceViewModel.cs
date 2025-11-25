using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using RMIDispenseSequenceEditor.Models;

namespace RMIDispenseSequenceEditor.ViewModels
{

    public class DispenseSequenceViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler PreviewResultCompleted;
    
    // visible left number
    public int SlotIndex { get; set; }

    // Ingredients available on the left side (source list)
    public ObservableCollection<string> Ingredients { get; set; }

    // Destination slot rows that hold chips
    public ObservableCollection<DestinationSlot> Slots { get; set; }
        = new ObservableCollection<DestinationSlot>();


    public IngredientsTableViewModel IngredientsTable { get; }
    
    public ICommand RemoveIngredientCommand { get; set; }
    
    public ICommand PreviewCommand { get; set; }

    public bool CanPreview => Slots.Sum(slot => slot.Chips.Count) > 0;
    
    // ----------------------------
    // Constructor
    // ----------------------------
    public DispenseSequenceViewModel()
    {
        
        RemoveIngredientCommand = new RelayCommand(OnRemoveIngredient);
        PreviewCommand = new RelayCommand(PreviewCommandHandler);
        
        IngredientsTable = new IngredientsTableViewModel
        {
            Column1Ingredients = new ObservableCollection<Ingredient>(),
            Column2Ingredients = new ObservableCollection<Ingredient>(),
            Column3Ingredients = new ObservableCollection<Ingredient>()
        };
        var slotCount = 5;
        Ingredients =  new ObservableCollection<string>()
        {
            "Protein", "Well", "Seed", "Fragment", "Additive"
        };
        GenerateSlots(slotCount);
    }
    
    private void PreviewCommandHandler(object obj)
    {
        IngredientsTable.ClearAll();

        FixInconsistentDispense();

        UpdatePreviewTable();
        
        PreviewResultCompleted?.Invoke(this, EventArgs.Empty);
    }

 
    private void OnRemoveIngredient(object ingredientName)
    {
        if (string.IsNullOrWhiteSpace(ingredientName.ToString()))
            return;

        // 1. Remove ingredient from whichever slot contains it
        foreach (var slot in Slots)
        {
            if (slot.Chips.Contains(ingredientName))
            {
                slot.Chips.Remove(ingredientName.ToString());
                break;
            }
        }

        // 2. Add back to source list (if not already there)
        if (!Ingredients.Contains(ingredientName))
        {
            Ingredients.Add(ingredientName.ToString());
        }
    }

    // ----------------------------
    // Create placeholder rows
    // ----------------------------
    private void GenerateSlots(int count)
    {
        Slots.Clear();

        for (int i = 1; i <= count; i++)
        {
            Slots.Add(new DestinationSlot
            {
                SlotIndex = i
            });
        }

        Notify(nameof(Slots));
    }


    // ----------------------------
    // Add OR Move chip into a slot
    // This handles the logic for:
    // - Dropping chips from source
    // - Moving between rows
    // - Moving within the same row
    // ----------------------------
    public void AddOrMoveChip(string chip, int targetSlotIndex)
    {
        if (string.IsNullOrEmpty(chip))
            return;

        var source =   Ingredients.FirstOrDefault(v => v == chip);
        if (source != null)
            Ingredients.Remove(source);
        // Remove from ANY slot that contains it
        foreach (var slot in Slots)
        {
            if (!slot.Chips.Contains(chip)) continue;
            slot.Chips.Remove(chip);
            slot.Notify(nameof(slot.Chips));
            break;
        }

        // Add to destination slot
        var target = Slots[targetSlotIndex - 1];

        if (!target.Chips.Contains(chip))
        {
            target.Chips.Add(chip);
            target.Notify(nameof(target.Chips));
        }

        Notify(nameof(Slots));
        Notify(nameof(CanPreview));
        

        //FixInconsistentDispense();
        //UpdatePreviewTable();
    }

    private void UpdatePreviewTable()
    {
        var altSequences = new List<DestinationSlot>();
        for (var column = 1; column <= 3; column++)
        {
            foreach (var sequence in Slots)
            {
                if (sequence.IsBatch && column == 1)
                {
                    DispenseToAllColumns(altSequences);
                    altSequences.Clear();
                    // dispense to all columns
                    DispenseToAllColumns(sequence);
                    continue;
                }

                if (!sequence.IsBatch && !sequence.IsAlternate)
                {
                    if (sequence.Chips.Count > 0)
                        IngredientsTable.AddIngredient(column, sequence.Chips.ToArray());
                }
                else
                {
                    altSequences.Add(sequence);
                }
            }
        }
    }

    private void DispenseToAllColumns(DestinationSlot sequence)
    {
        for (var column = 1; column <= 3; column++)
        {
            // dispense all together

            if (sequence.Chips.Count > 0)
                IngredientsTable.AddIngredient(column, sequence.Chips.ToArray());
        }
    }

    private void DispenseToAllColumns(List<DestinationSlot> sequences)
    {
        for (var column = 1; column <= 3; column++)
        {
            // dispense all together
            foreach (var sequence in sequences)
                IngredientsTable.AddIngredient(column, sequence.Chips.ToArray());
        }
    }
    private void FixInconsistentDispense()
    {
        var queue = new Queue<DestinationSlot>();
        foreach (var sequence in Slots)
        {
            if (!sequence.IsBatch)
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


    // ----------------------------
    // OPTIONAL: reorder inside slot
    // ----------------------------
    public void ReorderChipInsideSlot(string chip, int slotIndex, int targetIndex)
    {
        var slot = Slots[slotIndex];
        if (!slot.Chips.Contains(chip)) return;

        int oldIndex = slot.Chips.IndexOf(chip);
        if (oldIndex == targetIndex) return;

        slot.Chips.Move(oldIndex, targetIndex);
        slot.Notify(nameof(slot.Chips));
    }


    // ----------------------------
    // INotifyPropertyChanged helper
    // ----------------------------
    private void Notify(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}


}