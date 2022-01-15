using Surreal.Internal;
using Surreal.Timing;

namespace Surreal;

/// <summary>Represents a <see cref="Game"/>  that is being unit or integration tested.</summary>
public interface IGameUnderTest : IDisposable
{
  Game        Instance   { get; }
  ILoopTarget LoopTarget { get; }

  ValueTask InitializeAsync();
  ValueTask RunAsync(TimeSpan duration);

  void Tick();
  void Tick(DeltaTime deltaTime);
}

/// <summary>Represents a <see cref="TGame"/> that is being unit or integration tested.</summary>
public interface IGameUnderTest<out TGame> : IGameUnderTest
{
  new TGame Instance { get; }
}

/// <summary>The default <see cref="IGameUnderTest{TGame}"/> implementation.</summary>
internal sealed class GameUnderTest<TGame> : IGameUnderTest<TGame>
  where TGame : Game, ITestableGame, new()
{
  private readonly TimeStamp startTime = TimeStamp.Now;

  public GameUnderTest(IPlatform platform, Action<IServiceRegistry> serviceOverrides)
  {
    Instance = Game.Create<TGame>(new Game.Configuration
    {
      Platform         = platform,
      ServiceOverrides = serviceOverrides,
    });
  }

  public TGame       Instance   { get; }
  public ILoopTarget LoopTarget => Instance.LoopTarget;

  public ValueTask InitializeAsync()
  {
    return Instance.InitializeAsync();
  }

  public ValueTask RunAsync(TimeSpan duration)
  {
    using var cancellationToken = new CancellationTokenSource(duration);

    return Instance.RunAsync(cancellationToken.Token);
  }

  public void Tick()
  {
    Tick(16.Milliseconds());
  }

  public void Tick(DeltaTime deltaTime)
  {
    var gameTime = new GameTime(
      DeltaTime: deltaTime,
      TotalTime: TimeStamp.Now - startTime,
      IsRunningSlowly: false
    );

    LoopTarget.BeginFrame(gameTime);
    LoopTarget.Input(gameTime);
    LoopTarget.Update(gameTime);
    LoopTarget.Draw(gameTime);
    LoopTarget.EndFrame(gameTime);
  }

  public void Dispose()
  {
    Instance.Dispose();
  }

  Game IGameUnderTest.Instance => Instance;
}
