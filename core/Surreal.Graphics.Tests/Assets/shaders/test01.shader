// A simple sprite shader, for testing purposes.

#include "resx://Surreal.Graphics/Resources/Shaders/sprites.shader";

#shader_type sprite;

uniform lowp vec3 _Position;
uniform lowp float _Intensity;
varying vec3 _Color;

void vertex()
{
  POSITION = _Position + sin(_Intensity) * _Position.x;
}

void fragment()
{
  COLOR = _Color;
}
