using System;
using System.Diagnostics;
using Surreal.Framework.Parameters;

namespace Surreal.Diagnostics.Editing
{
  public abstract class EditorProperty
  {
    public static EditorProperty Anonymous<T>(string name, Func<T> getter, Action<T> setter)
      => new AnonymousProperty<T>(name, getter, setter);

    public static EditorProperty Parameter<T>(string name, Parameter<T> parameter)
      => new ParameterProperty<T>(name, parameter);

    public abstract string Name { get; }
    public abstract Type   Type { get; }

    public abstract object? Get();
    public abstract void    Set(object? value);

    [DebuggerDisplay("AnonymousProperty (Name={Name}, Type={Type})")]
    private sealed class AnonymousProperty<T> : EditorProperty
    {
      private readonly Func<T>   getter;
      private readonly Action<T> setter;

      public AnonymousProperty(string name, Func<T> getter, Action<T> setter)
      {
        this.getter = getter;
        this.setter = setter;

        Name = name;
      }

      public override string Name { get; }
      public override Type   Type => typeof(T);

      public override object? Get()              => getter();
      public override void    Set(object? value) => setter((T) value);
    }

    [DebuggerDisplay("ParameterProperty (Name={Name}, Type={Type}, Value={parameter})")]
    private sealed class ParameterProperty<T> : EditorProperty
    {
      private readonly Parameter<T> parameter;
      private readonly bool         useOverride;

      public ParameterProperty(string name, Parameter<T> parameter, bool useOverride = false)
      {
        this.parameter   = parameter;
        this.useOverride = useOverride;

        Name = name;
      }

      public override string Name { get; }
      public override Type   Type => typeof(T);

      public override object? Get() => parameter.Value;

      public override void Set(object? value)
      {
        if (useOverride)
        {
          parameter.Override((T) value);
        }
        else
        {
          parameter.Value = (T) value;
        }
      }
    }
  }
}