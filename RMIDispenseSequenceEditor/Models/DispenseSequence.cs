using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMIDispenseSequenceEditor.Models
{
  public class DispenseSequence
  {
    public Ingredient[] Ingredients { get; set; }
    public int Column { get; set; }
  }
}
