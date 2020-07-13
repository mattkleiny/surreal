namespace Surreal.Framework.Parameters {
  public abstract class Parameter {
  }

  public abstract class Parameter<T> : Parameter {
    private T value;
    private T overrideValue;

    private bool hasOverride;

    protected Parameter(T value) {
      this.value    = value;
      overrideValue = default!;
    }

    public virtual T Value {
      get {
        if (hasOverride) {
          return overrideValue;
        }

        return value;
      }
      set => this.value = value;
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