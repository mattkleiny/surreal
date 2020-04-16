attribute vec4 a_position;
attribute vec4 a_normal;
attribute vec4 a_color;
attribute vec2 a_texCoord0;

uniform mat4 u_projView;
uniform vec3 u_offset;

varying vec4 v_normal;
varying vec4 v_color;

const float epsilon = 1.001;

void main()
{
  v_normal = a_normal;
  v_color = a_color;

  gl_Position = u_projView * (vec4(u_offset.xyz, 1) + a_position * epsilon);
}
