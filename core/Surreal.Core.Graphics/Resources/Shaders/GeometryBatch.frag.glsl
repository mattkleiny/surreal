#version 330

varying vec4 v_color;

uniform sampler2D u_texture;

void main()
{
  gl_FragColor = v_color;
}