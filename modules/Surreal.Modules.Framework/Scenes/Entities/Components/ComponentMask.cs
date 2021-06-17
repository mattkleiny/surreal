using System;
using System.Collections.Generic;

namespace Surreal.Framework.Scenes.Entities.Components {
  internal sealed class ComponentMask {
    private readonly HashSet<int> inclusions = new();
    private readonly HashSet<int> exclusions = new();

    public void Include<TComponent>()
        where TComponent : IComponent {
      inclusions.Add(ComponentType.Holder<TComponent>.Instance.Id);
    }

    public void Exclude<TComponent>()
        where TComponent : IComponent {
      exclusions.Add(ComponentType.Holder<TComponent>.Instance.Id);
    }

    public bool InterestedIn<TComponent>()
        where TComponent : IComponent {
      var typeId = ComponentType.Holder<TComponent>.Instance.Id;

      return (inclusions.Count == 0 || inclusions.Contains(typeId)) && !exclusions.Contains(typeId);
    }

    private bool Equals(ComponentMask other) {
      return inclusions.Equals(other.inclusions) && exclusions.Equals(other.exclusions);
    }

    public override bool Equals(object? obj) {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;

      return obj is ComponentMask other && Equals(other);
    }

    public override int GetHashCode() => HashCode.Combine(inclusions, exclusions);
  }
}