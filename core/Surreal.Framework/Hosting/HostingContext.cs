using System.Reflection.Metadata;
using Surreal.Hosting;

[assembly: MetadataUpdateHandler(typeof(HostingContext))]

namespace Surreal.Hosting;

/// <summary>
/// Permits hosting of a project inside of another context.
/// <para/>
/// This is mainly used to facilitate executing of a game inside of the editor.
/// </summary>
public abstract class HostingContext
{
  private static ThreadLocal<HostingContext?> ThreadLocalContext { get; } = new();

  /// <summary>
  /// The active <see cref="HostingContext"/>.
  /// </summary>
  public static HostingContext? Current
  {
    get => ThreadLocalContext.Value!;
    set => ThreadLocalContext.Value = value;
  }

  /// <summary>
  /// Invoked when the hosting context has been hot reloaded.
  /// </summary>
  public event Action? HotReloaded;

  /// <summary>
  /// Invoked when the hosting context has been cancelled.
  /// </summary>
  public event Action? Cancelled;

  /// <summary>
  /// The <see cref="IPlatformHost"/> for the hosting context.
  /// </summary>
  public abstract IPlatformHost PlatformHost { get; }

  /// <summary>
  /// Notifies the context that the game has started.
  /// </summary>
  public virtual void NotifyStarted()
  {
  }

  /// <summary>
  /// Notifies the context that the game has stopped.
  /// </summary>
  public virtual void NotifyStopped()
  {
  }

  /// <summary>
  /// Notifies the context that the game has been cancelled.
  /// </summary>
  public virtual void NotifyCancelled()
  {
    Cancelled?.Invoke();
  }

  /// <summary>
  /// Notifies the context that the game has been updated.
  /// </summary>
  internal static void UpdateApplication(Type[] updatedTypes)
  {
    Current?.HotReloaded?.Invoke();
  }
}
