﻿using System;
using Surreal.IO;

namespace Surreal.Graphics.Textures {
  public interface ITextureData {
    TextureFormat Format { get; }

    int  Width  { get; }
    int  Height { get; }
    Size Size   { get; }

    Span<Color> Span { get; }
  }
}