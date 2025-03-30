@tool
extends EditorScript

func _run():
	var textures_path = "res://Textures/Images/"  # Change this to your folder path
	var dir = DirAccess.open(textures_path)
	
	if not dir:
		push_error("Failed to open directory!")
		return
	
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	while file_name != "":
		if file_name.to_lower().ends_with(".jpg") or file_name.to_lower().ends_with(".jpeg"):
			var texture_path = textures_path + file_name
			var texture = load(texture_path) as Texture2D
			
			if texture:
				var resource_path = textures_path + file_name.get_basename() + ".tres"
				var result = ResourceSaver.save(texture, resource_path)
				if result == OK:
					print("Saved:", resource_path)
				else:
					push_error("Failed to save:", resource_path)
		
		file_name = dir.get_next()
	
	print("All textures converted!")
