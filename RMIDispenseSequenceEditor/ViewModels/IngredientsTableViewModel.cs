using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RMIDispenseSequenceEditor.Models;

namespace RMIDispenseSequenceEditor.ViewModels
{
  public class IngredientsTableViewModel : INotifyPropertyChanged
  {
    private ObservableCollection<Ingredient> _column1Ingredients;

    private int _insertCounter = 0;

    public IngredientsTableViewModel()
    {
      InsertSequence = new SortedDictionary<int, DispenseSequence>();
    }

    public void AddIngredient( int column, string[] names )
    {
      var ingredients = new List<Ingredient>();
      foreach(var name in names)
      {
        var ingredient = new Ingredient()
        {
          SequenceOrder = _insertCounter + 1,
          Name = name,
          Column = column
        };
        ingredients.Add(ingredient);
        switch(column)
        {
          case 1:
            Column1Ingredients.Add( ingredient );
            break;
          case 2:
            Column2Ingredients.Add( ingredient );
            break;
          case 3:
            Column3Ingredients.Add( ingredient );
            break;
        }
      }
      // store with sequence number
      InsertSequence[_insertCounter++] = new DispenseSequence { Column = column, Ingredients = ingredients.ToArray() };
    }

    public void ClearAll()
    {
      Column1Ingredients.Clear();
      Column2Ingredients.Clear();
      Column3Ingredients.Clear();
      InsertSequence.Clear();
      _insertCounter = 0;
    }

    public ObservableCollection<Ingredient> Column1Ingredients
    {
      get => _column1Ingredients;
      set
      {
        _column1Ingredients = value;
        OnPropertyChanged( nameof( Column1Ingredients ) );
      }
    }

    private ObservableCollection<Ingredient> _column2Ingredients;
    public ObservableCollection<Ingredient> Column2Ingredients
    {
      get => _column2Ingredients;
      set
      {
        _column2Ingredients = value;
        OnPropertyChanged( nameof( Column2Ingredients ) );
      }
    }

    private ObservableCollection<Ingredient> _column3Ingredients;
    public ObservableCollection<Ingredient> Column3Ingredients
    {
      get => _column3Ingredients;
      set
      {
        _column3Ingredients = value;
        OnPropertyChanged( nameof( Column3Ingredients ) );
      }
    }

    public IDictionary<int, DispenseSequence> InsertSequence { get; }
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged( string propertyName ) =>
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
  }

}