using Surreal.Assets;
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
  /// The top-level <see cref="AssetManager"/> for the game.
  /// </summary>
  AssetManager Assets { get; }

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> for the game.
  /// </summary>
  IServiceRegistry Services { get; }

  /// <summary>
  /// True if the host wants to close.
  /// </summary>
  bool IsClosing { get; }

  /// <summary>
  /// Initialize the game.
  /// </summary>
  /// <param name="configuration"></param>
  void Initialize(GameConfiguration configuration);

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
  /// A callback for ticking a game.
  /// </summary>
  public delegate void GameTickDelegate(GameTime time);

  /// <summary>
  /// Creates a new <see cref="GameHost"/> instance using a delegate-based approach.
  /// </summary>
  public static GameHost Create(Func<GameTickDelegate> setupDelegate)
  {
    return Create(new FrameworkModule(), setupDelegate);
  }

  /// <summary>
  /// Creates a new <see cref="GameHost"/> instance using a delegate-based approach.
  /// </summary>
  public static GameHost Create(IServiceModule module, Func<GameTickDelegate> setupDelegate)
  {
    return Create(module, () => Task.FromResult(setupDelegate()));
  }

  /// <summary>
  /// Creates a new <see cref="GameHost"/> instance using a delegate-based approach.
  /// </summary>
  public static GameHost Create(Func<Task<GameTickDelegate>> setupDelegate)
  {
    return Create(new FrameworkModule(), setupDelegate);
  }

  /// <summary>
  /// Creates a new <see cref="GameHost"/> instance using a delegate-based approach.
  /// </summary>
  public static GameHost Create(IServiceModule module, Func<Task<GameTickDelegate>> setupDelegate)
  {
    return new DelegateGameHost(module, setupDelegate);
  }

  protected GameHost(IServiceModule module)
  {
    Services = new ServiceRegistry();
    Services.AddModule(module);
  }

  /// <inheritdoc/>
  public IEventBus Events { get; } = new EventBus();

  /// <inheritdoc/>
  public AssetManager Assets { get; } = new();

  /// <inheritdoc/>
  public IServiceRegistry Services { get; }

  /// <inheritdoc/>
  public bool IsClosing => false;

  /// <inheritdoc/>
  public abstract void Initialize(GameConfiguration configuration);

  /// <inheritdoc/>
  public abstract void Tick(GameTime time);

  public virtual void Dispose()
  {
    Assets.Dispose();

    if (Services is IDisposable disposable)
    {
      disposable.Dispose();
    }
  }

  /// <summary>
  /// An anonymous <see cref="GameHost"/> implementation.
  /// </summary>
  private sealed class DelegateGameHost(IServiceModule module, Func<Task<GameTickDelegate>> setupDelegate) : GameHost(module)
  {
    private GameTickDelegate? _tickDelegate;

    public override void Initialize(GameConfiguration configuration)
    {
      foreach (var loader in Services.GetServices<IAssetLoader>())
      {
        Assets.AddLoader(loader);
      }

      _tickDelegate = setupDelegate().Result;
    }

    public override void Tick(GameTime time)
    {
      _tickDelegate!(time);
    }
  }
}
