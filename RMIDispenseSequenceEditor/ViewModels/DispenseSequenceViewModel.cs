using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RMIDispenseSequenceEditor.ViewModels
{

    public class DispenseSequenceViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    // visible left number
    public int StepIndex { get; set; }

    // Ingredients available on the left side (source list)
    public ObservableCollection<string> Ingredients { get; set; }
        = new ObservableCollection<string>();

    // Destination slot rows that hold chips
    public ObservableCollection<DestinationSlot> Slots { get; set; }
        = new ObservableCollection<DestinationSlot>();


    // ----------------------------
    // Constructor
    // ----------------------------
    public DispenseSequenceViewModel(int slotCount = 5)
    {
        GenerateSlots(slotCount);
    }


    // ----------------------------
    // Create placeholder rows
    // ----------------------------
    public void GenerateSlots(int count)
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
            if (slot.Chips.Contains(chip))
            {
                slot.Chips.Remove(chip);
                slot.Notify(nameof(slot.Chips));
                break;
            }
        }

        // Add to destination slot
        var target = Slots[targetSlotIndex - 1];

        if (!target.Chips.Contains(chip))
        {
            target.Chips.Add(chip);
            target.Notify(nameof(target.Chips));
        }

        Notify(nameof(Slots));
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