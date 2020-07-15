using System;

namespace Surreal.Languages.Parsing {
  public sealed class ParsingException : Exception {
    public ParsingException() {
    }

    public ParsingException(string message)
        : base(message) {
    }

    public ParsingException(string message, Exception innerException)
        : base(message, innerException) {
    }
  }
}