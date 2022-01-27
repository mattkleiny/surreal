using Surreal.IO.Persistence;

namespace Isaac.Core.Actors;

/// <summary>The player <see cref="Character"/>.</summary>
public sealed class Player : Character, IPersistentObject
{
  Guid IPersistentObject.Id { get; } = Guid.Parse("b539cfd7-f9b7-49e1-ab48-4c6d0103950f");

  public Player(IActorContext context)
    : base(context)
  {
  }

  void IPersistentObject.OnPersistState(PersistenceContext context, IPersistenceWriter writer)
  {
    writer.Write(SharedProperties.Health, Health);
    writer.Write(SharedProperties.Coins, Coins);
    writer.Write(SharedProperties.Bombs, Bombs);
  }

  void IPersistentObject.OnResumeState(PersistenceContext context, IPersistenceReader reader)
  {
    Health = reader.Read(SharedProperties.Health, 100);
    Coins  = reader.Read(SharedProperties.Coins, 0);
    Bombs  = reader.Read(SharedProperties.Bombs, 0);
  }
}
