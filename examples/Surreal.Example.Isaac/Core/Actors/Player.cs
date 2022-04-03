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
    writer.Write(Core.Properties.Health, Health);
    writer.Write(Core.Properties.Coins, Coins);
    writer.Write(Core.Properties.Bombs, Bombs);
  }

  void IPersistentObject.OnResumeState(PersistenceContext context, IPersistenceReader reader)
  {
    Health = reader.Read(Core.Properties.Health, 100);
    Coins = reader.Read(Core.Properties.Coins, 0);
    Bombs = reader.Read(Core.Properties.Bombs, 0);
  }
}
