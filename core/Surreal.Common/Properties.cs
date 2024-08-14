using System.Linq.Expressions;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A delegate that can handle property change events.
/// </summary>
public delegate void PropertyEventHandler(object sender, string name);

/// <summary>
/// A property that can be listened to for changes.
/// </summary>
public interface IProperty
{
  /// <summary>
  /// Notifies listeners of dynamic changes to a property value,
  /// </summary>
  event PropertyEventHandler? PropertyChanging;

  /// <summary>
  /// Notifies listeners of dynamic changes to a property value,
  /// </summary>
  event PropertyEventHandler? PropertyChanged;
}

/// <summary>
/// Represents a heap-allocated property.
/// </summary>
public interface IProperty<T>
{
  /// <summary>
  /// Gets/sets the current value of the property.
  /// </summary>
  T Value { get; set; }
}

/// <summary>
/// Static factory for <see cref="IProperty{T}"/>s.
/// </summary>
public static class Property
{
  /// <summary>
  /// Creates a new <see cref="IProperty{T}"/> from the given <see cref="Expression{TDelegate}"/>.
  /// </summary>
  public static IProperty<T> FromExpression<TRoot, T>(TRoot root, Expression<Func<TRoot, T?>> expression)
    where TRoot : class
  {
    if (!expression.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException("The given expression doesn't represent a valid property");
    }

    if (propertyInfo.GetMethod == null || propertyInfo.SetMethod == null)
    {
      throw new InvalidOperationException($"The given property {propertyInfo} doesn't have a valid getter and setter");
    }

    var getter = propertyInfo.GetMethod.CreateDelegate<Func<T>>(root);
    var setter = propertyInfo.SetMethod.CreateDelegate<Action<T>>(root);

    return new DelegateProperty<T>(getter, setter);
  }

  /// <summary>
  /// A <see cref="IProperty{T}"/> that uses a getter and setter to access a value.
  /// </summary>
  private sealed class DelegateProperty<T>(Func<T> getter, Action<T> setter) : IProperty<T>
  {
    public T Value
    {
      get => getter();
      set => setter(value);
    }
  }
}
