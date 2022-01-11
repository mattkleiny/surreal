// A simple sprite shader, for testing purposes.
// This description should be attached up-front.

#include "resx://Surreal.Graphics/Resources/SpriteBatch.shader"

shader_type sprite;

uniform vec3 _Position;
uniform float _Intensity;
varying vec3 _Color;

void vertex()
{
  _Color = vec3(1,1,1) * _Position * _Intensity;
}

void fragment()
{
  COLOR = _Color;
}
