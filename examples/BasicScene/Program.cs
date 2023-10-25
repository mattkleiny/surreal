﻿using Surreal.Scenes.Spatial.Physics;

var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Scene Graph Example",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080,
      IsTransparent = true
    }
  }
};

Game.StartScene<ForwardRenderPipeline>(configuration, (SceneTree scene, ForwardRenderPipeline pipeline) =>
{
  pipeline.ClearColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

  scene.Add(new CameraViewport
  {
    new Camera2D { Zoom = 100f },
    new RigidBodySpawner { GlobalPosition = new Vector2(45f, 60f) },
  });
});

public sealed class RigidBodySpawner : Node2D
{
  private IntervalTimer _spawnTimer = new(TimeSpan.FromSeconds(0.5f));

  protected override void OnUpdate(DeltaTime deltaTime)
  {
    base.OnUpdate(deltaTime);

    if (_spawnTimer.Tick(deltaTime))
    {
      Add(new RigidBody2D
      {
        GlobalPosition = GlobalPosition
      });

      _spawnTimer.Reset();
    }
  }
}
