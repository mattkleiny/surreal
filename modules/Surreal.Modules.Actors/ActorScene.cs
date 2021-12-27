using System.Reflection;
using Surreal.Storage;
using Surreal.Timing;

namespace Surreal;

/// <summary>A scene of managed <see cref="Actor"/>s.</summary>
public sealed class ActorScene : IActorContext
{
	private readonly Dictionary<ActorId, SceneNode<Actor>> nodes = new();
	private readonly Dictionary<Type, IComponentStorage> storages = new();

	private readonly LinkedList<Actor> awakeQueue = new();
	private readonly LinkedList<Actor> destroyQueue = new();

	public void Input(DeltaTime time)
	{
		foreach (var node in nodes.Values)
		{
			if (node.Status == ActorStatus.Active)
			{
				node.Actor.OnInput(time);
			}
		}
	}

	public void Update(DeltaTime time)
	{
		foreach (var node in nodes.Values)
		{
			if (node.Status == ActorStatus.Active)
			{
				node.Actor.OnUpdate(time);
			}
		}
	}

	public void Draw(DeltaTime time)
	{
		foreach (var node in nodes.Values)
		{
			if (node.Status == ActorStatus.Active)
			{
				node.Actor.OnDraw(time);
			}
		}
	}

	public ActorStatus GetStatus(ActorId id)
	{
		if (nodes.TryGetValue(id, out var node))
		{
			return node.Status;
		}

		return ActorStatus.Unknown;
	}

	public void Spawn(Actor actor)
	{
		if (nodes.TryGetValue(actor.Id, out var node))
		{
			node.Status = ActorStatus.Active;
		}
		else
		{
			nodes[actor.Id] = new SceneNode<Actor>(actor)
			{
				Status = ActorStatus.Active
			};

			awakeQueue.AddLast(actor);
		}
	}

	public void Enable(ActorId id)
	{
		if (nodes.TryGetValue(id, out var node))
		{
			node.Status = ActorStatus.Active;
		}
	}

	public void Disable(ActorId id)
	{
		if (nodes.TryGetValue(id, out var node))
		{
			node.Status = ActorStatus.Inactive;
		}
	}

	public void Destroy(ActorId id)
	{
		if (nodes.TryGetValue(id, out var node))
		{
			destroyQueue.AddLast(node.Actor);
		}
	}

	IComponentStorage<T> IActorContext.GetStorage<T>()
	{
		if (!storages.TryGetValue(typeof(T), out var storage))
		{
			storages[typeof(T)] = storage = CreateStorage<T>();
		}

		return (IComponentStorage<T>) storage;
	}

	private static IComponentStorage<T> CreateStorage<T>()
	{
		var storageType = typeof(SparseComponentStorage<>);
		var attribute = typeof(T).GetCustomAttribute<ComponentAttribute>();

		if (attribute != null)
		{
			storageType = attribute.StorageType;
		}

		var instance = Activator.CreateInstance(storageType.MakeGenericType(typeof(T)))!;

		return (IComponentStorage<T>) instance;
	}

	private sealed record SceneNode<T>(T Actor)
	{
		public ActorStatus Status { get; set; }
		public SceneNode<T>? Parent { get; set; }
		public List<SceneNode<T>> Children { get; } = new();
	}
}
