namespace Surreal.Graphics.Experimental.Shady {
  public sealed class ShadyMetadata {
    public string           Name        { get; }
    public string           Description { get; }
    public ShadyProgramType Type        { get; }

    public ShadyMetadata(string name, string description, ShadyProgramType type) {
      Name        = name;
      Description = description;
      Type        = type;
    }
  }
}