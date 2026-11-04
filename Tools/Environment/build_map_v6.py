import bpy,math,random,json
from pathlib import Path
from mathutils import Vector
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'ArtSource/BlackpineMapV6_ForestCastle';OUT.mkdir(parents=True,exist_ok=True)
scene=bpy.data.scenes['Blackpine_V5_UnityTower_Theme'];bpy.context.window.scene=scene;scene.name='Blackpine_V6_84x72_Forest_Castle';random.seed(641)
groups={}
for n in ['V6_Forest_Extension','V6_Forest_Understory','V6_Castle','V6_Castle_Details','V6_River_Extension','V6_Entrance_Road']:
    c=bpy.data.collections.new(n);scene.collection.children.link(c);groups[n]=c
def mat(n,c):
    m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);m.use_nodes=True;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*c,1);p.inputs['Roughness'].default_value=.85;return m
def recolor(name,c):
    m=bpy.data.materials.get(name)
    if m:
        m.diffuse_color=(*c,1);p=m.node_tree.nodes.get('Principled BSDF')
        if p:p.inputs['Base Color'].default_value=(*c,1)
# Lift midtones, preserving the tower's warm oak / grey / ochre family.
for n,c in {
'V5 tower-matched charcoal framing':(.17,.185,.155),'V5 tower-matched muted grey trim':(.29,.305,.25),
'V5 tower-matched warm oak':(.29,.19,.085),'V5 tower-matched door wood':(.23,.155,.077),
'V5 tower-matched chestnut roof':(.24,.135,.055),'V5 tower-matched ochre cloth':(.43,.30,.085),
'V5 tower-matched brass':(.54,.34,.055),'Bone white':(.35,.32,.23),
'Warm hewn limestone':(.28,.29,.23),'Worn stone edges':(.36,.37,.29),'Slate inset':(.17,.19,.15),
'Weathered timber':(.23,.145,.065),'End grain':(.31,.22,.11),'Blackened iron':(.11,.13,.11),
'Blue green roof slate':(.19,.135,.07),'Roof tile variation':(.24,.175,.085),
'Legion oxblood banner':(.40,.28,.07),'BP_Pine':(.115,.22,.075),'BP_PineLight':(.21,.31,.09),'BP_PineDark':(.07,.14,.055),
}.items():recolor(n,c)
grass=bpy.data.materials.get('Textured meadow')
if grass:
    for n in grass.node_tree.nodes:
        if n.type=='VECT_MATH' and n.operation=='SCALE':n.inputs[3].default_value=21
        if n.type=='MIX_RGB':n.inputs[2].default_value=(.21,.265,.08,1)
for o in scene.objects:
    if o.name=='56 x 48 m terrain plinth':o.dimensions=(84,72,o.dimensions.z);o.name='84 x 72 m terrain plinth'
    if o.name=='Meadow surface':o.dimensions=(84,72,o.dimensions.z)
# Hide the small lodge and its palette add-ons; revive the castle as a compact,
# lower silhouette rather than the oversized pale V3 version.
for n in ['V4_Compact_Dark_Base','V5_Tower_Style_Details']:
    if n in bpy.data.collections:bpy.data.collections[n].hide_render=True;bpy.data.collections[n].hide_viewport=True
center=Vector((24.1,9.4,0));factor=.82
for n in ['V3_Base_Architecture','V3_Base_Details']:
    src=bpy.data.collections.get(n)
    if not src:continue
    for old in src.objects:
        o=old.copy();o.data=old.data;groups['V6_Castle' if n.endswith('Architecture') else 'V6_Castle_Details'].objects.link(o)
        o.name='V6 '+old.name;o.location=center+(old.location-center)*factor;o.scale=old.scale*factor;o.hide_render=False;o.hide_viewport=False;o.hide_set(False)
        if o.type=='MESH':
            o.data=o.data.copy()
            for i,m in enumerate(o.data.materials):
                if not m:continue
                if m.name=='Bone white':o.data.materials[i]=bpy.data.materials.get('Warm hewn limestone',m)
                if m.name in ['Legion oxblood banner','Banner']:o.data.materials[i]=bpy.data.materials.get('V5 tower-matched ochre cloth',m)
# Remove regrowth that would visually intersect the revived enclosure.
for o in scene.objects:
    if o.name.startswith('Regrown') and 20.7<o.location.x<27.8 and 4.5<o.location.y<14.0:o.hide_render=True;o.hide_set(True)
def mesh(n,vs,fs,m,g):
    me=bpy.data.meshes.new(n);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(n,me);groups[g].objects.link(o);me.materials.append(m);return o
def copy(src,x,y,s,g):
    o=src.copy();o.data=src.data;groups[g].objects.link(o);o.location=(x,y,0);o.rotation_euler=(0,0,random.uniform(0,math.tau));o.scale=(s,)*3;o.hide_render=False;o.hide_viewport=False;o.hide_set(False);return o
trees=[o for o in bpy.data.collections['Forest'].objects if o.type=='MESH' and o.name.startswith(('Pine_Tall','Pine_Medium','Pine_Wide'))]
shrubs=[o for o in bpy.data.collections['Props'].objects if o.type=='MESH' and o.name.startswith('Shrub')]
rocks=[o for o in bpy.data.collections['Rocks'].objects if o.type=='MESH' and o.name.startswith('Moss_Rock')]
def rx(y):return 8+.45*math.sin(y*.19)
forest_count=0
# A staggered 1.9 m lattice with jitter forms a continuous dense canopy belt.
for ix in range(44):
    for iy in range(38):
        x=-41+ix*1.9+random.uniform(-.45,.45);y=-35+iy*1.9+random.uniform(-.45,.45)
        if -28.6<x<28.6 and -24.6<y<24.6:continue
        if abs(x-rx(y))<1.7:continue
        if x<-27 and abs(y+11)<2.0:continue
        o=copy(random.choice(trees),x,y,random.uniform(.9,1.35),'V6_Forest_Extension');o.name='Dense forest pine';forest_count+=1
for i in range(1000):
    x=random.uniform(-41,41);y=random.uniform(-35,35)
    if -28<x<28 and -24<y<24:continue
    if abs(x-rx(y))<1.3 or (x<-27 and abs(y+11)<1.6):continue
    copy(random.choice(shrubs if i%4 else rocks),x,y,random.uniform(.35,.75),'V6_Forest_Understory')
# Extend the stream through the forest rather than ending at the old boundary.
water=bpy.data.materials['River teal']
for sign in [-1,1]:
    vs=[];fs=[]
    for i in range(49):
        y=sign*(24+i*.25);x=rx(y);vs.extend([(x-.86,y,.06),(x+.86,y,.06)])
        if i:a=(i-1)*2;fs.append((a,a+1,a+3,a+2) if sign>0 else (a+2,a+3,a+1,a))
    mesh('Stream into forest',vs,fs,water,'V6_River_Extension')
    for j in range(24):
        y=sign*(24+j*.5)
        for side in [-1,1]:copy(random.choice(rocks),rx(y)+side*1.05,y,random.uniform(.20,.34),'V6_River_Extension')
mesh('Forest approach road',[(-42,-11.85,.045),(-28,-11.85,.045),(-28,-10.15,.045),(-42,-10.15,.045)],[(0,1,2,3)],bpy.data.materials['Packed ochre path'],'V6_Entrance_Road')
# Use brighter diffuse daylight instead of flattening the whole palette to white.
for o in bpy.data.collections['Presentation'].objects:
    if o.type=='LIGHT' and o.data.type=='SUN':o.data.energy=2.5;o.data.color=(1,.95,.84);o.data.angle=.15
scene.world.node_tree.nodes['Background'].inputs[1].default_value=.85
scene.view_settings.exposure=.25
cam=scene.camera;cam.location=(62,-83,96);cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=114
scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=2400;scene.render.resolution_y=1800;scene.render.resolution_percentage=100;scene.render.filepath=str(OUT/'Map_V6_Forest_Castle.png')
scene['map_dimensions_m']='84 x 72';scene['linear_scale_from_V5']=1.5;scene['tower_pads']=5
bpy.ops.file.pack_all();bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_V6_Forest_Castle.blend'));bpy.ops.render.render(write_still=True)
cam.location=(38,-7,22);cam.rotation_euler=(Vector((24.1,9.4,1.8))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=17;scene.render.resolution_x=1600;scene.render.resolution_y=1300;scene.render.filepath=str(OUT/'Castle_V6_Closeup.png');bpy.ops.render.render(write_still=True)
(OUT/'design_report.json').write_text(json.dumps({'size_m':[84,72],'previous_size_m':[56,48],'linear_scale':1.5,'area_multiplier':2.25,'new_forest_trees':forest_count,'tower_pads':5,'castle_footprint_m':[6.56,8.86]},indent=2))
