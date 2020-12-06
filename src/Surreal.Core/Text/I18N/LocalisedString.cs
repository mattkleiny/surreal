using System;
using System.Diagnostics;

namespace Surreal.Text.I18N {
  [DebuggerDisplay("{Raw}: ({Final})")]
  public readonly struct LocalisedString : IEquatable<LocalisedString> {
    public static string T(string message) => TranslationProviders.Current(message);

    private LocalisedString(string raw) {
      Raw = raw;
    }

    public readonly string Raw;
    public          string Final => T(Raw);

    public override int    GetHashCode() => Raw?.GetHashCode() ?? 0;
    public override string ToString()    => Final;

    public          bool Equals(LocalisedString other) => string.Equals(Raw, other.Raw);
    public override bool Equals(object obj)            => obj is LocalisedString other && Equals(other);

    public static implicit operator LocalisedString(string raw)       => new(raw);
    public static implicit operator string(LocalisedString localised) => localised.Final;
  }
}