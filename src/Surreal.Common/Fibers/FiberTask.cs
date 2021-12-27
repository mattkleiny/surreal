using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Fibers.Internal;
using Surreal.Fibers.Promises;
using Surreal.Timing;

namespace Surreal.Fibers;

/// <summary>Status of a <see cref="FiberTask"/>.</summary>
public enum FiberTaskStatus
{
	Pending,
	Succeeded,
	Canceled,
	Faulted
}

/// <summary>A low allocation <see cref="Task"/>-like type for asynchronous execution.</summary>
[AsyncMethodBuilder(typeof(FiberTaskBuilder))]
public readonly struct FiberTask : IDisposable
{
	public static FiberTask CompletedTask => default;

	public static FiberTask Create(Func<FiberTask> factory) => factory();
	public static FiberTask<T> Create<T>(Func<FiberTask<T>> factory) => factory();

	public static FiberTask<T> FromResult<T>(T value) => new(value);
	public static FiberTask FromException(Exception exception) => FromPromise(new ExceptionPromise<Unit>(exception));
	public static FiberTask<T> FromException<T>(Exception exception) => FromPromise(new ExceptionPromise<T>(exception));

	public static FiberYieldAwaitable Yield() => new();
	public static WhenAllBuilder WhenAll() => new(WhenAllPromise.Create());

	internal static FiberTask FromPromise(IPromise<Unit> promise) => new(promise, promise.Version);
	internal static FiberTask<T> FromPromise<T>(IPromise<T> promise) => new(promise, promise.Version);

	public static FiberTask WhenAll(FiberTask a, FiberTask b)
		=> WhenAll().AddTask(a).AddTask(b).Begin();

	public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c)
		=> WhenAll().AddTask(a).AddTask(b).AddTask(c).Begin();

	public static FiberTask WhenAll(FiberTask a, FiberTask b, FiberTask c, FiberTask d)
		=> WhenAll().AddTask(a).AddTask(b).AddTask(c).AddTask(d).Begin();

	public static FiberTask Delay(IClock clock, TimeSpan duration)
		=> FromPromise(DelayPromise.Create(clock, duration));

	internal readonly IPromise<Unit>? Promise;
	internal readonly short Version;

	internal FiberTask(IPromise<Unit> promise, short version)
	{
		Promise = promise;
		Version = version;
	}

	public FiberTaskStatus Status
	{
		get
		{
			if (Promise != null)
			{
				return Promise.GetStatus(Version);
			}

			return FiberTaskStatus.Succeeded;
		}
	}

	[UsedImplicitly]
	public FiberTaskAwaiter GetAwaiter()
	{
		return new(this);
	}

	public void Cancel() => Promise?.Cancel(Version);
	public void Dispose() => Cancel();

	public void Forget()
	{
		// no-op
	}

	public readonly struct WhenAllBuilder
	{
		private readonly WhenAllPromise promise;

		internal WhenAllBuilder(WhenAllPromise promise)
		{
			this.promise = promise;
		}

		public WhenAllBuilder AddTask(FiberTask task)
		{
			promise.AddTask(task);

			return this;
		}

		public FiberTask Begin()
		{
			promise.Advance();

			return FromPromise(promise);
		}
	}
}

/// <summary>A low allocation <see cref="Task"/>-like type for asynchronous execution.</summary>
[AsyncMethodBuilder(typeof(FiberTaskBuilder<>))]
public readonly struct FiberTask<T> : IDisposable
{
	internal readonly IPromise<T>? Promise;
	internal readonly short Version;
	internal readonly T? Result;

	internal FiberTask(T result)
	{
		Promise = default;
		Version = default;
		Result = result;
	}

	internal FiberTask(IPromise<T> promise, short version)
	{
		Promise = promise;
		Version = version;
		Result = default;
	}

	public FiberTaskStatus Status
	{
		get
		{
			if (Promise != null)
			{
				return Promise.GetStatus(Version);
			}

			return FiberTaskStatus.Succeeded;
		}
	}

	[UsedImplicitly]
	public FiberTaskAwaiter<T> GetAwaiter()
	{
		return new(this);
	}

	public void Cancel() => Promise?.Cancel(Version);
	public void Dispose() => Cancel();
}
