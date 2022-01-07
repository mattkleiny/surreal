using Surreal.Text;

namespace Surreal.Internal.Graphics.Utilities;

/// <summary>A utility for building shader programs from raw source text.</summary>
internal sealed record ShaderCodeBuilder(StringBuilder Builder, int IndentLevel = 0) : IDisposable
{
  public void AppendLine(string raw) => Builder.AppendLine(raw);
  public void AppendBlankLine()      => Builder.AppendLine();

  public void AppendComment(string text)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append("/* ")
      .Append(text)
      .AppendLine(" */");
  }

  public void AppendUniformDeclaration(string? precision, string type, string name)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append("uniform ")
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(type)
      .Append(' ')
      .Append(name)
      .AppendLine(";");
  }

  public void AppendVaryingDeclaration(string? precision, string type, string name)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append("varying ")
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(type)
      .Append(' ')
      .Append(name)
      .AppendLine(";");
  }

  public void AppendConstantDeclaration(string? precision, string type, string name, string value)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append("const ")
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(type)
      .Append(' ')
      .Append(name)
      .Append(" = ")
      .Append(value)
      .AppendLine(";");
  }

  public void AppendAssignment(string name, string value)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append(name)
      .Append(" = ")
      .Append(value)
      .AppendLine(";");
  }

  public ShaderCodeBuilder AppendFunctionDeclaration(string? precision, string returnType, string name, IEnumerable<string> parameters)
  {
    Builder
      .AppendIndent(IndentLevel)
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(returnType)
      .Append(' ')
      .Append(name)
      .Append("(")
      .Append(string.Join(", ", parameters))
      .Append(")")
      .AppendLine(" {");

    return this with { IndentLevel = IndentLevel + 1 };
  }

  public void Dispose()
  {
    Builder
      .AppendIndent(IndentLevel - 1)
      .AppendLine("}");
  }
}
