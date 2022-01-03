using System.Globalization;

namespace Surreal;

/// <summary>Uniquely identifies a single <see cref="Actor"/>.</summary>
public readonly record struct ActorId(ulong Id)
{
  private static ulong nextId = 1;

  public static ActorId Invalid    => default;
  public static ActorId Allocate() => new(Interlocked.Increment(ref nextId));

  public bool IsInvalid => Id == 0;
  public bool IsValid   => Id != 0;

  public override string ToString()
  {
    return Id.ToString(CultureInfo.InvariantCulture);
  }
}
