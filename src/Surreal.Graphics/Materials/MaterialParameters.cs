using System;
using System.Collections;
using System.Collections.Generic;

namespace Surreal.Graphics.Materials {
  public sealed class MaterialParameters : IEnumerable<KeyValuePair<string, object>> {
    private readonly Dictionary<string, object> uniforms = new Dictionary<string, object>();

    public void Add(string key, object value) => uniforms.Add(key, value);
    public bool Remove(string key)            => uniforms.Remove(key);
    public bool ContainsKey(string key)       => uniforms.ContainsKey(key);
    public void Clear()                       => uniforms.Clear();

    public void ApplyTo(ShaderProgram shader) {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => uniforms.GetEnumerator();
    IEnumerator IEnumerable.                         GetEnumerator() => GetEnumerator();
  }
}