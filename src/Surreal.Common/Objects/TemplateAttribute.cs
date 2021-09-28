using System;

namespace Surreal.Objects
{
  [AttributeUsage(AttributeTargets.Class)]
  public class TemplateAttribute : Attribute
  {
    public Type Type { get; }

    public TemplateAttribute(Type type)
    {
      Type = type;
    }
  }
}
