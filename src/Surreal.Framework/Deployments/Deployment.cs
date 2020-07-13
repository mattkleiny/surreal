using System;

namespace Surreal.Framework.Deployments {
  public sealed class Deployment {
    public static Deployment Current => throw new NotImplementedException();

    public bool    IsDevelopmentBuild { get; }
    public Version Version            { get; }

    public Deployment(bool isDevelopmentBuild, Version version) {
      IsDevelopmentBuild = isDevelopmentBuild;
      Version            = version;
    }
  }
}