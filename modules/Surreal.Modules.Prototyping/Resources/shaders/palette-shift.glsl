// Implements a simple palette shifting effect for sprites.

#version 330 core

#shader_type vertex

uniform mat4 u_projectionView = mat4(1.0);

layout (location = 0) in vec2 position;
layout (location = 1) in vec4 color;
layout (location = 2) in vec2 uv;

out vec4 v_color;
out vec2 v_uv;

void main()
{
  v_color = color;
  v_uv = uv;

  gl_Position = vec4(position, 0.0, 1.0) * u_projectionView;
}

#shader_type fragment

layout(location = 0) out vec4 color;

uniform sampler2D u_texture;
uniform sampler2D u_palette;
uniform int u_paletteWidth;

in vec2 v_uv;
in vec4 v_color;

// Performs a simple palette-shifting effect on the given color.
vec4 sample_palette(vec4 color, int channel)
{
  float normalized_y = channel * 0.25 - 0.125;

  // sample the palette texture, discretizing source colors into N equal quadrants across each of the palettes.
  float index = ceil(color.r * (u_paletteWidth - 1)) / u_paletteWidth;
  vec4 final = texture(u_palette, vec2(index, normalized_y));

  final.a = color.a;
  final.rgb *= color.a;

  return final;
}

void main()
{
  vec4 main_color = texture(u_texture, v_uv);
  vec4 final_color = sample_palette(main_color, 1);

  color = final_color * v_color;
}
