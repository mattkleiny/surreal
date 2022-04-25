﻿using Surreal.Assets;
using Surreal.IO;
using Surreal.Timing;

namespace Surreal;

/// <summary>Represents the underlying platform host for the engine.</summary>
public interface IPlatformHost : IDisposable
{
  event Action<int, int> Resized;

  int  Width     { get; }
  int  Height    { get; }
  bool IsVisible { get; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  void RegisterServices(IServiceRegistry services);
  void RegisterAssetLoaders(IAssetManager manager);
  void RegisterFileSystems(IFileSystemRegistry registry);

  void BeginFrame(DeltaTime deltaTime);
  void EndFrame(DeltaTime deltaTime);
}
