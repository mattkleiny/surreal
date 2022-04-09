using Surreal.Combat;
using Surreal.Persistence;

namespace Isaac.Core.Actors;

public readonly record struct PlayerSpawned(Player Player);
public readonly record struct PlayerDamaged(Player Player, Damage Damage);
public readonly record struct PlayerDestroyed(Player Player);
public readonly record struct PlayerAttacked(Player Player, Vector2 Position, Vector2 Direction);
public readonly record struct PlayerUsedCoin(Player Player, Vector2 Position);
public readonly record struct PlayerUsedBomb(Player Player, Vector2 Position);

/// <summary>The player <see cref="Character"/>.</summary>
public sealed class Player : Character, IPersistentObject
{
  private static Property<Vector2> LastPosition { get; } = new(nameof(LastPosition));

  Guid IPersistentObject.Id { get; } = Guid.Parse("b539cfd7-f9b7-49e1-ab48-4c6d0103950f");

  protected override void OnStart()
  {
    base.OnStart();

    Message.Publish(new PlayerSpawned(this));
  }

  [MessageSubscriber]
  private void OnCharacterDamaged(ref CharacterDamaged message)
  {
    if (message.Character == this)
    {
      Message.Publish(new PlayerDamaged(this, message.Damage));
    }
  }

  [MessageSubscriber]
  private void OnCharacterDestroyed(ref CharacterDestroyed message)
  {
    if (message.Character == this)
    {
      Message.Publish(new PlayerDestroyed(this));
    }
  }

  void IPersistentObject.OnPersistState(PersistenceContext context, IPersistenceWriter writer)
  {
    writer.Write(Properties.Health, Health);
    writer.Write(Properties.MoveSpeed, MoveSpeed);
    writer.Write(Properties.AttackSpeed, AttackSpeed);
    writer.Write(Properties.Range, Range);
    writer.Write(Properties.Bombs, Bombs);
    writer.Write(Properties.Coins, Coins);

    if (context.Mode == PersistenceMode.Permanent)
    {
      writer.Write(LastPosition, Transform.Position);
    }
  }

  void IPersistentObject.OnResumeState(PersistenceContext context, IPersistenceReader reader)
  {
    Health = reader.Read(Properties.Health, 100);
    MoveSpeed = reader.Read(Properties.MoveSpeed);
    AttackSpeed = reader.Read(Properties.AttackSpeed);
    Range = reader.Read(Properties.Range);
    Bombs = reader.Read(Properties.Bombs);
    Coins = reader.Read(Properties.Coins);

    if (context.Mode == PersistenceMode.Permanent)
    {
      Transform.Position = reader.Read(LastPosition);
    }
  }
}
