using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Surreal.Graphics.UI.Editors;

#pragma warning disable CA2255

/// <summary>Associates an <see cref="ImmediateModeEditor{T}"/> with a given type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class ImmediateModeEditorAttribute : Attribute
{
  public Type Type { get; }

  public ImmediateModeEditorAttribute(Type type)
  {
    Type = type;
  }
}

/// <summary>Base class for any <see cref="ImmediateModeEditor{T}"/> implementations.</summary>
public abstract class ImmediateModeEditor
{
  private static Dictionary<Type, ImmediateModeEditor> editorsByType = new();

  [ModuleInitializer]
  internal static void DiscoverEditors()
  {
    var results =
      from assembly in AppDomain.CurrentDomain.GetAssemblies()
      where !assembly.IsDynamic
      from type in assembly.GetExportedTypes()
      where typeof(ImmediateModeEditor).IsAssignableFrom(type)
      let attribute = type.GetCustomAttribute<ImmediateModeEditorAttribute>()
      where attribute != null
      where type.IsClass && !type.IsAbstract
      select new
      {
        Type = attribute.Type,
        Editor = (ImmediateModeEditor) Activator.CreateInstance(type)!
      };

    foreach (var result in results)
    {
      editorsByType[result.Type] = result.Editor;
    }
  }

  public static bool TryGet<T>([NotNullWhen(true)] out ImmediateModeEditor<T>? result)
  {
    if (editorsByType.TryGetValue(typeof(T), out var editor))
    {
      result = editor as ImmediateModeEditor<T>;
      return result != null;
    }

    result = default;
    return false;
  }
}

/// <summary>Allows editing a particular type <see cref="T"/> from an <see cref="IImmediateModeCanvas"/>.</summary>
public abstract class ImmediateModeEditor<T> : ImmediateModeEditor
{
  public abstract void DrawReadOnly(IImmediateModeCanvas layout, T value);
  public abstract void DrawReadWrite(IImmediateModeCanvas layout, ref T value);
}

/// <summary>Allows interacting with <see cref="ImmediateModeEditor"/>s from <see cref="IImmediateModeCanvas"/> APIs.</summary>
public static class ImmediateModeEditorExtensions
{
  public static void DrawEditorReadOnly<T>(this IImmediateModeCanvas layout, T value)
  {
    if (ImmediateModeEditor.TryGet<T>(out var editor))
    {
      editor.DrawReadOnly(layout, value);
    }
  }

  public static void DrawEditor<T>(this IImmediateModeCanvas layout, T value)
  {
    if (ImmediateModeEditor.TryGet<T>(out var editor))
    {
      editor.DrawReadWrite(layout, ref value);
    }
  }

  public static void DrawEditor<T>(this IImmediateModeCanvas layout, ref T value)
  {
    if (ImmediateModeEditor.TryGet<T>(out var editor))
    {
      editor.DrawReadWrite(layout, ref value);
    }
  }
}
