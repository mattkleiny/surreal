using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using Surreal.Fibers;
using Surreal.Platform;
using Surreal.Timing;

namespace Surreal.Framework {
  public static class Engine {
    [ModuleInitializer]
    internal static void Initialize() {
      GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
    }

    public static bool IsRunning { get; private set; }

    public static void Run(IPlatformHost host, params IFrameListener[] listeners) {
      if (IsRunning) {
        throw new InvalidOperationException("The engine is already running, and cannot start again!");
      }

      var clock = new Clock();

      IsRunning = true;

      while (IsRunning && !host.IsClosing) {
        var deltaTime = clock.Tick();

        host.Tick(deltaTime);

        for (var i = 0; i < listeners.Length; i++) {
          listeners[i].Tick(deltaTime);
        }

        FiberScheduler.Tick();
      }

      IsRunning = false;
    }

    public static void Stop() {
      IsRunning = false;
    }

    private sealed class Clock {
      private TimeStamp lastTime = TimeStamp.Now;

      public TimeSpan TargetDeltaTime { get; } = 16.Milliseconds();
      public TimeSpan MaxDeltaTime    { get; } = (16 * 10).Milliseconds();

      public DeltaTime Tick() {
        var now   = TimeStamp.Now;
        var delta = now - lastTime;

        if (delta > MaxDeltaTime) {
          delta = TargetDeltaTime;
        }

        lastTime = now;

        return new DeltaTime(delta);
      }
    }
  }
}