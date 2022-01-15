namespace Isaac.Actors;

/// <summary>The player <see cref="Character"/>.</summary>
public sealed class Player : Character
{
  private readonly PlayerState state;

  public Player(IActorContext context, PlayerState state)
    : base(context)
  {
    this.state = state;
  }

  public int Health
  {
    get => state.Health;
    set => state.Health = value;
  }

  public int Coins
  {
    get => state.Coins;
    set => state.Coins = value;
  }

  public int Bombs
  {
    get => state.Bombs;
    set => state.Bombs = value;
  }

  public int Damage
  {
    get => state.Damage;
    set => state.Damage = value;
  }
}
