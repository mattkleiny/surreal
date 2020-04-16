namespace Surreal
{
  public interface IInstalledMod
  {
    IMod         Instance { get; }
    IModMetadata Metadata { get; }
  }
}
