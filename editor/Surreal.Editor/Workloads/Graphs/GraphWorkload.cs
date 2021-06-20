namespace Surreal.Editor.Workloads.Graphs
{
  public interface IGraphDocument : IDocument
  {
  }

  public sealed class GraphWorkload : DocumentWorkload
  {
    public IGraphDocument Document { get; }

    public GraphWorkload(DocumentMediator mediator, IGraphDocument document)
        : base(mediator)
    {
      Document = document;
    }
  }
}