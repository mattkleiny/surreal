using System;

namespace Surreal.IO.VFS {
  public sealed class ResourceNotFoundException : Exception {
    public ResourceNotFoundException(string message)
        : base(message) {
    }
  }
}