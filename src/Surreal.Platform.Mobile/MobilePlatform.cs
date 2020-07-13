using System;

namespace Surreal.Platform {
  public sealed class MobilePlatform : IPlatform {
    public MobileConfiguration Configuration { get; } = new MobileConfiguration();

    public IPlatformHost BuildHost() {
      if (Configuration.Application == null) {
        throw new Exception("Mobile application was not provided!");
      }

      return new MobilePlatformHost(Configuration.Application);
    }
  }
}