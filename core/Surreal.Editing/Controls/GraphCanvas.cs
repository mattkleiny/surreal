using Avalonia.Controls;
using Surreal.Graphs;

namespace Surreal.Controls;

/// <summary>A <see cref="Control"/> which renders a graph canvas for editing data.</summary>
public abstract class GraphCanvas : Control
{
  protected abstract IGraphNodeProvider    Provider { get; }
  protected abstract IGraphLayoutAlgorithm Layout   { get; }

  protected interface IGraphLayoutAlgorithm
  {
  }
}

/// <summary>A <see cref="GraphCanvas"/> that renders a fixed topological graph.</summary>
public sealed class FixedGraphCanvas : GraphCanvas
{
  protected override IGraphNodeProvider    Provider => (IGraphNodeProvider) DataContext!;
  protected override IGraphLayoutAlgorithm Layout   => new FixedGraphLayout();

  private sealed class FixedGraphLayout : IGraphLayoutAlgorithm
  {
  }
}
