using System;

namespace Surreal.Framework.Deployments
{
  [AttributeUsage(AttributeTargets.Assembly)]
  public sealed class DeploymentProviderAttribute : Attribute
  {
    public Type ProviderType { get; }

    public DeploymentProviderAttribute(Type providerType)
    {
      ProviderType = providerType;
    }
  }
}