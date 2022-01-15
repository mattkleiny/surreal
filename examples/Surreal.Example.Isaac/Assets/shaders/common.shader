// Common shader code for the entire project.

#include "resx://Surreal.Graphics/Resources/shaders/common.shader";

uniform sampler2D _CombinedPaletteTex;
uniform int _CombinedPaletteWidth;

vec4 sample_palette(vec4 color)
{
  // TODO: sample the color palette texture
  return color;
}
