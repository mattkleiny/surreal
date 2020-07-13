using System.Collections.Generic;

namespace Surreal.I18N {
  public delegate string TranslationProvider(string raw);

  public static class TranslationProviders {
    public static TranslationProvider Current { get; set; } = Identity();

    public static TranslationProvider Identity() {
      return raw => raw;
    }

    public static TranslationProvider LookupTable(IDictionary<string, string> dictionary) {
      return raw => dictionary.TryGetValue(raw, out var final) ? final : raw;
    }
  }
}