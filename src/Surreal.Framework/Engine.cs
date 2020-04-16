using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Surreal.Collections;
using Surreal.Fibers;
using Surreal.Platform;
using Surreal.Timing;

namespace Surreal.Framework
{
  public static class Engine
  {
    private static readonly ActionQueue    DefaultActionQueue    = new ActionQueue();
    private static readonly FiberScheduler DefaultFiberScheduler = new FiberScheduler();

    public static bool IsRunning { get; private set; }

    public static void  Schedule(Action task)                          => DefaultActionQueue.Enqueue(task);
    public static Fiber Schedule(Func<Task> method)                    => Fiber.Start(DefaultFiberScheduler, _ => method());
    public static Fiber Schedule(Func<CancellationToken, Task> method) => Fiber.Start(DefaultFiberScheduler, method);

    public static void Run(IPlatformHost host, params IFrameListener[] listeners)
    {
      if (IsRunning)
      {
        throw new InvalidOperationException("The engine is already running, and cannot start again!");
      }

      IsRunning = true;

      // switch to a low-latency mode to help ease frame rate issues from GC operation
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

      var clock = new Clock();

      while (IsRunning && !host.IsClosing)
      {
        DefaultActionQueue.Execute();

        var deltaTime = clock.Tick();

        host.Tick(deltaTime);

        for (var i = 0; i < listeners.Length; i++)
        {
          listeners[i].Tick(deltaTime);
        }

        DefaultFiberScheduler.Run();
      }
    }

    public static void Stop()
    {
      IsRunning = false;
    }

    private sealed class Clock
    {
      private readonly Stopwatch stopwatch = Stopwatch.StartNew();

      public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();
      public TimeSpan MaxDeltaTime    { get; } = (16 * 10).Milliseconds();

      public DeltaTime Tick()
      {
        var delta = stopwatch.Elapsed;

        stopwatch.Restart();

        if (delta > MaxDeltaTime)
        {
          delta = TargetDeltaTime;
        }

        return new DeltaTime(delta);
      }
    }
  }
}