namespace Surreal.Assets;

/// <summary>
/// An internal asset entry in the asset manager.
/// </summary>
public interface IAssetEntry
{
  /// <summary>
  /// Invoked when the dependency has changed.
  /// </summary>
  event Action? Changed;

  /// <summary>
  /// The value of the dependency.
  /// </summary>
  object? Value { get; }
}

/// <summary>
/// Represents an asset in the system <see cref="T"/>.
/// </summary>
public readonly record struct Asset<T>(IAssetEntry Entry)
{
  /// <summary>
  /// Raised when the value of the asset changes.
  /// </summary>
  public event Action? Changed
  {
    add => Entry.Changed += value;
    remove => Entry.Changed -= value;
  }

  /// <summary>
  /// The underlying asset value.
  /// </summary>
  public T Value => (T)Entry.Value!;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static implicit operator T (Asset<T> asset) => asset.Value;
}
