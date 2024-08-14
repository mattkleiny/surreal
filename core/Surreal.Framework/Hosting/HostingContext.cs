using System.Reflection.Metadata;
using Surreal.Hosting;

[assembly: MetadataUpdateHandler(typeof(HostingContext))]

namespace Surreal.Hosting;

/// <summary>
/// Permits hosting of a project inside another context.
/// <para/>
/// This is mainly used to facilitate executing of a game inside the editor.
/// </summary>
public abstract class HostingContext
{
  private static ThreadLocal<HostingContext?> ThreadLocalContext { get; } = new();

  /// <summary>
  /// A no-op <see cref="HostingContext"/>.
  /// </summary>
  public static HostingContext Null { get; } = new NullHostingContext();

  /// <summary>
  /// The active <see cref="HostingContext"/>.
  /// </summary>
  public static HostingContext Current
  {
    get => ThreadLocalContext.Value ?? Null;
    set => ThreadLocalContext.Value = value;
  }

  public event Action? HotReloaded;
  public event Action? Cancelled;

  /// <summary>
  /// The <see cref="IPlatformHost"/> for the hosting context.
  /// </summary>
  public abstract IPlatformHost? PlatformHost { get; }

  public abstract void OnStarted();
  public abstract void OnStopped();

  public void NotifyCancelled()
  {
    Cancelled?.Invoke();
  }

  /// <summary>
  /// Notifies the application of a .NET hot reload.
  /// </summary>
  [UsedImplicitly]
  internal static void UpdateApplication(Type[] updatedTypes)
  {
    Current.HotReloaded?.Invoke();
  }

  /// <summary>
  /// A no-op <see cref="HostingContext"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullHostingContext : HostingContext
  {
    public override IPlatformHost? PlatformHost => null;

    public override void OnStarted()
    {
    }

    public override void OnStopped()
    {
    }
  }
}
