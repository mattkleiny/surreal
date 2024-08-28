using Surreal.Assets;
using Surreal.Editing.Projects;

namespace Surreal.Editing.Common;

/// <summary>
/// Base class for any view model in the editor.
/// </summary>
public abstract class EditorViewModel : INotifyPropertyChanged, INotifyPropertyChanging
{
  /// <summary>
  /// The active <see cref="EditorProjectContext"/>.
  /// </summary>
  private static EditorProjectContext? Context => EditorApplication.Current?.Context;

  /// <summary>
  /// The current <see cref="EditorConfiguration"/>.
  /// </summary>
  public static EditorConfiguration? Configuration => Context?.Configuration;

  /// <summary>
  /// The current <see cref="EditorProject"/>.
  /// </summary>
  public static EditorProject? Project => Context?.Project;

  /// <summary>
  /// The current <see cref="AssetDatabase"/>.
  /// </summary>
  public static AssetDatabase? Assets => Context?.Assets;

  /// <inheritdoc/>
  public event PropertyChangedEventHandler? PropertyChanged;

  /// <inheritdoc/>
  public event PropertyChangingEventHandler? PropertyChanging;

  /// <summary>
  /// Sets the value of the given field and notifies listeners if the value has changed.
  /// </summary>
  protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    return SetField(ref field, value, EqualityComparer<T>.Default, propertyName);
  }

  /// <summary>
  /// Sets the value of the given field and notifies listeners if the value has changed.
  /// </summary>
  protected bool SetField<T>(ref T field, T value, EqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
  {
    if (!comparer.Equals(field, value))
    {
      NotifyPropertyChanging(propertyName);
      field = value;
      NotifyPropertyChanged(propertyName);

      return true;
    }

    return false;
  }

  /// <summary>
  /// Notifies listeners that the given property is changing.
  /// </summary>
  protected virtual void NotifyPropertyChanging([CallerMemberName] string? propertyName = null)
  {
    PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
  }

  /// <summary>
  /// Notifies listeners that the given property has changed.
  /// </summary>
  protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}
