varying vec4 v_color;
varying vec4 v_normal;

void main()
{
  gl_FragColor = vec4(v_color.xyz, v_color.w);
}
