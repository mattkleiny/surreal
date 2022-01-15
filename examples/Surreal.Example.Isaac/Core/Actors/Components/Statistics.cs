namespace Isaac.Core.Actors.Components;

/// <summary>Character statistics.</summary>
public record struct Statistics
{
  private int health;
  private int bombs;
  private int coins;

  public int Health
  {
    get => health;
    set => health = value.Clamp(0, 100);
  }

  public int Bombs
  {
    get => bombs;
    set => bombs = value.Clamp(0, 99);
  }

  public int Coins
  {
    get => coins;
    set => coins = value.Clamp(0, 99);
  }
}