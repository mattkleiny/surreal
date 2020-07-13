namespace Surreal.Framework.Editing.Graphs {
  public interface IGraph {
  }

  public interface IGraphPin {
  }

  public interface IGraphConnector {
    IGraphPin From { get; }
    IGraphPin To   { get; }
  }
}