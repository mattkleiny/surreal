using System;

namespace Surreal.IO {
  public sealed class ResourceNotFoundException : Exception {
    public ResourceNotFoundException(string message)
        : base(message) {
    }
  }
}