namespace Surreal.Editor.Workloads
{
  public interface IDocumentWorkload
  {
    DocumentMediator Mediator { get; }
  }

  public abstract class DocumentWorkload : IDocumentWorkload
  {
    protected DocumentWorkload(DocumentMediator mediator)
    {
      Mediator = mediator;
    }

    public DocumentMediator Mediator { get; }
  }
}