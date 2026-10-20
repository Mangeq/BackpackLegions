import bpy, math, random, json
from mathutils import Vector
from pathlib import Path
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'ArtSource/BlackpineMapV4';OUT.mkdir(parents=True,exist_ok=True)
scene=bpy.data.scenes['Blackpine_Strategic_Map_V3_Base'];bpy.context.window.scene=scene;scene.name='Blackpine_V4_Compact_Outpost';random.seed(41)
for n in ['V3_Base_Architecture','V3_Base_Details']:
    c=bpy.data.collections[n];c.hide_render=True;c.hide_viewport=True
group=bpy.data.collections.new('V4_Compact_Dark_Base');scene.collection.children.link(group)
def mat(n,c,metal=0):
    m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);m.use_nodes=True;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*c,1);p.inputs['Roughness'].default_value=.9;p.inputs['Metallic'].default_value=metal;return m
stone=mat('V4 mossy charcoal stone',(.115,.135,.095));edge=mat('V4 worn olive stone',(.19,.205,.14));wood=mat('V4 dark aged oak',(.095,.055,.025));timber=mat('V4 warmer exposed timber',(.19,.115,.045));roof=mat('V4 weathered forest shingles',(.055,.105,.075));roof2=mat('V4 shingle variation',(.095,.135,.085));iron=mat('V4 black iron',(.04,.055,.045),.4);gold=mat('V4 aged bronze',(.37,.24,.065),.4);red=mat('V4 muted oxblood banner',(.20,.035,.015));moss=mat('V4 moss seams',(.17,.23,.045));earth=mat('V4 courtyard earth',(.17,.13,.06))
def link(o):
    for c in list(o.users_collection):c.objects.unlink(o)
    group.objects.link(o);return o
def box(n,p,s,m,b=.025):
    bpy.ops.mesh.primitive_cube_add(size=1,location=p);o=bpy.context.object;o.name=n;o.scale=s;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);link(o)
    if b:v=o.modifiers.new('Worn edges','BEVEL');v.width=b;v.segments=2
    return o
def cone(n,p,r,h,m,r2=None,verts=12):
    bpy.ops.mesh.primitive_cone_add(vertices=verts,radius1=r,radius2=r if r2 is None else r2,depth=h,location=p);o=bpy.context.object;o.name=n;o.data.materials.append(m);return link(o)
def beam(n,a,b,r=.07,m=timber):
    a,b=Vector(a),Vector(b);o=cone(n,(a+b)/2,r,(b-a).length,m);o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();return o
def mesh(n,vs,fs,m):
    me=bpy.data.meshes.new(n);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(n,me);group.objects.link(o);me.materials.append(m);return o
# 6.2 x 6.2 m footprint: about 55% less area than the V3 enclosure.
box('Small earthen outpost yard',(24.7,9.1,.09),(6.2,6.2,.16),earth,.10)
for i in range(14):
    x=random.uniform(21.9,27.4);y=random.uniform(6.2,12.0)
    box('Sparse courtyard stepping stone',(x,y,.19),(random.uniform(.3,.6),random.uniform(.3,.55),.06),edge,.07)
# One low timber lodge replaces the fortress and watchtowers.
box('Low rubble foundation',(25.7,9.65,.38),(3.1,3.9,.55),stone,.10)
box('Timber lodge body',(25.7,9.65,1.40),(2.85,3.65,1.6),wood,.04)
for x in [24.24,27.16]:
    for y in [7.8,9.65,11.5]:box('Exposed oak post',(x,y,1.45),(.13,.14,1.85),timber)
for z in [.65,1.65,2.18]:
    for x in [24.23,27.17]:box('Horizontal timber frame',(x,9.65,z),(.14,3.8,.11),timber)
for j in range(9):
    y=7.9+j*.41
    for x in [24.26,27.14]:box('Wall plank seam',(x,y,1.40),(.025,.035,1.5),iron,.004)
for y in [7.78,11.52]:
    mesh('Closed timber gable',[(24.08,y,2.18),(27.32,y,2.18),(25.7,y,3.18)],[(0,1,2)],wood)
    for x in [24.8,25.7,26.6]:box('Front facade post',(x,y,1.45),(.10,.10,1.7),timber)
box('West facing oak door',(24.19,8.9,1.27),(.06,.80,1.36),timber,.04)
for z in [.85,1.55]:box('Door iron strap',(24.15,8.9,z),(.025,.76,.055),iron,.005)
cone('Bronze door handle',(24.10,8.66,1.18),.045,.035,gold)
for y in [10.6]:
    box('Small inset window',(24.19,y,1.55),(.05,.55,.55),iron,.03)
    box('Window wood mullion',(24.15,y,1.55),(.04,.04,.53),timber,.005)
    box('Window crossbar',(24.15,y,1.55),(.04,.52,.04),timber,.005)
for side in [-1,1]:
    for row in range(8):
        x=25.7+side*(.12+row*.21);z=3.18-row*.13
        for j in range(8):
            o=box('Aged forest roof shingle',(x,7.65+j*.57,z),(.34,.55,.07),roof if random.random()<.7 else roof2,.015);o.rotation_euler.y=side*.55
beam('Lodge ridge cap',(25.7,7.42,3.27),(25.7,11.96,3.27),.085,iron)
box('Small stone flue',(26.35,10.8,3.01),(.35,.38,.85),stone,.035);box('Flue cap',(26.35,10.8,3.46),(.47,.49,.12),edge)
# Low wall segments with timber palisades, no battlements or white paving.
for y in [6.0,12.2]:
    for j in range(11):
        x=21.8+j*.56
        for row in range(2):box('Low dry stone boundary',(x,y,.32+row*.26),(.54,.40,.25),stone if random.random()<.8 else edge,.05)
        if j%2==0:
            cone('Rough palisade upright',(x,y,.98),.075,.88,wood,r2=.055);cone('Sharpened wooden tip',(x,y,1.48),.058,.18,timber,r2=0)
    beam('Boundary timber rail',(21.7,y,1.1),(27.5,y,1.1),.055,wood)
for y in [6.6,7.2,11,11.6]:
    cone('Entrance palisade post',(21.55,y,.68),.10,1.1,wood,r2=.08);cone('Entrance pointed cap',(21.55,y,1.3),.08,.2,timber,r2=0)
for y in [7.75,10.45]:beam('Small gateway post',(21.5,y,.15),(21.5,y,1.95),.12,wood)
beam('Simple gateway lintel',(21.5,7.55,1.95),(21.5,10.65,1.95),.13,timber)
# Restrained bronze and red banner provides a recognizable base silhouette.
mesh('Small legion banner',[(25.1,7.69,2.03),(25.85,7.69,2.03),(25.85,7.69,.90),(25.48,7.69,1.05),(25.1,7.69,.90)],[(0,1,2,3,4)],red)
box('Banner bronze emblem',(25.48,7.675,1.55),(.05,.02,.44),gold,.003);box('Banner emblem bar',(25.48,7.67,1.62),(.26,.02,.05),gold,.003)
# Small supplies rather than an oversized courtyard.
for x,y in [(22.5,11.1),(23.1,11.15)]:
    box('Outpost provision crate',(x,y,.43),(.55,.55,.55),wood)
    for z in [.22,.62]:box('Crate slat',(x,y-.29,z),(.57,.055,.06),timber)
for x,y in [(26.8,6.8),(27.25,7.15)]:
    cone('Outpost oak barrel',(x,y,.46),.24,.65,wood,r2=.22)
    for z in [.24,.66]:cone('Barrel dark hoop',(x,y,z),.245,.04,iron)
for y in [8.0,10.2]:
    cone('Small gate brazier pedestal',(21.1,y,.37),.14,.45,stone,verts=8);cone('Small iron brazier',(21.1,y,.64),.20,.16,iron,r2=.25)
    flame=bpy.data.materials.get('Warm flame');cone('Warm base flame',(21.1,y,.83),.12,.26,flame or gold,r2=0,verts=7)
for i in range(8):
    x=random.uniform(21.9,27.4);y=random.choice([6.3,11.85]);cone('Moss near low boundary',(x,y,.20),random.uniform(.12,.25),.025,moss,verts=7)
# Replant the strip vacated by the large V3 fortress, keeping pad 05 open.
tree=next(o for o in scene.objects if o.name.startswith('Pine_Young') and o.type=='MESH')
shrub=next(o for o in scene.objects if o.name.startswith('Shrub_1') and o.type=='MESH')
for x,y,s in [(20.7,13.8,.55),(23.0,14.3,.7),(25.3,14.5,.6),(27.0,13.8,.5),(22.0,4.7,.5),(24.5,4.9,.55),(27.0,4.8,.55)]:
    o=tree.copy();o.data=tree.data;group.objects.link(o);o.name='Regrown young pine';o.location=(x,y,0);o.scale=(s,)*3;o.hide_render=False;o.hide_viewport=False;o.hide_set(False)
for i in range(14):
    o=shrub.copy();o.data=shrub.data;group.objects.link(o);o.name='Regrown base edge shrub';o.location=(random.uniform(20.4,27.3),random.choice([5.0,13.4])+random.uniform(-.4,.4),0);o.scale=(.4,)*3;o.hide_render=False;o.hide_viewport=False;o.hide_set(False)
scene['base_revision']='Compact dark timber outpost; no castle towers';scene['unity_exported']=False
cam=scene.camera;cam.location=(46,-60,70);cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=77
scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=2048;scene.render.resolution_y=1536;scene.render.resolution_percentage=100
scene.render.filepath=str(OUT/'Map_V4_Compact_Base.png');bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_V4_Compact_Base.blend'))
bpy.ops.render.render(write_still=True)
cam.location=(36,-5,17);cam.rotation_euler=(Vector((24.7,9.2,1.4))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=14.5;scene.render.resolution_x=1600;scene.render.resolution_y=1300;scene.render.filepath=str(OUT/'Base_V4_Closeup.png');bpy.ops.render.render(write_still=True)
(OUT/'revision.json').write_text(json.dumps({'footprint_m':[6.2,6.2],'previous_footprint_m':[8.0,10.8],'area_reduction_percent':55.5,'roof_height_m':3.3,'previous_roof_height_m':5.3,'tower_pads':5,'unity_exported':False},indent=2))
