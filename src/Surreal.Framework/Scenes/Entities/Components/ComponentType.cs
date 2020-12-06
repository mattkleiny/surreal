using System;

namespace Surreal.Framework.Scenes.Entities.Components {
  internal struct ComponentType {
    private static int NextComponentId; // static id for identifying component types lazily

    public readonly int  Id;
    public readonly Type Type;

    private ComponentType(int id, Type type) {
      Id   = id;
      Type = type;
    }

    public static class Holder<T>
        where T : IComponent {
      public static ComponentType Instance { get; } = new(NextComponentId++, typeof(T));
    }
  }
}