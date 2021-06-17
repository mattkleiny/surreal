using System;

namespace Surreal.Framework.Scenes.Entities.Collections {
  internal sealed class InvalidSlotException : Exception {
    public InvalidSlotException(string message)
        : base(message) {
    }
  }
}