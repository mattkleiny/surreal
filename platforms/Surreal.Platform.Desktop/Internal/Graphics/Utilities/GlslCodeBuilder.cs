using Surreal.Text;

namespace Surreal.Internal.Graphics.Utilities;

/// <summary>A scope of a <see cref="GlslCodeBuilder"/>, for writing raw GLSL.</summary>
internal interface IShaderCodeBuilderScope : IDisposable
{
  void AppendLine(string raw);
  void AppendBlankLine();
  void AppendComment(string text);
  void AppendUniformDeclaration(string? precision, string type, string name);
  void AppendVaryingDeclaration(string? precision, string type, string name);
  void AppendConstantDeclaration(string? precision, string type, string name, string value);
  void AppendAssignment(string name, string value);

  IShaderCodeBuilderScope AppendFunctionDeclaration(
    string? precision,
    string returnType,
    string name,
    IEnumerable<string> parameters
  );
}

/// <summary>A utility for building shader programs from raw source text.</summary>
internal sealed class GlslCodeBuilder : IShaderCodeBuilderScope
{
  private readonly Scope rootScope = new(new StringBuilder(), IndentLevel: 0);

  public void AppendLine(string raw)
    => rootScope.AppendLine(raw);

  public void AppendBlankLine()
    => rootScope.AppendBlankLine();

  public void AppendComment(string text)
    => rootScope.AppendComment(text);

  public void AppendUniformDeclaration(string? precision, string type, string name)
    => rootScope.AppendUniformDeclaration(precision, type, name);

  public void AppendVaryingDeclaration(string? precision, string type, string name)
    => rootScope.AppendVaryingDeclaration(precision, type, name);

  public void AppendConstantDeclaration(string? precision, string type, string name, string value)
    => rootScope.AppendConstantDeclaration(precision, type, name, value);

  public void AppendAssignment(string name, string value)
    => rootScope.AppendAssignment(name, value);

  public IShaderCodeBuilderScope AppendFunctionDeclaration(
    string? precision,
    string returnType,
    string name,
    IEnumerable<string> parameters
  )
  {
    return rootScope.AppendFunctionDeclaration(precision, returnType, name, parameters);
  }

  public string ToSourceCode()
    => rootScope.Builder.ToString();

  public void Dispose()
  {
    // no-op
  }

  /// <summary>A scope for an indented part of the code.</summary>
  private sealed record Scope(StringBuilder Builder, int IndentLevel) : IShaderCodeBuilderScope
  {
    public void AppendLine(string raw) => Builder.AppendLine(raw);
    public void AppendBlankLine()      => Builder.AppendLine();

    public void AppendComment(string text) => Builder
      .AppendIndent(IndentLevel)
      .Append("/* ")
      .Append(text)
      .AppendLine(" */");

    public void AppendUniformDeclaration(string? precision, string type, string name) => Builder
      .AppendIndent(IndentLevel)
      .Append("uniform ")
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(type)
      .Append(' ')
      .Append(name)
      .AppendLine(";");

    public void AppendVaryingDeclaration(string? precision, string type, string name) => Builder
      .AppendIndent(IndentLevel)
      .Append("varying ")
      .Append(precision)
      .Append(precision != null ? ' ' : null)
      .Append(type)
      .Append(' ')
      .Append(name)
      .AppendLine(";");

    public void AppendConstantDeclaration(string? precision, string type, string name, string value) => Builder
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

    public void AppendAssignment(string name, string value) => Builder
      .AppendIndent(IndentLevel)
      .Append(name)
      .Append(" = ")
      .Append(value)
      .AppendLine(";");

    public IShaderCodeBuilderScope AppendFunctionDeclaration(
      string? precision,
      string returnType,
      string name,
      IEnumerable<string> parameters
    )
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
}
