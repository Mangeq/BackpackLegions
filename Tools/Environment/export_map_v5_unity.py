import bpy,json,math
from pathlib import Path
from mathutils import Vector
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'Assets/Art/BlackpineV5Test';OUT.mkdir(parents=True,exist_ok=True)
scene=bpy.data.scenes['Blackpine_V5_UnityTower_Theme'];bpy.context.window.scene=scene
hidden={'V3_Base_Architecture','V3_Base_Details','SOURCE_UnityTower_Hidden','Tactical_Overlay'}
exportgroups={};materials={};deps=bpy.context.evaluated_depsgraph_get()
source=list(scene.objects)
for o in source:
    if o.type not in {'MESH','CURVE','FONT'} or o.hide_render or any(c.hide_render or c.name in hidden for c in o.users_collection):continue
    names=[c.name for c in o.users_collection]
    if 'Presentation' in names:continue
    group=names[0] if names else 'Environment'
    if group=='Tower_Pads_Exactly_5':
        # Preserve the five logical pad roots while batching their ring details.
        x,y=o.matrix_world.translation.x,o.matrix_world.translation.y
        positions=[(-17,-3.3),(-11,3.3),(-3,10.5),(2,-1.5),(17,4)]
        i=min(range(5),key=lambda i:(x-positions[i][0])**2+(y-positions[i][1])**2);group='Tower_Pad_%02d'%(i+1)
    ev=o.evaluated_get(deps);me=bpy.data.meshes.new_from_object(ev,depsgraph=deps)
    if not me or not me.polygons:continue
    me.transform(o.matrix_world)
    if group=='Road':me.flip_normals()
    # Generate a stable world-space grass UV instead of Blender-only Generated nodes.
    if any(m and m.name=='Textured meadow' for m in me.materials):
        uv=me.uv_layers.active or me.uv_layers.new(name='WorldUV')
        for loop in me.loops:
            p=me.vertices[loop.vertex_index].co;uv.data[loop.index].uv=(p.x/4,p.y/4)
    new=bpy.data.objects.new(o.name,me);scene.collection.objects.link(new);exportgroups.setdefault(group,[]).append(new)
    for m in me.materials:
        if not m or m.name in materials:continue
        p=m.node_tree.nodes.get('Principled BSDF') if m.use_nodes else None
        materials[m.name]={'name':m.name,'color':list(p.inputs['Base Color'].default_value if p else m.diffuse_color),'metallic':p.inputs['Metallic'].default_value if p else 0,'smoothness':1-p.inputs['Roughness'].default_value if p else .15,'grass':m.name=='Textured meadow','emission':list(p.inputs['Emission Color'].default_value) if p else [0,0,0,1],'emissionStrength':p.inputs['Emission Strength'].default_value if p else 0}
# Batch static pieces by semantic collection, retaining source hierarchy meaning.
exported=[];exportobjects=[]
for group,objs in exportgroups.items():
    bpy.ops.object.select_all(action='DESELECT')
    for o in objs:o.select_set(True)
    bpy.context.view_layer.objects.active=objs[0]
    if len(objs)>1:bpy.ops.object.join()
    o=bpy.context.object;o.name=group
    bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
    o.data.calc_loop_triangles();exported.append({'group':group,'triangles':len(o.data.loop_triangles)});exportobjects.append(o)
bpy.ops.object.select_all(action='DESELECT')
for o in exportobjects:o.select_set(True)
bpy.ops.export_scene.fbx(filepath=str(OUT/'Blackpine_V5_Map.fbx'),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,bake_anim=False,add_leaf_bones=False,use_mesh_modifiers=True)
(OUT/'materials.json').write_text(json.dumps(list(materials.values()),indent=2))
(ROOT/'ArtSource/BlackpineMapV5_TowerTheme/unity_export_report.json').write_text(json.dumps({'groups':exported,'triangles':sum(a['triangles'] for a in exported)},indent=2))
