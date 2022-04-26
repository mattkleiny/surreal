#version 330 core

uniform sampler2D u_texture;

in vec2 o_uv;
in vec4 o_color;

out vec4 FragColor;

void main()
{
    FragColor = texture(u_texture, o_uv) * o_color;
}
