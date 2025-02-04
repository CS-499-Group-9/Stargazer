shader_type spatial;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uv = vec2(atan(VERTEX.z, VERTEX.x) / (2.0 * PI) + 0.5, asin(VERTEX.y) / PI + 0.5);
	float azimuth_lines = mod(uv.x * 36.0, 1.0) < 0.05 ? 1.0 : 0.0;
	vec3 grid_color = vec3(azimuth_lines);
	
	ALBEDO = grid_color;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
