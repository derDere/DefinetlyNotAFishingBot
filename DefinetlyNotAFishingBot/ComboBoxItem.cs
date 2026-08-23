using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefinetlyNotAFishingBot {
  internal class ComboBoxItem<T> {
    public T Value { get; set; }
    public string Label { get; set; }
    public ComboBoxItem() { }
    public ComboBoxItem(T value) {
      Value = value;
      Label = value.ToString();
    }
    public ComboBoxItem(T value, string label) : this(value) {
      Label = label;
    }
    public override string ToString() {
      return this.Label;
    }
  }
}
