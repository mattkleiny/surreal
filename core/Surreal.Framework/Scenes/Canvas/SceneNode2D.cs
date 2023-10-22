using Surreal.Maths;
using Surreal.Timing;

namespace Surreal.Scenes.Canvas;

/// <summary>
/// A node in 2d space.
/// </summary>
public class SceneNode2D : SceneNode
{
  private Vector2 _localPosition;
  private Vector2 _localScale = Vector2.One;
  private Angle _localRotation;

  private Vector2 _globalPosition;
  private Vector2 _globalScale = Vector2.One;
  private Angle _globalRotation;

  private bool _isLocalDirty = true;
  private bool _isGlobalDirty = true;

  /// <summary>
  /// The position of the node, relative to it's parent.
  /// </summary>
  public Vector2 LocalPosition
  {
    get => _localPosition;
    set
    {
      if (SetField(ref _localPosition, value))
      {
        _isLocalDirty = true;
      }
    }
  }

  /// <summary>
  /// The scale of the node, relative to it's parent.
  /// </summary>
  public Vector2 LocalScale
  {
    get => _localScale;
    set
    {
      if (SetField(ref _localScale, value))
      {
        _isLocalDirty = true;
      }
    }
  }

  /// <summary>
  /// The rotation of the node, relative to it's parent.
  /// </summary>
  public Angle LocalRotation
  {
    get => _localRotation;
    set
    {
      if (SetField(ref _localRotation, value))
      {
        _isLocalDirty = true;
      }
    }
  }

  /// <summary>
  /// The position of the node, relative to the scene.
  /// </summary>
  public Vector2 GlobalPosition
  {
    get => _globalPosition;
    set
    {
      if (SetField(ref _globalPosition, value))
      {
        _isGlobalDirty = true;
      }
    }
  }

  /// <summary>
  /// The scale of the node, relative to the scene.
  /// </summary>
  public Vector2 GlobalScale
  {
    get => _globalScale;
    set
    {
      if (SetField(ref _globalScale, value))
      {
        _isGlobalDirty = true;
      }
    }
  }

  /// <summary>
  /// The rotation of the node, relative to the scene.
  /// </summary>
  public Angle GlobalRotation
  {
    get => _globalRotation;
    set
    {
      if (SetField(ref _globalRotation, value))
      {
        _isGlobalDirty = true;
      }
    }
  }

  /// <summary>
  /// Notifies of a change in the node's transform.
  /// </summary>
  protected virtual void OnTransformUpdated()
  {
  }

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    if (_isGlobalDirty)
    {
      // notify children of transform changes
      Inbox.Enqueue(new Notification(NotificationType.TransformChanged, this));

      OnTransformUpdated();
      _isGlobalDirty = false;
    }

    if (_isLocalDirty)
    {
      if (TryResolveParent(out SceneNode2D parent))
      {
        GlobalPosition = parent.GlobalPosition + LocalPosition;
        GlobalScale = parent.GlobalScale * LocalScale;
        GlobalRotation = parent.GlobalRotation + LocalRotation;
      }

      OnTransformUpdated();
      _isLocalDirty = false;
    }
  }

  internal override void OnNotification(Notification notification)
  {
    base.OnNotification(notification);

    switch (notification)
    {
      case { Type: NotificationType.TransformChanged, Sender: SceneNode2D sender } when sender != this:
      {
        // propagate transform changes to children
        GlobalPosition = sender.GlobalPosition + LocalPosition;
        GlobalScale = sender.GlobalScale * LocalScale;
        GlobalRotation = sender.GlobalRotation + LocalRotation;

        OnTransformUpdated();

        _isGlobalDirty = false; // we're already going to notify children

        break;
      }
    }
  }
}
