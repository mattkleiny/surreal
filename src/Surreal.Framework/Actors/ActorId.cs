namespace Surreal.Actors
{
  public readonly record struct ActorId(uint Id)
  {
    public static ActorId None => default;

    public override string ToString()
    {
      return Id.ToString();
    }
  }
}
