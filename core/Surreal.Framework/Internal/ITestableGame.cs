using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Surreal.Testing")]

namespace Surreal.Internal;

/// <summary>A hook to allow unit testing of game components.</summary>
internal interface ITestableGame
{
  ILoopTarget LoopTarget { get; }

  void Initialize(CancellationToken cancellationToken = default);
  void Run(CancellationToken cancellationToken = default);
}
