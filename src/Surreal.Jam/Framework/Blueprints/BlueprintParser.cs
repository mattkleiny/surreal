using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sprache;

namespace Surreal.Framework.Blueprints {
  public sealed class Blueprint {
    public static Blueprint Parse(string raw) {
      return Parser.Blueprint.Parse(raw);
    }

    public IEnumerable<Section> Sections { get; }

    public Blueprint(IEnumerable<Section> sections) {
      Sections = sections;
    }

    public sealed class Prototype {
      public string Id   { get; }
      public string Name { get; }

      public Prototype(string id, string name) {
        Id   = id;
        Name = name;
      }
    }

    public sealed class Section {
      public string                 Id         { get; }
      public string                 Title      { get; }
      public IEnumerable<Prototype> Prototypes { get; }

      public Section(string id, string title, IEnumerable<Prototype> prototypes) {
        Id         = id;
        Title      = title;
        Prototypes = prototypes;
      }
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal static class Parser {
      public static readonly Parser<string> Identifier =
          Sprache.Parse.Letter.AtLeastOnce().Text().Token();

      public static readonly Parser<string> QuotedText = (
          from open in Sprache.Parse.Char('"')
          from content in Sprache.Parse.CharExcept('"').Many().Text()
          from close in Sprache.Parse.Char('"')
          select content
      ).Token();

      public static readonly Parser<Prototype> Prototype =
          from id in Identifier
          from prompt in QuotedText
          select new Prototype(id, prompt);

      public static readonly Parser<Section> Section =
          from id in Identifier
          from title in QuotedText
          from lbracket in Sprache.Parse.Char('[').Token()
          from questions in Prototype.Many()
          from rbracket in Sprache.Parse.Char(']').Token()
          select new Section(id, title, questions);

      public static readonly Parser<Blueprint> Blueprint =
          Section.Many().Select(sections => new Blueprint(sections)).End();
    }
  }
}