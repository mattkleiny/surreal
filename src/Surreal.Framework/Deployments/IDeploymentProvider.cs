namespace Surreal.Framework.Deployments {
  public interface IDeploymentProvider {
    Deployment CurrentDeployment { get; }
  }
}