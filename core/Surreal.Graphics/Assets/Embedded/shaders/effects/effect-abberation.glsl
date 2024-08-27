// An effect shader that shifts the red and blue channels in opposite directions.

#version 330 core

#shader_type vertex

layout (location = 0) in vec2 a_position;

void main()
{
  gl_Position = vec4(a_position, 0.0, 1.0);
}

#shader_type fragment

layout(location = 0) out vec4 o_color;

in vec4 v_color;

void main()
{
  o_color = v_color;
}
