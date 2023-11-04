using System.Linq.Expressions;

namespace Surreal.Utilities;

/// <summary>
/// Helpers for working with C# expressions.
/// </summary>
public static class ExpressionExtensions
{
  /// <summary>
  /// Attempts to get the given attribute, <see cref="TAttribute"/> from the given property expression.
  /// </summary>
  public static bool TryGetPropertyAttribute<TAttribute>(this Expression property, out TAttribute attribute)
    where TAttribute : Attribute
  {
    if (!property.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    return propertyInfo.TryGetCustomAttribute(out attribute);
  }

  /// <summary>
  /// Gets the given attribute, <see cref="TAttribute"/> from the given property expression.
  /// If the attribute is not found, an exception is thrown.
  /// </summary>
  public static TAttribute GetPropertyAttributeOrThrow<TAttribute>(this Expression property)
    where TAttribute : Attribute
  {
    if (!property.TryGetPropertyAttribute(out TAttribute attribute))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    return attribute;
  }

  /// <summary>
  /// Attempts to get the given attribute, <see cref="TAttribute"/> from the given property expression.
  /// </summary>
  public static bool GetPropertyAttributes<TAttribute>(this Expression property, out TAttribute[] attributes)
    where TAttribute : Attribute
  {
    if (!property.TryResolveProperty(out var propertyInfo))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    return propertyInfo.TryGetCustomAttributes(out attributes);
  }

  /// <summary>
  /// Gets the given attribute, <see cref="TAttribute"/> from the given property expression.
  /// If the attribute is not found, an exception is thrown.
  /// </summary>
  public static TAttribute[] GetPropertyAttributesOrThrow<TAttribute>(this Expression property)
    where TAttribute : Attribute
  {
    if (!property.GetPropertyAttributes(out TAttribute[] attributes))
    {
      throw new InvalidOperationException($"The given expression is not a valid property accessor: {property}");
    }

    return attributes;
  }

  /// <summary>
  /// Resolves different supported forms of expression for property accesses.
  /// </summary>
  public static bool TryResolveProperty(this Expression property, out PropertyInfo result)
  {
    if (property is LambdaExpression { Body: var body })
    {
      // recurse into the body
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
