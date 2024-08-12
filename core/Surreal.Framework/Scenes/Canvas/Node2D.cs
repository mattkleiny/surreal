using Surreal.Mathematics;
using Surreal.Timing;

namespace Surreal.Scenes.Canvas;

/// <summary>
/// A <see cref="SceneNode"/> that represents a 2D object.
/// </summary>
public class Node2D : SceneNode
{
  // global transform
  private Vector2 _globalPosition;
  private Vector2 _globalScale = Vector2.One;
  private Angle _globalRotation;
  private bool _isGlobalDirty;

  // local transform
  private Vector2 _localPosition;
  private Vector2 _localScale = Vector2.One;
  private Angle _localRotation;
  private bool _isLocalDirty;

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

  protected override void OnAwake()
  {
    base.OnAwake();

    // initialize global transform propagation
    SendMessageToChildren(MessageType.TransformChanged);
  }

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    if (_isGlobalDirty)
    {
      OnTransformUpdated();
      SendMessageToChildren(MessageType.TransformChanged);

      _isGlobalDirty = false;
    }

    if (_isLocalDirty)
    {
      if (TryResolveParent(out Node2D parent))
      {
        GlobalPosition = parent.GlobalPosition + LocalPosition;
        GlobalScale = parent.GlobalScale * LocalScale;
        GlobalRotation = parent.GlobalRotation + LocalRotation;
      }
      else
      {
        GlobalPosition = LocalPosition;
        GlobalScale = LocalScale;
        GlobalRotation = LocalRotation;
      }

      OnTransformUpdated();
      SendMessageToChildren(MessageType.TransformChanged);

      _isLocalDirty = false;
    }

    base.OnUpdate(deltaTime);
  }

  /// <summary>
  /// Notifies of a change in the node's transform.
  /// </summary>
  protected virtual void OnTransformUpdated()
  {
  }

  internal override void OnMessageReceivedFromParent(Message message)
  {
    base.OnMessageReceivedFromParent(message);

    switch (message)
    {
      // propagate transform changes to children
      case { Type: MessageType.TransformChanged, Sender: Node2D parent }:
      {
        _globalPosition = parent._globalPosition + _localPosition;
        _globalScale = parent._globalScale * _localScale;
        _globalRotation = parent._globalRotation + _localRotation;

        break;
      }
    }
  }
}
