namespace Avventura.Core.Actors;

/// <summary>Abstractly represents an object that can be commanded about the world.</summary>
public interface IPawn
{
  // directives
  void Halt();
  void Move(Point2 direction);
}

/// <summary>Allows controlling a <see cref="Character"/>.</summary>
public sealed class Pawn : IPawn
{
  public Pawn(Character character)
  {
    Character = character;
  }

  public Character Character { get; }

  public void Halt()
  {
    throw new NotImplementedException();
  }

  public void Move(Point2 direction)
  {
    Character.Position += direction;
  }
}
