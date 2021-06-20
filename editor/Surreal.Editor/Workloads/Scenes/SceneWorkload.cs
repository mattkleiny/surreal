namespace Surreal.Editor.Workloads.Scenes
{
  public interface ISceneDocument : IDocument
  {
  }

  public sealed class SceneWorkload : DocumentWorkload
  {
    public ISceneDocument Document { get; }

    public SceneWorkload(DocumentMediator mediator, ISceneDocument document)
        : base(mediator)
    {
      Document = document;
    }
  }
}