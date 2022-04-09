namespace Isaac.Core.Actors;

/// <summary>Abstractly represents an object that can be commanded about the world.</summary>
public interface IPawn
{
  void Move(Vector2 direction);
}

/// <summary>Allows controlling a <see cref="Character"/>.</summary>
public sealed class Pawn : IPawn
{
  private readonly Character character;

  public Pawn(Character character)
  {
    this.character = character;
  }

  public void Move(Vector2 direction)
  {
    character.Position += direction * character.MoveSpeed;
  }
}
