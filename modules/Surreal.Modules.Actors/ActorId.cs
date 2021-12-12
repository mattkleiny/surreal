namespace Surreal;

/// <summary>Uniquely identifies a single <see cref="Actor"/>.</summary>
public readonly record struct ActorId(uint Id)
{
  private static uint nextId = 1;

  public static ActorId Invalid    => default;
  public static ActorId Allocate() => new(Interlocked.Increment(ref nextId));

  public bool IsValid => Id != 0;

  public override string ToString()
  {
    return Id.ToString();
  }
}
