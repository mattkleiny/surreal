using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Surreal.Graphics.Meshes {
  public sealed class VertexAttributes {
    private readonly VertexAttribute[] attributes;

    public static VertexAttributes FromVertex<TVertex>()
        where TVertex : unmanaged {
      return new VertexAttributes(typeof(TVertex)
          .GetMembers(BindingFlags.Public | BindingFlags.Instance)
          .OfType<FieldInfo>()
          .Where(member => member.GetCustomAttribute<VertexAttributeAttribute>() != null)
          .Select(member => member.GetCustomAttribute<VertexAttributeAttribute>())
          .ToArray());
    }

    private VertexAttributes(params VertexAttributeAttribute[] attributes) {
      Check.That(attributes.Length > 0, "At least one attribute must be specified.");

      this.attributes = CreateAttributes(attributes).ToArray();
    }

    public int Length => attributes.Length;
    public int Stride { get; private set; }

    public VertexAttribute this[int index] => attributes[index];

    private IEnumerable<VertexAttribute> CreateAttributes(IEnumerable<VertexAttributeAttribute> attributes) {
      var accumulator = 0;

      foreach (var attribute in attributes) {
        var result = new VertexAttribute(
            alias: attribute.Alias,
            count: attribute.Count,
            type: attribute.Type,
            normalized: attribute.Normalized,
            offset: accumulator
        );

        accumulator += result.Stride;

        yield return result;
      }

      // calculate total stride of the members
      Stride = accumulator;
    }

    public override string ToString() => string.Join(", ", attributes.Select(it => it.ToString()).ToArray());
  }
}