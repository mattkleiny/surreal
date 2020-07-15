using System.Collections.Generic;

namespace Surreal.Graphics.Materials.Shady {
  internal abstract class ShadyStatement {
    public abstract T Accept<T>(Visitor<T> visitor);

    public abstract class Visitor<T> {
      public virtual T Visit(MetadataDecl statement) => default!;
      public virtual T Visit(ShaderDecl statement)   => default!;
      public virtual T Visit(UniformDecl statement)  => default!;
      public virtual T Visit(FunctionDecl statement) => default!;
    }

    public sealed class MetadataDecl : ShadyStatement {
      public ShadyMetadata Metadata { get; }

      public MetadataDecl(ShadyMetadata metadata) {
        Metadata = metadata;
      }

      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class ShaderDecl : ShadyStatement {
      public ShaderType                  Type       { get; }
      public IEnumerable<ShadyStatement> Statements { get; }

      public ShaderDecl(ShaderType type, IEnumerable<ShadyStatement> statements) {
        Type       = type;
        Statements = statements;
      }

      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class UniformDecl : ShadyStatement {
      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }

    public sealed class FunctionDecl : ShadyStatement {
      public override T Accept<T>(Visitor<T> visitor) => visitor.Visit(this);
    }
  }
}