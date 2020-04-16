using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

internal static class Check
{
  [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void NotNullOrEmpty(string value, [InvokerParameterName] string name)
  {
    if (string.IsNullOrEmpty(value))
    {
      throw new ArgumentNullException(name);
    }
  }

  [Conditional("DEBUG"), MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void That(bool condition, string message)
  {
    if (!condition)
    {
      throw new ArgumentException(message);
    }
  }
}