using Surreal.Assets;
using Surreal.IO;
using Surreal.Text;
using static Surreal.BlueprintSyntaxTree;

namespace Surreal;

/// <summary>Parser front-end for blueprint descriptors.</summary>
public sealed class BlueprintParser : Parser<BlueprintDeclaration>
{
  private static ImmutableHashSet<string> Keywords { get; } = new[] { "#include", "component", "attribute", "event", "#tag", "entity", "item", "override" }.ToImmutableHashSet();

  private readonly IncludeContext includeContext;

  public BlueprintParser()
    : this(IncludeContext.Static())
  {
  }

  public BlueprintParser(IAssetManager manager)
    : this(IncludeContext.FromAssets(manager))
  {
  }

  private BlueprintParser(IncludeContext includeContext)
  {
    this.includeContext = includeContext;
  }

  public override async ValueTask<BlueprintDeclaration> ParseAsync(string path, TextReader reader, CancellationToken cancellationToken = default)
  {
    var tokens  = await TokenizeAsync(Keywords, reader, cancellationToken);
    var context = new BlueprintParserContext(path, tokens);

    var declaration = context.ParseBlueprintDeclaration();

    declaration = await MergeIncludesAsync(declaration, cancellationToken);

    // TODO: validate the result?

    return declaration;
  }

  private async Task<BlueprintDeclaration> MergeIncludesAsync(BlueprintDeclaration declaration, CancellationToken cancellationToken = default)
  {
    var includedPaths = new HashSet<VirtualPath>();

    foreach (var include in declaration.Includes)
    {
      if (includedPaths.Add(include.Path))
      {
        var included = await includeContext.LoadAsync(this, include.Path, cancellationToken);

        declaration = declaration.MergeWith(included);
      }
    }

    return declaration;
  }

  /// <summary>Context for syntax parsing operations. This is a recursive descent style parser.</summary>
  private sealed class BlueprintParserContext : ParserContext
  {
    private readonly string path;

    public BlueprintParserContext(string path, IEnumerable<Token> tokens)
      : base(tokens)
    {
      this.path = path;
    }

    public BlueprintDeclaration ParseBlueprintDeclaration()
    {
      var nodes = new List<BlueprintSyntaxTree>();

      while (TryPeek(out var token))
      {
        var node = token.Type switch
        {
          TokenType.Keyword => ParseGlobalKeyword(),
          _                 => ParseNull(),
        };

        if (node != null)
        {
          nodes.Add(node);
        }
      }

      return new BlueprintDeclaration(path)
      {
        Includes   = nodes.OfType<IncludeStatement>().ToImmutableArray(),
        Archetypes = nodes.OfType<BlueprintArchetype>().ToImmutableArray(),
      };
    }

    private BlueprintSyntaxTree ParseGlobalKeyword()
    {
      var literal = ConsumeLiteral<string>(TokenType.Keyword);

      return literal switch
      {
        "#include" => ParseIncludeStatement(),
        "item"     => ParseArchetypeDeclaration(ArchetypeKind.Item),
        "entity"   => ParseArchetypeDeclaration(ArchetypeKind.Entity),

        _ => throw Error($"An unrecognized keyword was encountered: {literal}"),
      };
    }

    private IncludeStatement ParseIncludeStatement()
    {
      var path = ConsumeLiteral<string>(TokenType.String);

      Consume(TokenType.SemiColon);

      return new IncludeStatement(path);
    }

    private BlueprintArchetype ParseArchetypeDeclaration(ArchetypeKind kind)
    {
      var name      = ConsumeLiteral<string>(TokenType.Identifier);
      var baseTypes = ConsumeBaseTypeList();
      var block     = ConsumeStatementBlock();

      return new BlueprintArchetype(kind, name)
      {
        BaseTypes  = baseTypes,
        Tags       = block.OfType<TagDeclaration>().ToImmutableArray(),
        Attributes = block.OfType<AttributeDeclaration>().ToImmutableArray(),
        Components = block.OfType<ComponentDeclaration>().ToImmutableArray(),
        Events     = block.OfType<EventDeclaration>().ToImmutableArray(),
      };
    }

    private ImmutableArray<string> ConsumeBaseTypeList()
    {
      var baseTypes = ImmutableArray.CreateBuilder<string>();

      if (TryConsume(TokenType.Colon))
      {
        while (!TryPeek(TokenType.LeftBrace))
        {
          baseTypes.Add(ConsumeLiteral<string>(TokenType.Identifier));

          TryConsume(TokenType.Comma);
        }
      }

      return baseTypes.ToImmutable();
    }

    private List<BlueprintSyntaxTree> ConsumeStatementBlock()
    {
      var results = new List<BlueprintSyntaxTree>();

      Consume(TokenType.LeftBrace);

      while (!TryPeek(TokenType.RightBrace))
      {
        if (TryConsume(TokenType.Comment)) continue; // ignore comments

        results.Add(ParseLocalDeclaration());
      }

      Consume(TokenType.RightBrace);

      return results;
    }

    private BlueprintSyntaxTree ParseLocalDeclaration()
    {
      var literal = ConsumeLiteral<string>(TokenType.Keyword);

      return literal switch
      {
        "#tag"      => ParseTagDeclaration(),
        "attribute" => ParseAttributeDeclaration(),
        "component" => ParseComponentDeclaration(),
        "event"     => ParseEventDeclaration(),

        _ => throw Error($"An unrecognized keyword was encountered: {literal}"),
      };
    }

    private TagDeclaration ParseTagDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      return new TagDeclaration(name);
    }

    private AttributeDeclaration ParseAttributeDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      var parameters = ParseParameterList();
      var isOverride = ParseOverride();

      Consume(TokenType.SemiColon);

      return new AttributeDeclaration(name)
      {
        IsOverride = isOverride,
        Parameters = parameters,
      };
    }

    private ComponentDeclaration ParseComponentDeclaration()
    {
      var name = ConsumeLiteral<string>(TokenType.Identifier);

      var parameters = ParseParameterList();
      var isOverride = ParseOverride();

      Consume(TokenType.SemiColon);

      return new ComponentDeclaration(name)
      {
        IsOverride = isOverride,
        Parameters = parameters,
      };
    }

    private EventDeclaration ParseEventDeclaration()
    {
      var name       = ConsumeLiteral<string>(TokenType.Identifier);
      var parameters = ParseParameterList();

      Consume(TokenType.SemiColon);

      return new EventDeclaration(name)
      {
        Parameters = parameters,
      };
    }

    private ImmutableArray<Expression> ParseParameterList()
    {
      var parameters = ImmutableArray.CreateBuilder<Expression>();

      Consume(TokenType.LeftParenthesis);

      while (!TryPeek(TokenType.RightParenthesis))
      {
        if (TryConsumeLiteral(TokenType.String, out string @string))
        {
          parameters.Add(new Expression.Constant(@string));
          TryConsume(TokenType.Comma);
        }
        else if (TryConsumeLiteral(TokenType.Number, out decimal number))
        {
          parameters.Add(new Expression.Constant(number));
          TryConsume(TokenType.Comma);
        }
        else if (TryConsumeLiteral(TokenType.Identifier, out string identifier))
        {
          parameters.Add(new Expression.Identifier(identifier));
          TryConsume(TokenType.Comma);
        }
        else
        {
          throw Error("An unexpected parameter was encountered");
        }
      }

      Consume(TokenType.RightParenthesis);

      return parameters.ToImmutable();
    }

    private bool ParseOverride()
    {
      return TryConsumeLiteralIf(TokenType.Keyword, "override");
    }

    private BlueprintSyntaxTree? ParseNull()
    {
      Consume();

      return null;
    }
  }
}
