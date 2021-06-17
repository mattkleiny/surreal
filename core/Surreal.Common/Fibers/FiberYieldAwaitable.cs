using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Surreal.Fibers {
  /// <summary>An awaitable yield operation for <see cref="FiberTask"/>s.</summary>
  public readonly struct FiberYieldAwaitable {
    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    public Awaiter GetAwaiter() => new();

    public readonly struct Awaiter : INotifyCompletion {
      public bool IsCompleted => false;

      public void GetResult() {
        // no-op
      }

      public void OnCompleted(Action continuation) {
        FiberScheduler.Schedule(continuation);
      }
    }
  }
}