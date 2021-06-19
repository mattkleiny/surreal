using System;
using System.Text.RegularExpressions;

namespace Surreal.Text.Parsing
{
  public class RegexLexer<TToken> : StringLexer<TToken>
      where TToken : struct
  {
    private readonly Rule[] rules;

    public RegexLexer(params Rule[] rules)
    {
      this.rules = rules;
    }

    protected override bool TryMatch(
        ReadOnlySpan<char> characters,
        TokenPosition position,
        out TToken token,
        out int length,
        out bool ignore)
    {
      token  = default;
      length = 0;
      ignore = false;

      foreach (var rule in rules)
      {
        // TODO: replace this .ToString() with a non-allocating version, if it ever becomes available
        var match = rule.Regex.Match(characters.ToString(), position.Column);

        if (match.Success && match.Index - position.Column == 0)
        {
          token  = rule.Tokenizer(match.Value, position);
          length = match.Length;
          ignore = rule.Disregard;

          return true;
        }
      }

      return false;
    }

    public sealed class Rule
    {
      public Regex     Regex     { get; }
      public Tokenizer Tokenizer { get; }
      public bool      Disregard { get; }

      public Rule(string pattern, Tokenizer tokenizer, bool disregard = false)
      {
        Regex     = new Regex(pattern, RegexOptions.Compiled);
        Tokenizer = tokenizer;
        Disregard = disregard;
      }
    }

    public delegate TToken Tokenizer(string lexeme, TokenPosition position);
  }
}