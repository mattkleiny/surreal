using System.Collections.Generic;

namespace Surreal.Collections
{
  public interface IMultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, IReadOnlyList<TValue>>>
  {
    int Count { get; }

    IEnumerable<TKey>   Keys   { get; }
    IEnumerable<TValue> Values { get; }

    IReadOnlyList<TValue> this[TKey key] { get; }

    bool TryGetValues(TKey key, out IReadOnlyList<TValue> values);

    void Add(TKey key, TValue value);
    void Remove(TKey key, TValue value);
    void RemoveAll(TKey key);
    bool ContainsKey(TKey key);
    void Clear();
  }
}
