#version 330

varying vec4 v_color;
varying vec2 v_texCoords;

uniform sampler2D u_texture;
uniform sampler2D u_palette;

vec4 PaletteShift(vec4 color)
{
  float index  = ceil(color.r * 3) / 4;
  vec4  target = texture2D(u_palette, vec2(index, 0));

  vec4 final = mix(color, target, target.a);

  final.a    = color.a;
  final.rgb *= color.a;

  return final;
}

void main()
{
  vec4 color = texture2D(u_texture, v_texCoords);
  vec4 final = PaletteShift(color);

  gl_FragColor = v_color * final;
}
