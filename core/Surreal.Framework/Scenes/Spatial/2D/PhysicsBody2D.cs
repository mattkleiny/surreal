namespace Surreal.Scenes.Spatial;

public abstract class CollisionNode2D : SceneNode2D;

public class Area2D : CollisionNode2D;
public abstract class PhysicsBody2D : CollisionNode2D;

public class StaticBody2D : PhysicsBody2D;
public class KinematicBody2D : PhysicsBody2D;
public class RigidBody2D : PhysicsBody2D;

public class CollisionShape2D(Shape2D shape) : SceneNode2D;

public abstract class Shape2D;
public class CircleShape2D : Shape2D;
public class RectangleShape2D : Shape2D;
