﻿using Surreal.Mathematics.Timing;

namespace Surreal.Graphics.Cameras {
  public interface ICameraController {
    void Input(DeltaTime deltaTime);
  }
}