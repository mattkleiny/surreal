namespace Mindustry.Modules.Core.Model.Resources {
  public sealed class ResourceStack {
    public ResourceType Type  { get; }
    public ushort       Count { get; set; }

    public ResourceStack(ResourceType type, ushort count) {
      Type  = type;
      Count = count;
    }
  }
}