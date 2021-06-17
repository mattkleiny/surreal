using System;
using System.ComponentModel.Design;
using Surreal.Timing;

namespace Surreal.Modules.Mods {
  public interface IModContext {
    IServiceContainer Services { get; }
  }

  public interface IMod : IDisposable {
    void Initialize(IModContext context);
    void Input(DeltaTime deltaTime);
    void Update(DeltaTime deltaTime);
    void Draw(DeltaTime deltaTime);
  }

  public abstract class Mod : IMod {
    public virtual void Initialize(IModContext context) {
    }

    public virtual void Input(DeltaTime deltaTime) {
    }

    public virtual void Update(DeltaTime deltaTime) {
    }

    public virtual void Draw(DeltaTime deltaTime) {
    }

    public virtual void Dispose() {
    }
  }
}