using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Surreal.Diagnostics.Logging
{
  public enum LogLevel : byte
  {
    Trace = 0,
    Debug = 1,
    Warn  = 2,
    Error = 3,
    Fatal = 4,
  }

  public interface ILog
  {
    bool IsLevelEnabled(LogLevel level);
    void WriteMessage(LogLevel level, string message);

    void Trace(string message)
    {
      if (IsLevelEnabled(LogLevel.Trace))
      {
        WriteMessage(LogLevel.Trace, message);
      }
    }

    void Debug(string message)
    {
      if (IsLevelEnabled(LogLevel.Debug))
      {
        WriteMessage(LogLevel.Debug, message);
      }
    }

    void Warn(string message)
    {
      if (IsLevelEnabled(LogLevel.Warn))
      {
        WriteMessage(LogLevel.Warn, message);
      }
    }

    void Error(string message)
    {
      if (IsLevelEnabled(LogLevel.Error))
      {
        WriteMessage(LogLevel.Error, message);
      }
    }

    void Fatal(string message)
    {
      if (IsLevelEnabled(LogLevel.Fatal))
      {
        WriteMessage(LogLevel.Fatal, message);
      }
    }

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
        Trace($"{message}. Time taken: {stopwatch.Elapsed.ToString("g")}");
      }
    }

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
        Trace($"{message}. Time taken: {stopwatch.Elapsed.ToString("g")}");
      }
    }

    async Task ProfileAsync(string message, Func<Task> body)
    {
      var stopwatch = Stopwatch.StartNew();
      try
      {
        await body();
      }
      finally
      {
        stopwatch.Stop();
        Trace($"{message}. Time taken: {stopwatch.Elapsed.ToString("g")}");
      }
    }

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
        Trace($"{message}. Time taken: {stopwatch.Elapsed.ToString("g")}");
      }
    }
  }
}