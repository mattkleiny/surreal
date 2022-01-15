using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Surreal.Utilities;

/// <summary>Base class for any view model that notifies of property changes.</summary>
public abstract record ViewModel : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  public bool SetProperty<T>(ref T reference, T value, [CallerMemberName] in string propertyName = default!)
  {
    if (!Equals(reference, value))
    {
      reference = value;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

      return true;
    }

    return false;
  }
}
