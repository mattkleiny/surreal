using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Surreal.Diagnostics.Logging
{
  public interface ILog
  {
    bool IsLevelEnabled(LogLevel level);
    void WriteMessage(LogLevel level, string message);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Trace(string message)
    {
      if (IsLevelEnabled(LogLevel.Trace))
      {
        WriteMessage(LogLevel.Trace, message);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Debug(string message)
    {
      if (IsLevelEnabled(LogLevel.Debug))
      {
        WriteMessage(LogLevel.Debug, message);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Warn(string message)
    {
      if (IsLevelEnabled(LogLevel.Warn))
      {
        WriteMessage(LogLevel.Warn, message);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Error(string message)
    {
      if (IsLevelEnabled(LogLevel.Error))
      {
        WriteMessage(LogLevel.Error, message);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Fatal(string message)
    {
      if (IsLevelEnabled(LogLevel.Fatal))
      {
        WriteMessage(LogLevel.Fatal, message);
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Profile(string message, Action body)
    {
      var stopwatch = Stopwatch.StartNew();
      try
      {
        body();
      }
      finally
      {
        stopwatch.Stop();
        Trace($"{message}. Time taken: {stopwatch.Elapsed:g}");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    TResult Profile<TResult>(string message, Func<TResult> body)
    {
      var stopwatch = Stopwatch.StartNew();
      try
      {
        return body();
      }
      finally
      {
        stopwatch.Stop();
        Trace($"{message}. Time taken: {stopwatch.Elapsed:g}");
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async Task<TResult> ProfileAsync<TResult>(string message, Func<Task<TResult>> body)
    {
      var stopwatch = Stopwatch.StartNew();
      try
      {
        return await body();
      }
      finally
      {
        stopwatch.Stop();
        Trace($"{message}. Time taken: {stopwatch.Elapsed:g}");
      }
    }
  }
}