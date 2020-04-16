#version 330

attribute vec4 a_position;
attribute vec4 a_color;

uniform mat4 u_projView;

varying vec4 v_color;

void main()
{
  v_color = a_color;

  gl_Position = u_projView * a_position;
}