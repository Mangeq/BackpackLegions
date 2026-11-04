import bpy
from pathlib import Path
root=Path('C:/Users/Emir/BackPackDefance/ArtSource/CampaignMaps')
for file in sorted(root.glob('*.blend')):
 bpy.ops.wm.open_mainfile(filepath=str(file))
 for o in bpy.data.objects:
  if o.type=='LIGHT' and o.data.type=='AREA':o.data.energy=15000
 bpy.ops.object.light_add(type='SUN',location=(-12,-15,28))
 bpy.context.object.rotation_euler=(.45,-.4,-.5);bpy.context.object.data.energy=2.5;bpy.context.object.data.angle=.15
 bpy.context.preferences.filepaths.save_version=0
 bpy.ops.wm.save_as_mainfile(filepath=str(file))
 bpy.context.scene.render.filepath=str(file.with_suffix('.png'))
 bpy.ops.render.render(write_still=True)
