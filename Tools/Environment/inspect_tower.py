import bpy,json
from pathlib import Path
from mathutils import Vector
root=Path('C:/Users/Emir/BackPackDefance/ArtSource/TowerReference');root.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
reference=next(Path('C:/Users/Emir/Desktop').rglob('UnityTower.fbx'))
bpy.ops.import_scene.fbx(filepath=str(reference))
objects=[o for o in bpy.context.scene.objects if o.type=='MESH']
bounds=[o.matrix_world@Vector(c) for o in objects for c in o.bound_box]
lo=Vector(tuple(min(p[i] for p in bounds) for i in range(3)));hi=Vector(tuple(max(p[i] for p in bounds) for i in range(3)));center=(lo+hi)/2;size=max(hi-lo)
info={'bounds':{'min':list(lo),'max':list(hi)},'objects':[{'name':o.name,'vertices':len(o.data.vertices),'materials':[m.name for m in o.data.materials if m]} for o in objects],'materials':[]}
for m in bpy.data.materials:
    p=m.node_tree.nodes.get('Principled BSDF') if m.use_nodes else None
    info['materials'].append({'name':m.name,'color':list(p.inputs['Base Color'].default_value if p else m.diffuse_color),'textures':[n.image.filepath for n in m.node_tree.nodes if n.type=='TEX_IMAGE' and n.image] if m.use_nodes else []})
(root/'inspection.json').write_text(json.dumps(info,indent=2))
scene=bpy.context.scene;scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=1200;scene.render.resolution_y=1200;scene.render.resolution_percentage=100
bpy.ops.object.camera_add(location=center+Vector((1.3,-1.8,1.2))*size);cam=bpy.context.object;cam.rotation_euler=(center-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='ORTHO';cam.data.ortho_scale=size*1.4;scene.camera=cam
bpy.ops.object.light_add(type='AREA',location=center+Vector((1,-1,2))*size);bpy.context.object.data.energy=80*size*size;bpy.context.object.data.size=size*3
bpy.ops.object.light_add(type='SUN');bpy.context.object.rotation_euler=(.5,-.5,-.5);bpy.context.object.data.energy=2
scene.world=bpy.data.worlds.new('Studio');scene.world.use_nodes=True;scene.world.node_tree.nodes['Background'].inputs[0].default_value=(.15,.18,.20,1);scene.world.node_tree.nodes['Background'].inputs[1].default_value=.7
scene.render.filepath=str(root/'Tower_Reference.png');bpy.ops.wm.save_as_mainfile(filepath=str(root/'Tower_Reference.blend'));bpy.ops.render.render(write_still=True)
