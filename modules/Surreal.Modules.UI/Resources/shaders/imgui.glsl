// Core shader for immediate mode UI rendering.

#version 330 core

#shader_type vertex

uniform mat4 u_projectionView;

layout (location = 0) in vec2 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uv;
layout (location = 3) in int  textureIndex;

out vec4 v_color;
out vec2 v_uv;
out int v_textureIndex;

void main()
{
    v_color = color;
    v_uv = uv;
    v_textureIndex = textureIndex;

    gl_Position = vec4(position, 0.0, 1.0) * u_projectionView;
}

#shader_type fragment

layout(location = 0) out vec4 color;

uniform sampler2D u_texture[32];

in vec2 v_uv;
in vec4 v_color;
in int v_textureIndex;

void main()
{
  if (v_textureIndex == -1) {
    return v_color;
  }

  color = texture(u_texture[v_textureIndex], v_uv) * v_color;
}
