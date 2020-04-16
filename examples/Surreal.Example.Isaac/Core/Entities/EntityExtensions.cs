using System.Numerics;
using Isaac.Core.Entities.Components;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Graphics;
using Surreal.Graphics.Textures;

namespace Isaac.Core.Entities
{
  public static class EntityExtensions
  {
    public static bool IsPlayer(this Entity entity)  => entity.Has<Player>();
    public static bool HasHealth(this Entity entity) => entity.Has<Health>();

    public static Entity CreateStandardEntity(
      this EntityScene scene,
      Color tint,
      int initialHealth = 100,
      TextureRegion? sprite = null)
    {
      var entity = scene.CreateEntity();

      entity.Create(new Health
      {
        Life = initialHealth
      });

      entity.Create(new Transform
      {
        Position = Vector2.Zero,
        Rotation = 0f
      });

      entity.Create(new RigidBody
      {
        Acceleration = Vector2.Zero,
        Velocity     = Vector2.Zero
      });

      if (sprite != null)
      {
        entity.Create(new Sprite
        {
          Scale   = 1f,
          Texture = sprite,
          Tint    = tint,
        });
      }

      return entity;
    }

    public static Entity CreatePlayer(
      this EntityScene scene,
      TextureRegion sprite,
      int initialHealth = 100,
      float moveSpeed = 100f)
    {
      var entity = scene.CreateStandardEntity(
        tint: Color.White,
        initialHealth: initialHealth,
        sprite: sprite
      );

      entity.Create(new Player
      {
        MoveSpeed = moveSpeed
      });

      return entity;
    }

    public static Entity CreateEnemy(
      this EntityScene scene,
      TextureRegion sprite,
      int initialHealth = 30,
      float moveSpeed = 100f)
    {
      var entity = scene.CreateStandardEntity(
        tint: Color.White,
        initialHealth: initialHealth,
        sprite: sprite
      );
      return entity;
    }
  }
}
