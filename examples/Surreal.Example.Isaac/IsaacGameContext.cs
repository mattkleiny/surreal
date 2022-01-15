using Surreal.Reactive;

namespace Isaac;

/// <summary>Context for an on-going game.</summary>
public sealed record IsaacGameContext : ViewModel
{
  public PlayerState Player { get; set; }
}

/// <summary>The state details for the player character.</summary>
public sealed record PlayerState : ViewModel
{
  private int health;
  private int coins;
  private int bombs;
  private int damage;

  public int Health
  {
    get => health;
    set => SetProperty(ref health, value.Clamp(0, 99));
  }

  public int Coins
  {
    get => coins;
    set => SetProperty(ref coins, value.Clamp(0, 99));
  }

  public int Bombs
  {
    get => bombs;
    set => SetProperty(ref bombs, value.Clamp(0, 99));
  }

  public int Damage
  {
    get => damage;
    set => SetProperty(ref damage, value.Clamp(0, 99));
  }
}
