// Core shader for full-viewport 'canvas' rendering.

#version 330 core

#shader_type vertex

uniform vec2 u_screenSize = vec2(0.0);

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec4 a_color;
layout (location = 2) in vec2 a_uv;

out vec4 v_color;
out vec2 v_uv;

void main()
{
  v_color = a_color;
  v_uv = a_uv;

  gl_Position = vec4(
    2.0 * a_position.x / u_screenSize.x - 1.0,
    1.0 - 2.0 * a_position.y / u_screenSize.y,
    0.0,
    1.0
  );
}

#shader_type fragment

uniform sampler2D u_texture;

in vec2 v_uv;
in vec4 v_color;

void main()
{
  gl_FragColor = texture(u_texture, v_uv) * v_color;
}
