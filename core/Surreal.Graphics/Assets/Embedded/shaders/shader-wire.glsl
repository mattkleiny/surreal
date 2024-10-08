﻿// Core shader for un-textured wire rendering.

#version 330 core

#shader_type vertex

uniform mat4 u_transform = mat4(1.0);
uniform vec4 u_color = vec4(1.0);

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec2 a_uv;
layout (location = 2) in vec4 a_color;

out vec4 v_color;

void main()
{
  v_color = a_color * u_color;
  gl_Position = u_transform * vec4(a_position, 0.0, 1.0);
}

#shader_type fragment

layout(location = 0) out vec4 o_color;

in vec4 v_color;

void main()
{
  o_color = v_color;
}
