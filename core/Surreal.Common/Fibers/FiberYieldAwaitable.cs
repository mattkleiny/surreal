using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Surreal.Fibers;

/// <summary>Allows yielding <see cref="FiberTask"/>s to the next scheduler invocation.</summary>
public readonly struct FiberYieldAwaitable
{
  [SuppressMessage(
    "ReSharper",
    "MemberCanBeMadeStatic.Global",
    Justification = "Required for the await pattern"
  )]
  public Awaiter GetAwaiter() => new();

  public readonly struct Awaiter : INotifyCompletion
  {
    public bool IsCompleted => false;

    public void GetResult()
    {
      // no-op
    }

    public void OnCompleted(Action continuation)
    {
      FiberScheduler.Schedule(continuation);
    }
  }
}
