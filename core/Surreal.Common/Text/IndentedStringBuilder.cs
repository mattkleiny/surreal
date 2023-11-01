namespace Surreal.Text;

/// <summary>
/// A <see cref="StringBuilder" /> wrapper that maintains an indent level.
/// <para />
/// Useful for building up human-readable code in string form.
/// </summary>
public sealed class IndentedStringBuilder
{
  private readonly StringBuilder _builder = new();
  private readonly int _indentSize;

  private int _indentLevel;

  public IndentedStringBuilder(int indentSize = 2)
  {
    _indentSize = indentSize;
  }

  /// <summary>
  /// Starts a new indentation level. Disposing of the scope will return to the previous indentation level.
  /// </summary>
  public IndentScope Indent()
  {
    _indentLevel += 1;

    return new IndentScope(this);
  }

  /// <summary>
  /// Returns to the previous indentation level.
  /// </summary>
  public void Dedent()
  {
    _indentLevel = Math.Max(0, _indentLevel - 1);
  }

  public IndentedStringBuilder Append(string value)
  {
    AppendIndent();
    _builder.Append(value);

    return this;
  }

  public IndentedStringBuilder AppendIf(bool condition, string value)
  {
    if (condition)
    {
      AppendIndent();
      _builder.Append(value);
    }

    return this;
  }

  public IndentedStringBuilder Append(object value)
  {
    AppendIndent();
    _builder.Append(value);

    return this;
  }

  public IndentedStringBuilder AppendLine()
  {
    AppendIndent();
    _builder.AppendLine();

    return this;
  }

  public IndentedStringBuilder AppendLine(string value)
  {
    AppendIndent();
    _builder.AppendLine(value);

    return this;
  }

  private void AppendIndent()
  {
    _builder.Append(new string(' ', _indentLevel * _indentSize));
  }

  public override string ToString()
  {
    return _builder.ToString();
  }

  public StringBuilder ToStringBuilder()
  {
    return new StringBuilder(ToString());
  }

  /// <summary>
  /// Scopes a single indentation level in the <see cref="IndentedStringBuilder" />.
  /// </summary>
  public readonly struct IndentScope : IDisposable
  {
    private readonly IndentedStringBuilder _builder;

    public IndentScope(IndentedStringBuilder builder)
    {
      _builder = builder;
    }

    public void Dispose()
    {
      _builder.Dedent();
    }
  }
}
