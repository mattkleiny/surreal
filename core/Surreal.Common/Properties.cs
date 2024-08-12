namespace Surreal;

/// <summary>
/// A delegate that can handle property change events.
/// </summary>
public delegate void PropertyEventHandler(object sender, string name);

/// <summary>
/// Allows listening to changes in a property value.
/// </summary>
public interface IPropertyChangingEvents
{
  /// <summary>
  /// Notifies listeners of dynamic changes to a property value,
  /// </summary>
  event PropertyEventHandler? PropertyChanging;
}

/// <summary>
/// Allows listening to changes in a property value.
/// </summary>
public interface IPropertyChangedEvents
{
  /// <summary>
  /// Notifies listeners of dynamic changes to a property value,
  /// </summary>
  event PropertyEventHandler? PropertyChanged;
}
