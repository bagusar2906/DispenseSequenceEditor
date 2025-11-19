using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RMIDispenseSequenceEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class DestinationSlot : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int SlotIndex { get; set; }

        public ObservableCollection<string> Chips { get; set; }
            = new ObservableCollection<string>();

        public string PlaceholderText =>
            Chips.Count == 0 ? "Drop ingredients here" : "";

        public void Notify(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}