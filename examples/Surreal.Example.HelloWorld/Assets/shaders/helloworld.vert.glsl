#version 330 core

uniform mat4 u_projectionView;

in vec2 in_position;
in vec4 in_color;

out vec4 color;

void main()
{
    gl_Position = vec4(in_position, 0.0, 1.0) * u_projectionView;
    color = in_color;
}
