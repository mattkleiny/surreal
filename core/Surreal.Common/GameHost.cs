using Surreal.Resources;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// Contextual services for a game.
/// </summary>
public interface IGameHost : IDisposable
{
  /// <summary>
  /// The top-level <see cref="IEventBus"/> for the game.
  /// </summary>
  IEventBus Events { get; }

  /// <summary>
  /// The top-level <see cref="IResourceProvider"/> for the game.
  /// </summary>
  IResourceProvider Resources { get; }

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> for the game.
  /// </summary>
  IServiceRegistry Services { get; }

  /// <summary>
  /// True if the host wants to close.
  /// </summary>
  bool IsClosing { get; }

  /// <summary>
  /// Executes the main game loop.
  /// </summary>
  void Tick(GameTime time);
}

/// <summary>
/// A default implementation of <see cref="IGameHost"/>.
/// </summary>
public abstract class GameHost : IGameHost
{
  /// <summary>
  /// A callback for setting up a game.
  /// </summary>
  public delegate GameTickDelegate GameSetupDelegate();

  /// <summary>
  /// A callback for ticking a game.
  /// </summary>
  public delegate void GameTickDelegate(GameTime time);

  /// <summary>
  /// Creates a new <see cref="GameHost"/> instance using a delegate-based approach.
  /// </summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static GameHost Create(GameSetupDelegate setupDelegate)
  {
    return new DelegateGameHost(setupDelegate);
  }

  protected GameHost()
  {
    Events.SubscribeAll(this);
  }

  /// <inheritdoc/>
  public IEventBus Events { get; } = new EventBus();

  /// <inheritdoc/>
  public IResourceProvider Resources => throw new NotImplementedException();

  /// <inheritdoc/>
  public IServiceRegistry Services { get; } = new ServiceRegistry();

  /// <inheritdoc/>
  public bool IsClosing => false;

  /// <inheritdoc/>
  public abstract void Tick(GameTime time);

  public virtual void Dispose()
  {
    Events.UnsubscribeAll(this);
    Services.Dispose();
  }

  /// <summary>
  /// An anonymous <see cref="GameHost"/> implementation.
  /// </summary>
  private sealed class DelegateGameHost(GameSetupDelegate setupDelegate) : GameHost
  {
    private GameTickDelegate? _tickDelegate;

    public override void Tick(GameTime time)
    {
      _tickDelegate ??= setupDelegate();
      _tickDelegate(time);
    }
  }
}
