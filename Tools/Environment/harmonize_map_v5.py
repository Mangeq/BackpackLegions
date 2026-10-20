import bpy,math,json,random
from pathlib import Path
from mathutils import Vector
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'ArtSource/BlackpineMapV5_TowerTheme';OUT.mkdir(parents=True,exist_ok=True)
scene=bpy.data.scenes['Blackpine_V4_Compact_Outpost'];bpy.context.window.scene=scene;scene.name='Blackpine_V5_UnityTower_Theme';random.seed(59)
ref=next(Path('C:/Users/Emir/Desktop').rglob('UnityTower.fbx'));before=set(scene.objects);bpy.ops.import_scene.fbx(filepath=str(ref));added=set(scene.objects)-before;meshes=[o for o in added if o.type=='MESH']
reference_materials={m.name:m for o in meshes for m in o.data.materials if m}
def slot(name,i=0):
    o=next(o for o in meshes if o.name==name or o.name.startswith(name+'.'))
    return o.data.materials[i]
grey=slot('Pillar High');oak=slot('Main alt');brass=slot('Metal.001');stone=slot('Flag Metal');deep=slot('Tower HighMid');flag=slot('Flag Top');wood=slot('Tower High');black=slot('Flag Top',1);door=slot('BigDoor')
def copy_mat(src,name):
    m=src.copy();m.name=name;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Roughness'].default_value=.82;return m
stone=copy_mat(stone,'V5 tower-matched muted grey trim');grey=copy_mat(grey,'V5 tower-matched charcoal framing');oak=copy_mat(oak,'V5 tower-matched warm oak');brass=copy_mat(brass,'V5 tower-matched brass');flag=copy_mat(flag,'V5 tower-matched ochre cloth');black=copy_mat(black,'V5 tower-matched black emblem');wood=copy_mat(wood,'V5 tower-matched chestnut roof');door=copy_mat(door,'V5 tower-matched door wood')
# The incoming FBX material palette drives all built architecture. Ground and
# foliage retain natural muted greens to frame the tower without visual noise.
mapping={
'V4 mossy charcoal stone':grey,'V4 worn olive stone':stone,'V4 dark aged oak':deep,'V4 warmer exposed timber':oak,'V4 weathered forest shingles':wood,'V4 shingle variation':door,'V4 black iron':grey,'V4 aged bronze':brass,'V4 muted oxblood banner':flag,
'Warm hewn limestone':grey,'Worn stone edges':stone,'Slate inset':grey,'Weathered timber':door,'End grain':oak,'Blackened iron':grey,'Muted brass':brass,'Legion oxblood banner':flag,'Faded canvas':flag,
'BP_Stone':grey,'BP_StoneLight':stone,'BP_StoneDark':grey,'BP_Wood':door,'BP_WoodLight':oak,'BP_Gold':brass,'BP_Iron':grey,'BP_Banner':flag,
}
changed=0
for o in list(scene.objects):
    if o in added or o.type not in {'MESH','CURVE','FONT'}:continue
    if o.name.startswith(('Moss_Rock','Cliff_Cluster','V3 Moss','V3_')) and any(c.name in ['Rocks','River','Forest','V3_Understory'] for c in o.users_collection):continue
    if not o.data or not hasattr(o.data,'materials'):continue
    # Copy shared mesh data before changing slots to preserve other source scenes.
    replacements=[mapping.get(m.name,m) if m else m for m in o.data.materials]
    if any(a!=b for a,b in zip(replacements,o.data.materials)):
        o.data=o.data.copy()
        for i,m in enumerate(replacements):o.data.materials[i]=m
        changed+=1
# Refine the base with the same broad corner bands and small brass studs.
coll=bpy.data.collections.new('V5_Tower_Style_Details');scene.collection.children.link(coll)
def box(n,p,s,m,bevel=.035):
    bpy.ops.mesh.primitive_cube_add(size=1,location=p);o=bpy.context.object;o.name=n;o.scale=s;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
    for c in list(o.users_collection):c.objects.unlink(o)
    coll.objects.link(o);o.data.materials.append(m)
    if bevel:v=o.modifiers.new('Tower-like softened edge','BEVEL');v.width=bevel;v.segments=2
    return o
for x in [24.22,27.18]:
    for y in [7.78,11.52]:
        for z in [.72,1.43,2.14]:box('Matching charcoal corner collar',(x,y,z),(.25,.25,.17),grey)
for x in [24.6,25.3,26.0,26.7]:
    o=box('Small brass facade stud',(x,7.70,2.1),(.10,.045,.10),brass,.012);o.rotation_euler.y=math.pi/4
# Existing cross emblems are replaced by a dark inset heraldic shield, matching
# the tower's black/ochre banner treatment rather than the old red/gold theme.
for o in list(scene.objects):
    if o.name.startswith(('Banner bronze emblem','Banner emblem bar')):o.hide_render=True;o.hide_set(True)
vs=[(25.28,7.65,1.83),(25.68,7.65,1.83),(25.65,7.65,1.46),(25.48,7.65,1.30),(25.31,7.65,1.46)]
me=bpy.data.meshes.new('Black shield emblem');me.from_pydata(vs,[],[(0,1,2,3,4)]);me.update();o=bpy.data.objects.new('Tower-family banner emblem',me);coll.objects.link(o);me.materials.append(black)
# Calm the photographic grass with a uniform green tint, retaining texture.
grass=bpy.data.materials.get('Textured meadow')
if grass:
    p=grass.node_tree.nodes.get('Principled BSDF');links=grass.node_tree.links;incoming=next((l.from_socket for l in links if l.to_node==p and l.to_socket==p.inputs['Base Color']),None)
    if incoming:
        mix=grass.node_tree.nodes.new('ShaderNodeMixRGB');mix.blend_type='MIX';mix.inputs[0].default_value=.38;mix.inputs[2].default_value=(.16,.205,.065,1);links.new(incoming,mix.inputs[1]);links.new(mix.outputs[0],p.inputs['Base Color'])
# A single unchanged reference tower previews compatibility on pad 02.
verts=[];faces=[];mats=[];indices=[]
for src in meshes:
    offset=len(verts);verts.extend([src.matrix_world@v.co for v in src.data.vertices]);local=[]
    for m in src.data.materials:
        if m not in mats:mats.append(m)
        local.append(mats.index(m))
    for poly in src.data.polygons:faces.append(tuple(offset+i for i in poly.vertices));indices.append(local[poly.material_index] if local else 0)
lo=Vector(tuple(min(v[i] for v in verts) for i in range(3)));hi=Vector(tuple(max(v[i] for v in verts) for i in range(3)));center=(lo+hi)/2
scale=2.35/max(hi.x-lo.x,hi.y-lo.y)
verts=[((v.x-center.x)*scale,(v.y-center.y)*scale,(v.z-lo.z)*scale) for v in verts]
me=bpy.data.meshes.new('UnityTower reference geometry');me.from_pydata(verts,[],faces);me.update()
for m in mats:me.materials.append(m)
for p,i in zip(me.polygons,indices):p.material_index=i
preview=bpy.data.collections.new('REFERENCE_PREVIEW_Tower_On_Pad_02');scene.collection.children.link(preview);o=bpy.data.objects.new('UnityTower — reference preview, not gameplay',me);preview.objects.link(o);o.location=(-11,3.3,.13);o.rotation_euler.z=math.pi/2
# Imported originals remain hidden and organized, never exported to Unity.
archive=bpy.data.collections.new('SOURCE_UnityTower_Hidden');scene.collection.children.link(archive);archive.hide_render=True;archive.hide_viewport=True
for src in added:
    for c in list(src.users_collection):c.objects.unlink(src)
    archive.objects.link(src)
scene['theme_reference']='UnityTower.fbx';scene['reference_preview']='One tower on pad 02; four other pads empty';scene['unity_exported']=False
cam=scene.camera;cam.location=(46,-60,70);cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=77
scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=2048;scene.render.resolution_y=1536;scene.render.resolution_percentage=100;scene.render.filepath=str(OUT/'Map_V5_Tower_Theme.png')
bpy.ops.file.pack_all();bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_V5_Tower_Theme.blend'));bpy.ops.render.render(write_still=True)
cam.location=(36,-5,17);cam.rotation_euler=(Vector((24.7,9.2,1.4))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=14.5;scene.render.resolution_x=1600;scene.render.resolution_y=1300;scene.render.filepath=str(OUT/'Base_V5_Tower_Theme.png');bpy.ops.render.render(write_still=True)
cam.location=(1,-12,15);cam.rotation_euler=(Vector((-10,3.3,1.4))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=15;scene.render.filepath=str(OUT/'Tower_V5_In_Map.png');bpy.ops.render.render(write_still=True)
(OUT/'theme_report.json').write_text(json.dumps({'reference':str(ref),'changed_objects':changed,'reference_height_m':(hi.z-lo.z)*scale,'base_footprint_m':[6.2,6.2],'tower_pads':5,'preview_towers':1,'unity_exported':False},indent=2))
