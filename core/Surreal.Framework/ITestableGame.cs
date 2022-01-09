using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Surreal.Testing")]

namespace Surreal;

/// <summary>A hook to allow unit testing of game components.</summary>
internal interface ITestableGame
{
  ILoopTarget LoopTarget { get; }

  ValueTask InitializeAsync(CancellationToken cancellationToken = default);
  ValueTask RunAsync(CancellationToken cancellationToken = default);
}
