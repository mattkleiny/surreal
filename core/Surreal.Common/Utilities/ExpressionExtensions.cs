using System.Linq.Expressions;
using System.Reflection;

namespace Surreal.Utilities;

/// <summary>Helpers for working with C# expressions</summary>
public static class ExpressionExtensions
{
  /// <summary>Resolves the given attribute, <see cref="TAttribute"/> from the given property expression.</summary>
  public static TAttribute ResolvePropertyAttribute<TAttribute>(this Expression property)
    where TAttribute : Attribute
  {
    if (!property.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    var attribute = propertyInfo.GetCustomAttribute<TAttribute>();
    if (attribute == null)
    {
      throw new InvalidOperationException($"The given property expression does not have an associated {nameof(TAttribute)}: {property}");
    }

    return attribute;
  }

  /// <summary>Resolves the given attribute, <see cref="TAttribute"/> from the given property expression.</summary>
  public static IEnumerable<TAttribute> ResolvePropertyAttributes<TAttribute>(this Expression property)
    where TAttribute : Attribute
  {
    if (!property.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    var attributes = propertyInfo.GetCustomAttributes<TAttribute>().ToArray();
    if (!attributes.Any())
    {
      throw new InvalidOperationException($"The given property expression does not have any associated {nameof(TAttribute)}: {property}");
    }

    return attributes;
  }

  /// <summary>Resolves different supported forms of expression for property accesses.</summary>
  public static bool TryResolveProperty(this Expression property, out PropertyInfo result)
  {
    if (property is LambdaExpression { Body: var body })
    {
      return TryResolveProperty(body, out result);
    }

    if (property is MemberExpression { Member: PropertyInfo propertyInfo1 })
    {
      result = propertyInfo1;
      return true;
    }

    if (property is UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression { Member: PropertyInfo propertyInfo2 } })
    {
      result = propertyInfo2;
      return true;
    }

    result = default!;
    return false;
  }
}
