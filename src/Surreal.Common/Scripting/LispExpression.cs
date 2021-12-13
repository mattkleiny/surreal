using System.Diagnostics;
using Surreal.Collections;

namespace Surreal.Scripting;

/// <summary>A LISP expression with support for parsing from raw sources.</summary>
public abstract record LispExpression
{
  /// <summary>Parses the <see cref="LispExpression"/> from the given raw input.</summary>
  public static LispExpression Parse(string input)
  {
    return Parse(SExpression.Parse(input));
  }

  private static LispExpression Parse(SExpression expression)
  {
    return expression switch
    {
      SExpression.List list => Parse(list),
      SExpression.Atom atom => Parse(atom),

      _ => throw new ParsingException($"Failed to parse expression type: {expression}")
    };
  }

  private static LispExpression Parse(SExpression.List root)
  {
    switch (root.Contents.Dequeue())
    {
      case SExpression.List list:
      {
        return new Call(
          Body: Parse(list),
          Arguments: root.Contents.Select(Parse).ToArray()
        );
      }
      case SExpression.Atom atom:
      {
        switch (atom.Value)
        {
          // ... similar for -, *, /, =, etc
          case "+": return new ListFunction(ListOperator.Add, root.Contents.Select(Parse).ToArray());
          case "-": return new ListFunction(ListOperator.Subtract, root.Contents.Select(Parse).ToArray());
          case "*": return new ListFunction(ListOperator.Multiply, root.Contents.Select(Parse).ToArray());
          case "/": return new ListFunction(ListOperator.Divide, root.Contents.Select(Parse).ToArray());

          case "lambda":
            Debug.Assert(root.Contents.Peek() is SExpression.List, "\"lambda\" statements should be followed by list of parameters");

            var parameters = (SExpression.List) root.Contents.Dequeue();
            Debug.Assert(parameters.Contents.All(x => x is SExpression.Atom), "\"lambda\" statements should be followed by list of string parameters");

            return new Lambda(
              Body: Parse(root.Contents.Dequeue()),
              Parameters: parameters.Contents.Cast<SExpression.Atom>().Select(x => x.Value).ToArray()
            );

          case "if":
            Debug.Assert(root.Contents.Count == 3, "\"if\" statements should be followed by three expressions");

            return new If(
              Condition: Parse(root.Contents.Dequeue()),
              TrueBranch: Parse(root.Contents.Dequeue()),
              ElseBranch: Parse(root.Contents.Dequeue())
            );

          case "define":
            Debug.Assert(root.Contents.Peek() is SExpression.Atom, "\"define\" statements should be followed by string identifier");

            var innerAtom = (SExpression.Atom) root.Contents.Dequeue();

            return new Define(
              Name: innerAtom.Value,
              Expression: Parse(root.Contents.Dequeue())
            );

          default:
            return new Call(Parse(atom), root.Contents.Select(Parse).ToArray());
        }
      }
    }

    throw new ParsingException("An unexpected form was detected");
  }

  private static LispExpression Parse(SExpression.Atom atom)
  {
    if (double.TryParse(atom.Value, out var number))
      return new Number(number);

    if (atom.Value == "#t")
      return new Bool(true);

    if (atom.Value == "#f")
      return new Bool(false);

    return new Variable(atom.Value);
  }

  public enum ListOperator
  {
    Add,
    Subtract,
    Multiply,
    Divide
  }

  public interface IVisitor<out T>
  {
    T Visit(Number expression);
    T Visit(Bool expression);
    T Visit(Variable expression);
    T Visit(Call expression);
    T Visit(Lambda expression);
    T Visit(If expression);
    T Visit(Define expression);
    T Visit(ListFunction expression);
  }

  public T Accept<T>(IVisitor<T> visitor)
  {
    return visitor.Visit(this as dynamic);
  }

  public abstract record Value(object Underlying) : LispExpression
  {
    public T Cast<T>() => (T) Underlying;
  }

  public record Number(double Literal) : Value(Literal);
  public record Bool(bool Literal) : Value(Literal);
  public record Variable(string Name) : LispExpression;
  public record Call(LispExpression Body, LispExpression[] Arguments) : LispExpression;
  public record Define(string Name, LispExpression Expression) : LispExpression;
  public record If(LispExpression Condition, LispExpression TrueBranch, LispExpression ElseBranch) : LispExpression;
  public record Lambda(LispExpression Body, string[] Parameters) : LispExpression;
  public record ListFunction(ListOperator Operator, LispExpression[] Arguments) : LispExpression;
}

/// <summary>An s-expression, for use in partial deconstruction of LISP expressions.</summary>
internal abstract record SExpression
{
  /// <summary>Parses the <see cref="SExpression"/> from the given raw input.</summary>
  public static SExpression Parse(string input)
  {
    var tokens = input
      .Replace("(", " ( ")
      .Replace(")", " ) ")
      .Split()
      .Where(_ => _ != "")
      .ToQueue();

    return ParseTokens(tokens);
  }

  private static SExpression ParseTokens(Queue<string> tokens)
  {
    var token = tokens.Dequeue();
    if (token == ")")
    {
      throw new ParsingException("Unexpected '(' character");
    }

    if (token == "(")
    {
      var list = new List();

      while (tokens.Peek() != ")")
      {
        list.Contents.Enqueue(ParseTokens(tokens));
      }

      tokens.Dequeue();
      return list;
    }

    return new Atom(token);
  }

  /// <summary>A single expression literal.</summary>
  public record Atom(string Value) : SExpression;

  /// <summary>A list of sub-expression.</summary>
  public record List : SExpression
  {
    public Queue<SExpression> Contents { get; } = new();
  }
}

/// <summary>Indicates a general issue with parsing.</summary>
public sealed class ParsingException : Exception
{
  public ParsingException(string? message)
    : base(message)
  {
  }

  public ParsingException(string? message, Exception? innerException)
    : base(message, innerException)
  {
  }
}
