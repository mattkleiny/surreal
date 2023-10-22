// Core shader for image-to-image blits

#version 330 core

#shader_type vertex

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec2 a_uv;

out vec2 v_uv;

void main()
{
  v_uv = a_uv;

  gl_Position = vec4(a_position, 0.0, 1.0);
}

#shader_type fragment

uniform sampler2D u_texture;

in vec2 v_uv;

void main()
{
  vec2 uv = vec2(v_uv.x, -v_uv.y);

  gl_FragColor = texture(u_texture, uv);
}
