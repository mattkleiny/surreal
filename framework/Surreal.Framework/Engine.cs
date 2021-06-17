using System;
using System.Diagnostics;
using System.Runtime;
using Surreal.Mathematics.Timing;
using Surreal.Platform;

namespace Surreal.Framework {
  public static class Engine {
    public static bool IsRunning { get; private set; }

    public static void Schedule(Action task) => throw new NotImplementedException();

    public static void Run(IPlatformHost host, params IFrameListener[] listeners) {
      if (IsRunning) {
        throw new InvalidOperationException("The engine is already running, and cannot start again!");
      }

      IsRunning = true;

      // switch to a low-latency mode to help ease frame rate issues from GC operation
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

      var clock = new Clock();

      while (IsRunning && !host.IsClosing) {
        var deltaTime = clock.Tick();

        host.Tick(deltaTime);

        for (var i = 0; i < listeners.Length; i++) {
          listeners[i].Tick(deltaTime);
        }
      }
    }

    public static void Stop() {
      IsRunning = false;
    }

    private sealed class Clock {
      private readonly Stopwatch stopwatch = Stopwatch.StartNew();

      public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();
      public TimeSpan MaxDeltaTime    { get; } = (16 * 10).Milliseconds();

      public DeltaTime Tick() {
        var delta = stopwatch.Elapsed;

        stopwatch.Restart();

        if (delta > MaxDeltaTime) {
          delta = TargetDeltaTime;
        }

        return new DeltaTime(delta);
      }
    }
  }
}