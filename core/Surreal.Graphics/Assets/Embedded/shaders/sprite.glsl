// Core shader for sprite rendering.

#version 330 core

#shader_type vertex

uniform mat4 u_transform = mat4(1.0);

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 uv;
layout (location = 2) in vec4 color;

out vec4 v_color;
out vec2 v_uv;

void main()
{
  v_color = color;
  v_uv = uv;

  gl_Position = u_transform * vec4(position, 0.0, 1.0);
}

#shader_type fragment

layout(location = 0) out vec4 color;

uniform sampler2D u_texture;

in vec2 v_uv;
in vec4 v_color;

void main()
{
  color = texture(u_texture, v_uv) * v_color;
}
