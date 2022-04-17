﻿using Surreal.Effects;
using Surreal.Persistence;

namespace Avventura.Core.Actors;

// player related messages
public readonly record struct PlayerSpawned(Player Player);
public readonly record struct PlayerDamaged(Player Player, Damage Damage);
public readonly record struct PlayerDestroyed(Player Player);
public readonly record struct PlayerGainedStatus(Player Player, StatusEffect Effect);
public readonly record struct PlayerLostStatus(Player Player, StatusEffect Effect);

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
    writer.Write(PropertyTypes.Toxin, Toxin);
    writer.Write(PropertyTypes.Coins, Coins);

    if (context.Mode == PersistenceMode.Permanent)
    {
      writer.Write(LastPosition, Transform.Position);
    }
  }

  void IPersistentObject.OnResumeState(PersistenceContext context, IPersistenceReader reader)
  {
    Health = reader.Read(PropertyTypes.Health, 100);
    Toxin = reader.Read(PropertyTypes.Toxin);
    Coins = reader.Read(PropertyTypes.Coins);

    if (context.Mode == PersistenceMode.Permanent)
    {
      Transform.Position = reader.Read(LastPosition);
    }
  }
}