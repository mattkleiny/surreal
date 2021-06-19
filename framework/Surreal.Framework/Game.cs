using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Diagnostics.Profiling;
using Surreal.Fibers;
using Surreal.IO;
using Surreal.Platform;
using Surreal.Services;
using Surreal.Timing;

namespace Surreal.Framework
{
  public abstract class Game : IDisposable, IFrameListener
  {
    public static Game Current { get; private set; } = null!;

    private static readonly IProfiler Profiler = ProfilerFactory.GetProfiler<Game>();

    private readonly DateTime    startTime = DateTime.Now;
    private readonly ILoopTarget loopTarget;

    public static void Start<TGame>(Configuration configuration)
        where TGame : Game, new()
    {
      using var host = configuration.Platform!.BuildHost();
      using var game = new TGame();

      game.Initialize(host);

      Engine.Run(host, game);
    }

    protected Game()
    {
      Current    = this;
      loopTarget = new ProfiledLoopTarget(this);
    }

    public IPlatformHost     Host         { get; private set; } = null!;
    public IAssetManager     Assets       { get; }              = new AssetManager();
    public IServiceContainer Services     { get; }              = new ServiceContainer();
    public List<IGamePlugin> Plugins      { get; }              = new();
    public ILoopStrategy     LoopStrategy { get; set; }         = new AveragingLoopStrategy();

    protected internal void Initialize(IPlatformHost host)
    {
      Host = host;

      Initialize();
    }

    protected virtual void Initialize()
    {
      Host.Resized += OnResized;

      RegisterFileSystems(FileSystems.Registry);
      RegisterServices(Services);

      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Initialize();
      }

      LoadContentAsync(Assets).Forget();
    }

    protected virtual async FiberTask LoadContentAsync(IAssetResolver assets)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        await Plugins[i].LoadContentAsync(assets);
      }
    }

    protected virtual void RegisterServices(IServiceContainer services)
    {
      services.AddService(Assets);
      services.AddService(Host);
    }

    protected virtual void RegisterFileSystems(IFileSystemRegistry registry)
    {
      if (Host.Services.TryGetService(out IFileSystem platformFileSystem))
      {
        registry.Add(platformFileSystem);
      }

      registry.Add(new ResourceFileSystem());
    }

    void IFrameListener.Tick(DeltaTime deltaTime)
    {
      var totalTime = DateTime.Now - startTime;

      LoopStrategy.Tick(loopTarget, deltaTime, totalTime);
    }

    protected virtual void Begin(GameTime time)
    {
    }

    protected virtual void Input(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Input(time);
      }
    }

    protected virtual void Update(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Update(time);
      }
    }

    protected virtual void Draw(GameTime time)
    {
      for (var i = 0; i < Plugins.Count; i++)
      {
        Plugins[i].Draw(time);
      }
    }

    protected virtual void End(GameTime time)
    {
    }

    protected virtual void OnResized(int width, int height)
    {
    }

    public void Exit()
    {
      Engine.Stop();
    }

    public virtual void Dispose()
    {
      foreach (var plugin in Plugins)
      {
        plugin.Dispose();
      }

      if (Services is IDisposable services)
      {
        services.Dispose();
      }
    }

    public interface ILoopStrategy
    {
      GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime);
    }

    public sealed class ChillLoopStrategy : ILoopStrategy
    {
      public ChillLoopStrategy(ILoopStrategy strategy)
      {
        Strategy = strategy;
      }

      public ILoopStrategy Strategy         { get; }
      public TimeSpan      MaxSleepInterval { get; set; } = 16.Milliseconds();

      public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
      {
        var time = Strategy.Tick(target, deltaTime, totalTime);

        if (!time.IsRunningSlowly)
        {
          var sleepTime = MaxSleepInterval - time.DeltaTime;
          if (sleepTime.TotalMilliseconds > 0f)
          {
            using (Profiler.Track("Chilling"))
            {
              Thread.Sleep(sleepTime);
            }
          }
        }

        return time;
      }
    }

    public sealed class AveragingLoopStrategy : ILoopStrategy
    {
      private readonly RingBuffer<TimeSpan> samples;

      public AveragingLoopStrategy(int samples = 10)
      {
        Debug.Assert(samples > 0, "samples > 0");

        this.samples = new RingBuffer<TimeSpan>(samples);
      }

      public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();

      public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
      {
        samples.Add(deltaTime);

        var averagedDeltaTime = samples.FastAverage();

        var time = new GameTime(
            deltaTime: averagedDeltaTime,
            totalTime: totalTime,
            isRunningSlowly: averagedDeltaTime > TargetDeltaTime
        );

        target.Begin(time);
        target.Input(time);
        target.Update(time);
        target.Draw(time);
        target.End(time);

        return time;
      }
    }

    public sealed class VariableStepLoopStrategy : ILoopStrategy
    {
      private readonly DateTime startTime = DateTime.Now;

      public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

      public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
      {
        var time = new GameTime(
            deltaTime: deltaTime,
            totalTime: DateTime.Now - startTime,
            isRunningSlowly: deltaTime > TargetDeltaTime
        );

        target.Begin(time);
        target.Input(time);
        target.Update(time);
        target.Draw(time);
        target.End(time);

        return time;
      }
    }

    public sealed class FixedStepLoopStrategy : ILoopStrategy
    {
      private double accumulator;

      public TimeSpan Step            { get; set; } = 16.Milliseconds();
      public TimeSpan TargetDeltaTime { get; set; } = 16.Milliseconds();

      public GameTime Tick(ILoopTarget target, DeltaTime deltaTime, TimeSpan totalTime)
      {
        var time = new GameTime(
            deltaTime: deltaTime,
            totalTime: totalTime,
            isRunningSlowly: deltaTime > TargetDeltaTime
        );

        accumulator += deltaTime.TimeSpan.TotalSeconds;

        target.Begin(time);
        target.Input(time);

        while (accumulator >= Step.TotalSeconds)
        {
          var stepTime = new GameTime(
              deltaTime: Step,
              totalTime: time.TotalTime,
              isRunningSlowly: time.IsRunningSlowly
          );

          target.Update(stepTime);

          accumulator -= Step.TotalSeconds;
        }

        target.Draw(time);
        target.End(time);

        return time;
      }
    }

    public interface ILoopTarget
    {
      void Begin(GameTime time);
      void Input(GameTime time);
      void Update(GameTime time);
      void Draw(GameTime time);
      void End(GameTime time);
    }

    private sealed class ProfiledLoopTarget : ILoopTarget
    {
      private readonly Game game;

      public ProfiledLoopTarget(Game game)
      {
        this.game = game;
      }

      public void Begin(GameTime time)
      {
        using var _ = Profiler.Track(nameof(Begin));

        game.Begin(time);
      }

      public void Input(GameTime time)
      {
        using var _ = Profiler.Track(nameof(Input));

        game.Input(time);
      }

      public void Update(GameTime time)
      {
        using var _ = Profiler.Track(nameof(Update));

        game.Update(time);
      }

      public void Draw(GameTime time)
      {
        using var _ = Profiler.Track(nameof(Draw));

        game.Draw(time);
      }

      public void End(GameTime time)
      {
        using var _ = Profiler.Track(nameof(End));

        game.End(time);
      }
    }

    public sealed class Configuration
    {
      public IPlatform? Platform { get; init; }
    }
  }
}