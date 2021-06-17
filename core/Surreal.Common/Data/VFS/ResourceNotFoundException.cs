using System;

namespace Surreal.Data.VFS {
  public sealed class ResourceNotFoundException : Exception {
    public ResourceNotFoundException(string message)
        : base(message) {
    }
  }
}