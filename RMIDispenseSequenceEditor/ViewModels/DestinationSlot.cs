namespace RMIDispenseSequenceEditor.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class DestinationSlot : INotifyPropertyChanged
    {
        public DestinationSlot()
        {
            Chips.CollectionChanged += (s, e) =>
            {
                Notify(nameof(PlaceholderText));
            };
            IsNormal = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int SlotIndex { get; set; }
        
        public bool IsNormal { get; set; }
        public bool IsBatch { get; set; }
        public bool IsAliquot { get; set; }

        public ObservableCollection<string> Chips { get; set; }
            = new ObservableCollection<string>();

        public string PlaceholderText =>
            Chips.Count == 0 ? "Drop ingredients here" : "";

        public bool IsAlternate { get; set; }

        public void Notify(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}