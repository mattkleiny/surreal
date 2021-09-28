using System;
using System.Reflection;

namespace Surreal.Objects
{
  public static class TemplateFactory
  {
    public static T CreateFromTemplate<T>()
    {
      return TemplateCache<T>.Template.Create();
    }

    public static Template<T> CreateBlankTemplate<T>()
    {
      var attribute = typeof(T).GetCustomAttribute<TemplateAttribute>();
      if (attribute == null || !typeof(Template<T>).IsAssignableFrom(attribute.Type))
      {
        throw new Exception($"The type {typeof(T)} does not have a valid template associated");
      }

      return (Template<T>) Activator.CreateInstance(attribute.Type)!;
    }

    private static class TemplateCache<T>
    {
      public static Template<T> Template { get; } = CreateBlankTemplate<T>();
    }
  }
}
