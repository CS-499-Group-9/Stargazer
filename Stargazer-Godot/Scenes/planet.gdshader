shader_type spatial;
render_mode unshaded;

uniform sampler2D albedo_texture;
uniform vec3 target_direction = vec3(0.0,1.0,0.0);
uniform float threshold = 0.2;
void fragment() {


    // Ensure both vectors are normalized
    vec3 direction = normalize(target_direction);

    // Compute dot product between normal and target direction
    float alignment = dot(NORMAL, direction);
	vec4 texture_color = texture(albedo_texture, UV);
	vec3 corrected_color = pow(texture_color.rgb, vec3(2));
    // Darken or set black if alignment is strong
    ALBEDO = corrected_color; // White or base color



    //ALBEDO = corrected_color.rgb;
	//ALPHA = texture_color.a;
}