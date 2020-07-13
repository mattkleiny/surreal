using System;

namespace Surreal.Languages.Lexing {
  public sealed class LexingException : Exception {
    public LexingException() {
    }

    public LexingException(string message)
        : base(message) {
    }

    public LexingException(string message, Exception innerException)
        : base(message, innerException) {
    }
  }
}