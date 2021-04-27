from gimpfu import *

for x in range(1, 51):
	filename = "x:\path" + str(x) + ".png"
	image = pdb.gimp_file_load(filename, filename)
	pdb.gimp_colorize(image.active_layer, 50, 50, 100)
	pdb.script_fu_drop_shadow(image, image.active_layer, 0, 0, 7, "#000000", 100, 1)
	layer = pdb.gimp_image_merge_visible_layers(image, CLIP_TO_IMAGE)
	new_filename = "x:\path" + str(x) + ".png"
	pdb.gimp_file_save(image, layer, new_filename, new_filename)