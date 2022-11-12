using Surreal.Collections;
using Surreal.Timing;

namespace Surreal;

/// <summary>The possible states for an <see cref="Actor" />.</summary>
public enum ActorStatus
{
  Unknown,
  Active,
  Inactive,
  Destroyed
}

/// <summary>An actor in the game world.</summary>
/// <remarks>
///   This is a hybrid Game Object/ECS model that permits fast internal iteration of ECS-bound
///   components as well as high-level composition of game logic via classes and inheritance.
/// </remarks>
public class Actor
{
  private IActorContext? _context;

  public Actor()
  {
    Behaviours = new ActorBehaviourList(this);
  }

  public ArenaIndex Id { get; private set; } = ArenaIndex.Invalid;

  public ActorStatus Status => _context?.GetStatus(Id) ?? ActorStatus.Unknown;
  public IServiceProvider? Services => _context?.Services;

  public bool IsDestroyed => Status == ActorStatus.Destroyed;
  public bool IsActive => Status == ActorStatus.Active;
  public bool IsInactive => Status == ActorStatus.Inactive;

  public ActorBehaviourList Behaviours { get; }

  public void Enable()
  {
    _context?.Enable(Id);
  }

  public void Disable()
  {
    _context?.Disable(Id);
  }

  public void Destroy()
  {
    _context?.Destroy(Id);
  }

  internal void Connect(IActorContext context, ArenaIndex id)
  {
    _context = context;

    Id = id;
  }

  internal void Disconnect(IActorContext context)
  {
    _context = null;

    Id = ArenaIndex.Invalid;
  }

  public T Spawn<T>(T actor)
    where T : Actor
  {
    if (_context == null)
    {
      throw new InvalidOperationException("The actor is not part of a scene, unable to spawn child actor");
    }

    return _context.Spawn(actor);
  }

  public Actor AddBehaviour(ActorBehaviour behaviour)
  {
    Behaviours.Add(behaviour);

    return this;
  }

  public bool TryGetBehaviour<T>(out T result)
    where T : ActorBehaviour
  {
    return Behaviours.TryGet(out result);
  }

  public T GetBehaviour<T>()
  {
    throw new NotImplementedException();
  }

  public void AddComponent<T>(T component)
  {
    throw new NotImplementedException();
  }

  public ref T GetComponent<T>()
  {
    throw new NotImplementedException();
  }

  public bool TryGetComponent<T>(out T result)
  {
    throw new NotImplementedException();
  }

  protected internal virtual void OnAwake()
  {
    foreach (var behaviour in Behaviours) behaviour.OnAwake();
  }

  protected internal virtual void OnStart()
  {
    foreach (var behaviour in Behaviours) behaviour.OnStart();
  }

  protected internal virtual void OnEnable()
  {
    foreach (var behaviour in Behaviours) behaviour.OnEnable();
  }

  protected internal virtual void OnBeginFrame(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours) behaviour.OnBeginFrame(deltaTime);
  }

  protected internal virtual void OnInput(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours) behaviour.OnInput(deltaTime);
  }

  protected internal virtual void OnUpdate(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours) behaviour.OnUpdate(deltaTime);
  }

  protected internal virtual void OnDraw(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours) behaviour.OnDraw(deltaTime);
  }

  protected internal virtual void OnEndFrame(TimeDelta deltaTime)
  {
    foreach (var behaviour in Behaviours) behaviour.OnEndFrame(deltaTime);
  }

  protected internal virtual void OnDisable()
  {
    foreach (var behaviour in Behaviours) behaviour.OnDisable();
  }

  protected internal virtual void OnDestroy()
  {
    foreach (var behaviour in Behaviours) behaviour.OnDestroy();
  }
}


