shader_type spatial;
render_mode unshaded;

uniform sampler2D albedo_texture;
void fragment() {
    vec3 view_dir = normalize(VIEW);

    // Rotation angle (30 degrees in radians)
    float angle = radians(00.0);
    float cos_a = cos(angle);
    float sin_a = sin(angle);

    // Rotate NORMAL around the Y-axis
    vec3 rotated_normal = vec3(
        cos_a * NORMAL.x + sin_a * NORMAL.z, // X' = cos(θ) * X + sin(θ) * Z
        NORMAL.y,                            // Y' stays the same
        -sin_a * NORMAL.x + cos_a * NORMAL.z // Z' = -sin(θ) * X + cos(θ) * Z
    );

    float intensity = dot(rotated_normal, view_dir);
    
    // Convert to black-and-white shading
    //float visibility = step(0.0, intensity);
	vec4 texture_color = texture(albedo_texture, UV);
	vec3 corrected_color = pow(texture_color.rgb, vec3(2)); 
    ALBEDO = corrected_color.rgb;
	ALPHA = texture_color.a;
}