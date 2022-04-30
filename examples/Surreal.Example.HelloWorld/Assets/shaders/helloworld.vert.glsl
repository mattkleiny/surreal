#version 330 core

uniform mat4 u_projectionView;

layout (location = 0) in vec2 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uv;

out vec4 o_color;
out vec2 o_uv;

void main() {
  o_color = color;
  o_uv = uv;

  gl_Position = vec4(position, 0.0, 1.0) * u_projectionView;
}
