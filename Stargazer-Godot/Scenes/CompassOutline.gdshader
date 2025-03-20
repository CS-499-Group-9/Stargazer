shader_type canvas_item;

uniform float radius = 0.40;
uniform float thickness = 0.01;
uniform float feather = 0.01;
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float mask = smoothstep(radius-thickness-feather,radius-thickness,length(UV-vec2(0.5,0.5)))+smoothstep(radius+thickness+feather,radius+thickness,length(UV-vec2(0.5,0.5)))-1.0;
	COLOR = vec4(1.0,1.0,1.0,mask);

}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
