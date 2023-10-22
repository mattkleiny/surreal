﻿// Core shader for un-textured wire rendering.

#version 330 core

#shader_type vertex

uniform mat4 u_projectionView = mat4(1.0);

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uv;

out vec4 v_color;

void main()
{
  v_color = color;

  gl_Position = vec4(a_position, 0.0, 1.0) * u_projectionView;
}

#shader_type fragment

layout(location = 0) out vec4 color;

in vec4 v_color;

void main()
{
  color = v_color;
}