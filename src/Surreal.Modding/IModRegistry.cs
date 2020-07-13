using System.ComponentModel.Design;

namespace Surreal {
  public interface IModRegistry {
    IServiceContainer Services { get; }
  }
}