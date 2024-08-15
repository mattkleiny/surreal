// Core shader for image-to-image blits

#version 330 core

#shader_type vertex

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec2 a_texcoord;

out vec2 v_texcoord;

void main()
{
  v_texcoord = a_texcoord;

  gl_Position = vec4(a_position, 0.0, 1.0);
}

#shader_type fragment

layout(location = 0) out vec4 color;

uniform sampler2D u_texture;

in vec2 v_texcoord;

void main()
{
  vec2 uv = vec2(v_texcoord.x, -v_texcoord.y);

  color = texture(u_texture, uv);
}
