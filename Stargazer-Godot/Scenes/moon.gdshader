shader_type spatial;
render_mode unshaded;

uniform sampler2D albedo_texture;
uniform vec3 target_direction = vec3(0.0,1.0,0.0);
uniform float threshold = 0.9;
void fragment() {


    // Ensure both vectors are normalized
    vec3 direction = normalize(target_direction);
	vec3 world_normal = normalize((INV_VIEW_MATRIX * vec4(NORMAL,0.0)).xyz);
    // Compute dot product between normal and target direction
    float alignment = dot(world_normal, direction);
	vec4 texture_color = texture(albedo_texture, UV);
	vec3 corrected_color = pow(texture_color.rgb, vec3(2));
	float intensity = smoothstep(0.2, 0.3, alignment);
	ALBEDO = mix(vec3(0.0),corrected_color.rgb,alignment);
	ALPHA = mix(0.0,1.0,intensity);



    //ALBEDO = corrected_color.rgb;
	//ALPHA = texture_color.a;
}