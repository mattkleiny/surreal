using System;

namespace Surreal.Data.VFS {
  public readonly struct Path {
    private const string SchemeSeparator = "://";

    public static Path Parse(string uri) => new(ParseScheme(uri), ParseTarget(uri));

    public Path(string scheme, string target) {
      Scheme = scheme;
      Target = target;
    }

    public string Scheme { get; }
    public string Target { get; }

    public static implicit operator Path(string uri) => Parse(uri);

    public override string ToString() => $"<{Scheme}:{Target}>";

    private static string ParseScheme(string uri) {
      if (uri.Contains(SchemeSeparator)) {
        return uri.Split(new[] {SchemeSeparator}, StringSplitOptions.RemoveEmptyEntries)[0];
      }

      return "local"; // no scheme on this URI; default to local file system
    }

    private static string ParseTarget(string uri) {
      if (uri.Contains(SchemeSeparator)) {
        return uri.Split(new[] {SchemeSeparator}, StringSplitOptions.RemoveEmptyEntries)[1];
      }

      return uri; // no scheme on this URI
    }
  }
}