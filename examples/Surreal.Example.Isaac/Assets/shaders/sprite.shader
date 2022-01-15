﻿// Our standard sprite shader.

#include "Assets/shaders/common.shader";

#shader_type sprite;

uniform vec3 _Tint;
varying vec3 _Color;

void vertex()
{
  _Color = vec3(1,1,1) * _Position * _Intensity;
}

void fragment()
{
  COLOR = sample_palette(_Color) * _Tint;
}
