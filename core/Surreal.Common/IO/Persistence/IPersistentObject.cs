namespace Surreal.IO.Persistence;

/// <summary>Identifies an object with persistent data.</summary>
public interface IPersistentObject
{
  Guid Id { get; }

  void OnPersistState(PersistenceContext context, IPersistenceWriter writer);
  void OnResumeState(PersistenceContext context, IPersistenceReader reader);
}

/// <summary>Static extensions for <see cref="IPersistentObject"/>s.</summary>
public static class PersistentObjectExtensions
{
  public static void Persist(this IPersistentObject persistent, IPersistenceStore store, PersistenceMode mode = PersistenceMode.Transient)
  {
    var context = new PersistenceContext
    {
      Mode = mode,
    };

    var writer = store.CreateWriter(persistent.Id);

    persistent.OnPersistState(context, writer);
  }

  public static void Resume(this IPersistentObject persistent, IPersistenceStore store, PersistenceMode mode = PersistenceMode.Transient)
  {
    var context = new PersistenceContext
    {
      Mode = mode,
    };

    if (store.CreateReader(persistent.Id, out var reader))
    {
      persistent.OnResumeState(context, reader);
    }
  }
}
