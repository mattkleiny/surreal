using Isaac;
using Surreal.Framework;
using Surreal.Platform;

namespace IsaacEditor {
  public sealed class IsaacEditor : Editor<IsaacGame> {
    public static void Main() => Start<IsaacEditor>(new Configuration {
        Platform = new DesktopPlatform {
            Configuration = {
                Title          = "The Binding of Isaac - Editor",
                IsVsyncEnabled = true,
                ShowFPSInTitle = true
            }
        }
    });
  }
}