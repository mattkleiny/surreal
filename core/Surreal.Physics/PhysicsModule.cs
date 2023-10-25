﻿using Surreal.Utilities;

namespace Surreal.Physics;

/// <summary>
/// A <see cref="IServiceModule"/> for the <see cref="Physics"/> namespace.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PhysicsModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddAssemblyServices(Assembly.GetExecutingAssembly());
  }
}