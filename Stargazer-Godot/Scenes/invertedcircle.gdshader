shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}
void fragment() {
	vec2 center = vec2(0.5,0.5);
	float dist = length(UV-center);
	COLOR = vec4(0.0,0.0,0.2,smoothstep(0.4268,0.4269,dist));
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}