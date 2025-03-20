shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}
uniform vec3 starColor = vec3(1.0,1.0,1.0);
void fragment() {
	// Called for every pixel the material is visible on.

	float mask = smoothstep(0.5,0.45,length(UV-vec2(0.5,0.5)));
	COLOR = vec4(starColor,mask);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
