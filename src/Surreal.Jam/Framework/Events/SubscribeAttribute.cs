using System;
using JetBrains.Annotations;

namespace Surreal.Framework.Events {
  [MeansImplicitUse]
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class SubscribeAttribute : Attribute {
  }
}