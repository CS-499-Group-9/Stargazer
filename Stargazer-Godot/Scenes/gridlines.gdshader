

shader_type canvas_item;

uniform vec2 center = vec2(0.5, 0.5); // Center point (in UV space)
uniform float line_thickness = 0.10;    // Radial line thickness (in pixels)
uniform float angle_spacing = 15.0;    // Radial line spacing (degrees)

uniform float circle_spacing = 0.08;   // Distance between concentric circles (in pixels)
uniform float circle_thickness = 0.002;  // Thickness of the circle lines (in pixels)

void fragment()
{
    // Calculate polar coordinates
    vec2 polar_pos = (UV - center); // Scale to world space
    float angle = atan(polar_pos.y, polar_pos.x);       // Angle in radians
    float radial_distance = length(polar_pos);

    // ----------- RADIAL LINES -----------
    float degrees = degrees(angle);
    if (degrees < 0.0) {
        degrees += 360.0;
    }

    float radial_line_position = mod(degrees, angle_spacing);
    float thickness_scaled = line_thickness / radial_distance;
    float radial_mask = smoothstep(thickness_scaled, 0.0, min(radial_line_position, angle_spacing - radial_line_position));

    // ----------- CONCENTRIC CIRCLES -----------
    float circle_line_position = mod(radial_distance, circle_spacing);
    float circle_mask = smoothstep(circle_thickness, 0.0, min(circle_line_position, circle_spacing - circle_line_position));

    // ----------- COMBINED MASK -----------
    float combined_mask = max(radial_mask, circle_mask);

    // Final color — white grid lines on transparent background
    COLOR = vec4(vec3(combined_mask)*0.4, smoothstep(0.89,0.9,combined_mask));
}