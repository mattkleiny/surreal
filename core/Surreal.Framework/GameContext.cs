using System.Reflection.Metadata;
using Surreal;

[assembly: MetadataUpdateHandler(typeof(GameContext))]

namespace Surreal;

/// <summary>
/// Permits hosting of a <see cref="Game"/> inside another context.
/// <para/>
/// This is mainly used to facilitate executing of a game inside the editor.
/// </summary>
public abstract class GameContext
{
  private static ThreadLocal<GameContext?> ThreadLocalContext { get; } = new();

  /// <summary>
  /// A no-op <see cref="GameContext"/>.
  /// </summary>
  public static GameContext Null { get; } = new NullHostingContext();

  /// <summary>
  /// The active <see cref="GameContext"/>.
  /// </summary>
  public static GameContext Current
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

  /// <summary>
  /// Posts the given action to be run on the platform host's main thread.
  /// </summary>
  public abstract void PostOnMainThread(Action action);

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
  /// A no-op <see cref="GameContext"/>.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullHostingContext : GameContext
  {
    public override IPlatformHost? PlatformHost => null;

    public override void PostOnMainThread(Action action)
    {
      action.Invoke(); // no-op; run immediately.
    }
  }
}
