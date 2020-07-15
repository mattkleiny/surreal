using System;

namespace Surreal.Graphics.Fonts {
  public readonly struct CharacterSet {
    private static readonly char[] StandardCharacters = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i'};

    public static CharacterSet None     => default;
    public static CharacterSet Standard => new CharacterSet(StandardCharacters);

    private readonly char[] characters;

    public CharacterSet(char[] characters) {
      this.characters = characters;
    }

    public int Length => characters?.Length ?? 0;

    public char this[Index index] => characters[index];
    public Span<char> this[Range range] => characters[range];
  }
}