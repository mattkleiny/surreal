using System.Collections.Generic;

namespace Mindustry.Modules.Core.Model.Resources {
  public sealed class ResourceInventory {
    private readonly List<ResourceStack> stacks = new List<ResourceStack>();

    public ushort this[ResourceType type] {
      get {
        var stack = GetStack(type);

        if (stack != null) {
          return stack.Count;
        }

        return 0;
      }
      set {
        var stack = GetStack(type);

        if (stack != null) {
          stack.Count = value;

          if (stack.Count == 0) {
            stacks.Remove(stack);
          }
        }
        else if (value > 0) {
          stacks.Add(new ResourceStack(type, value));
        }
      }
    }

    public void Add(ResourceStack stack)    => this[stack.Type] += stack.Count;
    public void Remove(ResourceStack stack) => this[stack.Type] -= stack.Count;

    private ResourceStack? GetStack(ResourceType type) {
      foreach (var stack in stacks) {
        if (stack.Type == type) {
          return stack;
        }
      }

      return null;
    }
  }
}