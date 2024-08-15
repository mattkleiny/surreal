using Jint;
using Jint.Native;

namespace Surreal.Scripting.JavaScript;

/// <summary>
/// Helpers for working with <see cref="JsValue"/>s.
/// </summary>
internal static class JsValueExtensions
{
  public static Variant ToVariant(this JsValue value)
  {
    return value switch
    {
      JsNumber number => number.AsNumber(),
      JsString str => str.AsString(),
      JsBoolean boolean => boolean.AsBoolean(),
      JsNull _ => Variant.Null,
      JsUndefined _ => Variant.Null,
      JsObject obj => Variant.From(obj),

      _ => throw new NotSupportedException()
    };
  }
}
