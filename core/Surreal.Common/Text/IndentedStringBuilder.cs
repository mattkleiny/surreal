namespace Surreal.Text;

/// <summary>
/// A <see cref="StringBuilder" /> wrapper that maintains an indent level.
/// <para />
/// Useful for building up human-readable code in string form.
/// </summary>
public sealed class IndentedStringBuilder(int indentSize = 2)
{
  private readonly StringBuilder _builder = new();

  private int _indentLevel;

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
    _builder.Append(new string(' ', _indentLevel * indentSize));
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
  public readonly struct IndentScope(IndentedStringBuilder builder) : IDisposable
  {
    public void Dispose()
    {
      builder.Dedent();
    }
  }
}
