﻿using Surreal.Timing;

namespace Surreal;

/// <summary>Represents a <see cref="Game"/>  that is being unit or integration tested.</summary>
public interface IGameUnderTest : IDisposable
{
  Game        Instance   { get; }
  ILoopTarget LoopTarget { get; }

  Task InitializeAsync();
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

  public TGame Instance { get; } = Game.Create<TGame>(new Game.Configuration
  {
    Platform = new HeadlessPlatform()
  });

  public ILoopTarget LoopTarget => Instance.LoopTarget;

  public Task InitializeAsync()
  {
    return Instance.InitializeAsync();
  }

  public void Tick(DeltaTime deltaTime)
  {
    var gameTime = new GameTime(
      DeltaTime: deltaTime,
      TotalTime: TimeStamp.Now - startTime,
      IsRunningSlowly: false
    );

    LoopTarget.Begin(gameTime);
    LoopTarget.Input(gameTime);
    LoopTarget.Update(gameTime);
    LoopTarget.Draw(gameTime);
    LoopTarget.End(gameTime);
  }

  public void Dispose()
  {
    Instance.Dispose();
  }

  Game IGameUnderTest.Instance => Instance;
}