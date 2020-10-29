namespace Surreal.Platform {
  public sealed record DesktopConfiguration {
    public string Title                   { get; init; } = "Surreal";
    public int?   Width                   { get; init; }
    public int?   Height                  { get; init; }
    public bool   IsResizable             { get; init; } = true;
    public bool   IsVsyncEnabled          { get; init; } = false;
    public bool   WaitForFirstFrame       { get; init; } = true;
    public bool   ShowFPSInTitle          { get; init; } = false;
    public bool   EnableGraphicsDebugging { get; init; } = false;

    public IDesktopWindow? CustomWindow { get; init; }
  }
}