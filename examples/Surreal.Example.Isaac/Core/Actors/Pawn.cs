namespace Isaac.Core.Actors;

/// <summary>Status for <see cref="IPawn.Steer"/> commands.</summary>
public enum SteeringStatus
{
  Nominal,
  Blocked,
  Arrived,
}

/// <summary>Status for <see cref="IPawn.PathTo"/> commands.</summary>
public enum PathingStatus
{
  Travelling,
  Blocked,
  Arrived,
}

/// <summary>Abstractly represents an object that can be commanded about the world.</summary>
public interface IPawn
{
  // directives
  void Halt();
  void Move(Vector2 direction);

  // locomotion
  SteeringStatus Steer(Vector2 direction);
  PathingStatus PathTo(Vector2 position);

  // combat
  void PrimaryAttack(Vector2 direction);
  void SecondaryAttack(Vector2 direction);
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

  public void Move(Vector2 direction)
  {
    Character.Position += direction * Character.MoveSpeed;
  }

  public SteeringStatus Steer(Vector2 direction)
  {
    throw new NotImplementedException();
  }

  public PathingStatus PathTo(Vector2 position)
  {
    throw new NotImplementedException();
  }

  public void PrimaryAttack(Vector2 direction)
  {
    throw new NotImplementedException();
  }

  public void SecondaryAttack(Vector2 direction)
  {
    throw new NotImplementedException();
  }
}
