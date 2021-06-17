using System.Collections;
using System.Collections.Generic;

namespace Surreal.Mathematics.Tensors {
  public interface ITensor : IEnumerable {
    int   Rank  { get; }
    int[] Shape { get; }
  }

  public interface ITensor<T> : ITensor, IEnumerable<T> {
    T this[params int[] ranks] { get; set; }
  }
}