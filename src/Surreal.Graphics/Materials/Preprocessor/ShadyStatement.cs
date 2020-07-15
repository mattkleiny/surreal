using System.Collections.Generic;

namespace Surreal.Graphics.Materials.Preprocessor {
  internal abstract class ShadyStatement {
    public abstract T Accept<T>(Visitor<T> visitor);

    public abstract class Visitor<T> {
      public virtual T Visit(MetadataDeclaration statement) => default!;
      public virtual T Visit(ShaderDeclaration statement)   => default!;
      public virtual T Visit(UniformDefinition statement)   => default!;
      public virtual T Visit(FunctionDefinition statement)  => default!;
    }

    public sealed class MetadataDeclaration : ShadyStatement {
      public ShadyMetadata Metadata { get; }

      public MetadataDeclaration(ShadyMetadata metadata) {
        Metadata = metadata;
      }

      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class ShaderDeclaration : ShadyStatement {
      public ShaderType                  Type       { get; }
      public IEnumerable<ShadyStatement> Statements { get; }

      public ShaderDeclaration(ShaderType type, IEnumerable<ShadyStatement> statements) {
        Type       = type;
        Statements = statements;
      }

      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class UniformDefinition : ShadyStatement {
      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class FunctionDefinition : ShadyStatement {
      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }
  }
}