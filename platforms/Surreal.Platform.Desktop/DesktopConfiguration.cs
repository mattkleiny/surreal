namespace Surreal.Platform {
  public sealed record DesktopConfiguration {
    public string Title                   { get; set; } = "Surreal";
    public int?   Width                   { get; set; }
    public int?   Height                  { get; set; }
    public bool   IsResizable             { get; set; } = true;
    public bool   IsVsyncEnabled          { get; set; } = false;
    public bool   WaitForFirstFrame       { get; set; } = true;
    public bool   ShowFPSInTitle          { get; set; } = false;
    public bool   EnableGraphicsDebugging { get; set; } = false;

    public IDesktopWindow? CustomWindow { get; set; }
  }
}