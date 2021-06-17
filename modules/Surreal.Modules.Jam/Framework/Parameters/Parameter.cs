using System;

namespace Surreal.Framework.Parameters {
  public class Parameter<T> {
    private T value;
    private T overrideValue;

    private bool hasOverride;

    public Parameter(T value) {
      this.value    = value;
      overrideValue = default!;
    }

    public event Action<T>? Changed;

    public virtual T Value {
      get {
        if (hasOverride) {
          return overrideValue;
        }

        return value;
      }
      set {
        this.value = value;

        Changed?.Invoke(value);
      }
    }

    public virtual void Override(T value) {
      overrideValue = value;
      hasOverride   = true;
    }

    public void ResetOverride() {
      overrideValue = default!;
      hasOverride   = false;
    }

    public static implicit operator T(Parameter<T> parameter) => parameter.Value;
  }
}