namespace Surreal.Diagnostics.Logging {
  public interface ILogFactory {
    ILog GetLog(string category);
  }
}