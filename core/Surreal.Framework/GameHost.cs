using Surreal.Graphics;
using Surreal.Resources;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A callback for the main game loop.
/// </summary>
public delegate void TickDelegate(GameTime time, IGraphicsContext graphics);

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
  /// Initializes the game.
  /// </summary>
  void Initialize(GraphicsContext graphics);

  /// <summary>
  /// Executes the main game loop.
  /// </summary>
  void Tick(GameTime time, GraphicsContext graphics);
}

/// <summary>
/// A default implementation of <see cref="IGameHost"/>.
/// </summary>
public abstract class GameHost : IGameHost
{
  /// <summary>
  /// Creates a <see cref="GameHost"/> using an anonymous callback for the main game loop.
  /// </summary>
  public static GameHost Anonymous(TickDelegate tickDelegate)
  {
    return new AnonymousGameHost(tickDelegate);
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
  public virtual void Initialize(GraphicsContext graphics)
  {
  }

  /// <inheritdoc/>
  public abstract void Tick(GameTime time, GraphicsContext graphics);

  public void Dispose()
  {
    Events.UnsubscribeAll(this);
    Services.Dispose();
  }

  /// <summary>
  /// A <see cref="GameHost"/> using an anonymous callback for the main game loop.
  /// </summary>
  private sealed class AnonymousGameHost(TickDelegate tickDelegate) : GameHost
  {
    public override void Tick(GameTime time, GraphicsContext graphics)
    {
      tickDelegate.Invoke(time, graphics);
    }
  }
}
