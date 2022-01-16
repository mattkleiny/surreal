namespace Isaac.Core;

/// <summary>Commonly used <see cref="Property{T}"/>s across the project.</summary>
public static class Properties
{
  public static Property<int> Health { get; } = new(nameof(Health));
  public static Property<int> Bombs  { get; } = new(nameof(Bombs));
  public static Property<int> Coins  { get; } = new(nameof(Coins));
}
