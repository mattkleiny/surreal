using System;
using System.Runtime;
using Surreal.Fibers;
using Surreal.Platform;
using Surreal.Timing;

namespace Surreal.Framework
{
  public static class Engine
  {
    public static bool IsRunning { get; private set; }

    public static void Run(IPlatformHost host, params IFrameListener[] listeners)
    {
      if (IsRunning)
      {
        throw new InvalidOperationException("The engine is already running, and cannot start again!");
      }

      try
      {
        GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

        IsRunning = true;

        var stopwatch = new Stopwatch();

        while (IsRunning && !host.IsClosing)
        {
          var deltaTime = stopwatch.Tick();

          host.Tick(deltaTime);

          for (var i = 0; i < listeners.Length; i++)
          {
            listeners[i].Tick(deltaTime);
          }

          FiberScheduler.Tick();
        }
      }
      finally
      {
        IsRunning = false;
      }
    }

    public static void Stop()
    {
      IsRunning = false;
    }
  }
}