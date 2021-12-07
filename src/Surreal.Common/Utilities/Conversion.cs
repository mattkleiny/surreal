using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Surreal.Utilities;

/// <summary>Permits an object to participate in type conversion.</summary>
public interface ITypeConvertible
{
  bool   IsConvertibleTo(Type type);
  object ConvertTo(Type type);
}

/// <summary>Static utilities for type conversion.</summary>
public static class Conversion
{
  public static TOther? ConvertTo<T, TOther>(T? source)
  {
    var sourceType = typeof(T);
    var targetType = typeof(TOther);

    // cast the object types directly
    if (targetType.IsAssignableFrom(sourceType))
    {
      return Unsafe.As<T?, TOther?>(ref source);
    }

    // ask the object directly
    if (source is ITypeConvertible convertible &&
        convertible.IsConvertibleTo(targetType))
    {
      return (TOther) convertible.ConvertTo(targetType);
    }

    // perform c# conversion
    return (TOther?) Convert.ChangeType(source, targetType);
  }

  public static TOther? ConvertTo<TOther>(object source)
  {
    var sourceType = source.GetType();
    var targetType = typeof(TOther);

    // cast the object types directly
    if (targetType.IsAssignableFrom(sourceType))
    {
      return (TOther) source;
    }

    // ask the object directly
    if (source is ITypeConvertible convertible &&
        convertible.IsConvertibleTo(targetType))
    {
      return (TOther) convertible.ConvertTo(targetType);
    }

    // perform c# conversion
    return (TOther?) Convert.ChangeType(source, targetType);
  }

  public static bool IsConvertibleTo(this Type sourceType, Type targetType)
  {
    if (targetType.IsAssignableFrom(sourceType)) return true;
    if (sourceType.IsTypeConvertible()) return true;
    if (sourceType.IsValueConvertible(targetType)) return true;

    var source = TypeDescriptor.GetConverter(sourceType);
    var target = TypeDescriptor.GetConverter(targetType);

    return target.IsValid(source);
  }

  private static bool IsTypeConvertible(this Type sourceType)
  {
    var interfaces = sourceType.GetInterfaces();
    if (interfaces.Any(_ => _ == typeof(ITypeConvertible)))
    {
      return true;
    }

    return false;
  }

  private static bool IsValueConvertible(this Type sourceType, Type targetType)
  {
    return sourceType.GetInterfaces().Any(_ => _ == typeof(IConvertible)) &&
           targetType.GetInterfaces().Any(_ => _ == typeof(IConvertible));
  }
}