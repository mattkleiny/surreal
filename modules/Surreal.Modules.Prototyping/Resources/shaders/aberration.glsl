// Implements a simple chromatic aberration effect.

#version 330 core

#shader_type vertex

uniform mat4 u_projectionView = mat4(1.0);

layout (location = 0) in vec2 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uv;

out vec2 v_uv;

void main()
{
  v_uv = uv;

  // TODO: sort out y-flipping in post effects?
  gl_Position = vec4(position.x, -position.y, 0.0, 1.0) * u_projectionView;
}

#shader_type fragment

layout(location = 0) out vec4 color;

uniform sampler2D u_texture;
uniform float u_intensity;

in vec2 v_uv;

void main()
{
  color.r = texture(u_texture, vec2(v_uv.x + -1 * u_intensity, v_uv.y + -1 * u_intensity)).r;
  color.g = texture(u_texture, vec2(v_uv.x +  0 * u_intensity, v_uv.y +  0 * u_intensity)).g;
  color.b = texture(u_texture, vec2(v_uv.x + +1 * u_intensity, v_uv.y + +1 * u_intensity)).b;
  color.a = 1;
}
