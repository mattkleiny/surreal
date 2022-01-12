// A simple sprite shader, for testing purposes.

#include "resx://Surreal.Graphics/Shaders/common.shader";
#include "resx://Surreal.Graphics/Shaders/sprites.shader";

#shader_type sprite;

uniform vec3 _Position;
uniform float _Intensity;
varying vec3 _Color;

float circle(float radius, int depth)
{
  // return 1f * MathF.Pi;
}

void vertex()
{
  // _Color = vec3(1,1,1) * _Position * _Intensity;
}

void fragment()
{
  // COLOR = _Color;
}
