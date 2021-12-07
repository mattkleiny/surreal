namespace Surreal.Actors;

public readonly record struct ActorId(uint Id)
{
  private static uint nextActorId = 0;

  public static ActorId None       => default;
  public static ActorId Allocate() => new(Interlocked.Increment(ref nextActorId));

  public override string ToString()
  {
    return Id.ToString();
  }
}
