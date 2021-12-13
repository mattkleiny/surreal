using static Surreal.Scripting.LispExpression;

namespace Surreal.Scripting;

/// <summary>Interprets and executes LISP programs.</summary>
public sealed class LispInterpreter
{
  private readonly Dictionary<string, LispExpression> environment = new();
  private readonly EvaluationVisitor                  visitor;

  public LispInterpreter()
  {
    visitor = new EvaluationVisitor(this);
  }

  public object Execute(string raw)
  {
    var expression = Parse(raw);

    return Execute(expression);
  }

  public object Execute(LispExpression expression)
  {
    var value = expression.Accept(visitor);

    return value.Underlying;
  }

  private sealed class EvaluationVisitor : IVisitor<Value>
  {
    private readonly LispInterpreter interpreter;

    public EvaluationVisitor(LispInterpreter interpreter)
    {
      this.interpreter = interpreter;
    }

    public Value Visit(Number expression)
    {
      return expression;
    }

    public Value Visit(Bool expression)
    {
      return expression;
    }

    public Value Visit(Variable expression)
    {
      if (!interpreter.environment.TryGetValue(expression.Name, out var value))
      {
        throw new ParsingException($"An undefined variable was referenced {expression.Name}");
      }

      return value.Accept(this);
    }

    public Value Visit(Call expression)
    {
      throw new NotImplementedException();
    }

    public Value Visit(Lambda expression)
    {
      throw new NotImplementedException();
    }

    public Value Visit(If expression)
    {
      if (expression.Condition.Accept(this).Cast<bool>())
      {
        return expression.TrueBranch.Accept(this);
      }

      return expression.ElseBranch.Accept(this);
    }

    public Value Visit(Define expression)
    {
      throw new NotImplementedException();
    }

    public Value Visit(ListFunction expression)
    {
      IEnumerable<T> As<T>(LispExpression[] Arguments)
      {
        return expression.Arguments.Select(_ => _.Accept(this).Cast<T>());
      }

      switch (expression.Operator)
      {
        case ListOperator.Add:
        {
          var result = 0d;

          foreach (var value in As<double>(expression.Arguments))
          {
            result += value;
          }

          return new Number(result);
        }
        case ListOperator.Subtract:
        {
          var result = 0d;

          foreach (var value in As<double>(expression.Arguments))
          {
            result -= value;
          }

          return new Number(result);
        }
        case ListOperator.Multiply:
        {
          var result = 0d;

          foreach (var value in As<double>(expression.Arguments))
          {
            result *= value;
          }

          return new Number(result);
        }
        case ListOperator.Divide:
        {
          var result = 0d;

          foreach (var value in As<double>(expression.Arguments))
          {
            result /= value;
          }

          return new Number(result);
        }

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
