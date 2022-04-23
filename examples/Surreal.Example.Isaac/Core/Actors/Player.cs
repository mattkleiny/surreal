using Surreal.Combat;
using Surreal.Combat.Effects;
using Surreal.Persistence;

namespace Isaac.Core.Actors;

// player related messages
public readonly record struct PlayerSpawned(Player Player);
public readonly record struct PlayerDamaged(Player Player, Damage Damage);
public readonly record struct PlayerDestroyed(Player Player);
public readonly record struct PlayerGainedStatus(Player Player, StatusEffect Effect);
public readonly record struct PlayerLostStatus(Player Player, StatusEffect Effect);

/// <summary>The player <see cref="Character"/>.</summary>
public sealed class Player : Character, IPersistentObject
{
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

  [MessageSubscriber]
  private void OnCharacterGainedStatusEffect(ref CharacterGainedStatus message)
  {
    if (message.Character == this)
    {
      Message.Publish(new PlayerGainedStatus(this, message.Effect));
    }
  }

  [MessageSubscriber]
  private void OnCharacterLostStatusEffect(ref CharacterLostStatus message)
  {
    if (message.Character == this)
    {
      Message.Publish(new PlayerLostStatus(this, message.Effect));
    }
  }

  void IPersistentObject.OnPersistState(PersistenceContext context, IPersistenceWriter writer)
  {
    writer.Write(PropertyTypes.Health, Health);
    writer.Write(PropertyTypes.MoveSpeed, MoveSpeed);
    writer.Write(PropertyTypes.AttackSpeed, AttackSpeed);
    writer.Write(PropertyTypes.Range, Range);
    writer.Write(PropertyTypes.Bombs, Bombs);
    writer.Write(PropertyTypes.Coins, Coins);
  }

  void IPersistentObject.OnResumeState(PersistenceContext context, IPersistenceReader reader)
  {
    Health = reader.Read(PropertyTypes.Health, 100);
    MoveSpeed = reader.Read(PropertyTypes.MoveSpeed);
    AttackSpeed = reader.Read(PropertyTypes.AttackSpeed);
    Range = reader.Read(PropertyTypes.Range);
    Bombs = reader.Read(PropertyTypes.Bombs);
    Coins = reader.Read(PropertyTypes.Coins);
  }
}
