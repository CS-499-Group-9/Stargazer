extends Node

# Replace this with the actual path to your dialog box
@onready var dialog_box = $AcceptDialog

func _ready():
	dialog_box.visible = false

func _input(event):
	if event.is_action_pressed("credit_key"):
		dialog_box.visible = true
