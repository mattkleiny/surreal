using System;

namespace Surreal.Framework.Scenes.Entities.Components {
  public sealed class UnregisteredComponentException : Exception {
    public UnregisteredComponentException(string message)
        : base(message) {
    }
  }
}