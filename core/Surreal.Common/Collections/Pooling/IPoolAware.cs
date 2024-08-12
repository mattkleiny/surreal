namespace Surreal.Collections.Pooling;

/// <summary>
/// Permits an object to respond to pool callbacks.
/// </summary>
public interface IPoolAware
{
  /// <summary>
  /// Callback for when the object is rented from the pool.
  /// </summary>
  void OnRent();

  /// <summary>
  /// Callback for when the object is returned to the pool.
  /// </summary>
  void OnReturn();
}
