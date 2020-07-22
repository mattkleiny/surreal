using System;

namespace Surreal.Text.Lexing {
  public sealed class LexingException : Exception {
    public LexingException(string message, in TokenPosition position)
        : base(message) {
      Position = position;
    }

    public TokenPosition Position { get; }
  }
}