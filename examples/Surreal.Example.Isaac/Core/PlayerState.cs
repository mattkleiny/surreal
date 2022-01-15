using Surreal.IO.Binary;

namespace Isaac.Core;

/// <summary>The state details for the player character.</summary>
public sealed record PlayerState
{
  private int health;
  private int coins;
  private int bombs;
  private int damage;

  public int Health
  {
    get => health;
    set => health = value.Clamp(0, 99);
  }

  public int Coins
  {
    get => coins;
    set => coins = value.Clamp(0, 99);
  }

  public int Bombs
  {
    get => bombs;
    set => bombs = value.Clamp(0, 99);
  }

  public int Damage
  {
    get => damage;
    set => damage = value.Clamp(0, 99);
  }

  /// <summary>The <see cref="BinarySerializer{T}"/> for <see cref="PlayerState"/>.</summary>
  [BinarySerializer(typeof(PlayerState))]
  private sealed class Serializer : BinarySerializer<PlayerState>
  {
    public override async ValueTask SerializeAsync(PlayerState value, IBinaryWriter writer, CancellationToken cancellationToken = default)
    {
      await writer.WriteIntAsync(value.health, cancellationToken);
      await writer.WriteIntAsync(value.coins, cancellationToken);
      await writer.WriteIntAsync(value.bombs, cancellationToken);
      await writer.WriteIntAsync(value.damage, cancellationToken);
    }

    public override async ValueTask<PlayerState> DeserializeAsync(IBinaryReader reader, CancellationToken cancellationToken = default)
    {
      return new PlayerState
      {
        Health = await reader.ReadIntAsync(cancellationToken),
        Coins  = await reader.ReadIntAsync(cancellationToken),
        Bombs  = await reader.ReadIntAsync(cancellationToken),
        Damage = await reader.ReadIntAsync(cancellationToken),
      };
    }
  }
}
