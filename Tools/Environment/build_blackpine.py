import bpy, math, random, json, traceback
from pathlib import Path
from mathutils import Vector

ROOT = Path('C:/Users/Emir/BackPackDefance')
OUT = ROOT / 'ArtSource/BlackpineEnvironment'
EXPORT = ROOT / 'Assets/Art/BlackpineEnvironment/Models'
OUT.mkdir(parents=True, exist_ok=True)
EXPORT.mkdir(parents=True, exist_ok=True)
random.seed(731)
scene = bpy.data.scenes.new('Blackpine_Environment_Library')
bpy.context.window.scene = scene
scene.unit_settings.system = 'METRIC'
materials = {}
def mat(name, color, metal=0):
    m = bpy.data.materials.new('BP_' + name)
    m.diffuse_color = (*color, 1)
    m.use_nodes = True
    p = m.node_tree.nodes.get('Principled BSDF')
    p.inputs['Base Color'].default_value = (*color, 1)
    p.inputs['Roughness'].default_value = .88
    p.inputs['Metallic'].default_value = metal
    materials[name] = m
    return m
for n,c in [('Stone',(.22,.235,.19)),('StoneLight',(.34,.35,.28)),('StoneDark',(.12,.145,.13)),('Moss',(.22,.29,.065)),('Pine',(.085,.17,.055)),('PineLight',(.17,.255,.065)),('PineDark',(.045,.095,.038)),('Wood',(.22,.12,.045)),('WoodLight',(.36,.225,.085)),('Soil',(.27,.205,.085)),('Path',(.43,.33,.155)),('Grass',(.19,.245,.055)),('Water',(.035,.19,.235)),('Foam',(.20,.39,.37)),('Banner',(.32,.065,.025)),('Gold',(.55,.36,.10)),('Iron',(.12,.14,.125))]:
    mat(n,c,.5 if n in ['Gold','Iron'] else 0)
parts=[]
assets=[]
def register(o, material):
    o.data.materials.append(materials[material]); parts.append(o); return o
def cube(loc, size, material, bevel=.035):
    bpy.ops.mesh.primitive_cube_add(size=1, location=loc)
    o=bpy.context.object; o.scale=size
    bpy.ops.object.transform_apply(location=False,rotation=False,scale=True)
    if bevel:
        mod=o.modifiers.new('Hand hewn edges','BEVEL'); mod.width=bevel; mod.segments=1
        bpy.ops.object.modifier_apply(modifier=mod.name)
    return register(o,material)
def cone(loc,r1,r2,depth,material,verts=9):
    bpy.ops.mesh.primitive_cone_add(vertices=verts,radius1=r1,radius2=r2,depth=depth,location=loc)
    return register(bpy.context.object,material)
def beam(a,b,r,material='Wood',verts=8):
    a,b=Vector(a),Vector(b)
    o=cone((a+b)/2,r,r,(b-a).length,material,verts)
    o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler(); return o
def rock(loc,size,material='Stone',sub=1):
    bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=sub,radius=1,location=loc)
    o=bpy.context.object
    for v in o.data.vertices:
        v.co *= random.uniform(.83,1.17)
        v.co.z=max(v.co.z,-.65)
    o.scale=size; o.rotation_euler.z=random.random()*6.28
    return register(o,material)
def mesh(name,vs,fs,material):
    me=bpy.data.meshes.new(name); me.from_pydata(vs,[],fs); me.update()
    o=bpy.data.objects.new(name,me); scene.collection.objects.link(o); return register(o,material)
def finish(name,category):
    bpy.ops.object.select_all(action='DESELECT')
    for p in parts:p.select_set(True)
    bpy.context.view_layer.objects.active=parts[0]
    bpy.ops.object.join(); o=bpy.context.object; o.name='BP_'+name
    scene.cursor.location=(0,0,0); bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    bpy.ops.object.transform_apply(location=False,rotation=True,scale=True)
    bpy.ops.object.mode_set(mode='EDIT'); bpy.ops.mesh.select_all(action='SELECT'); bpy.ops.uv.smart_project(island_margin=.025); bpy.ops.object.mode_set(mode='OBJECT')
    o.asset_mark(); o.asset_data.description='Blackpine / '+category+' / 1 unit = 1 metre; ground pivot'
    o['category']=category
    bpy.ops.export_scene.fbx(filepath=str(EXPORT/(o.name+'.fbx')),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,bake_anim=False,add_leaf_bones=False)
    o.data.calc_loop_triangles()
    assets.append({'name':o.name,'category':category,'triangles':len(o.data.loop_triangles),'object':o})
    parts.clear()
    o.location=((len(assets)-1)%6*5.5, (len(assets)-1)//6*6,0)
    return o
def pine(h,w):
    cone((0,0,h*.30),w*.14,w*.08,h*.6,'Wood')
    for j in range(4):
        z=h*(.28+j*.17); r=w*(1-j*.205); depth=h*.40
        vs=[]; count=10
        for k in range(count):
            a=k*math.tau/count
            vs.append((math.cos(a)*r,math.sin(a)*r,z+(k%2)*.11))
        vs.append((.045,0,z+depth))
        fs=[(k,(k+1)%count,count) for k in range(count)]+[tuple(reversed(range(count)))]
        o=mesh('Needle tier',vs,fs,'Pine')
        o.data.materials.append(materials['PineLight']);o.data.materials.append(materials['PineDark'])
        for p in o.data.polygons:p.material_index=random.choices([0,1,2],[6,2,2])[0]
for name,h,w in [('Pine_Tall',4.6,1.05),('Pine_Medium',3.4,.95),('Pine_Young',2.0,.58),('Pine_Wide',3.1,1.25)]:
    pine(h,w);finish(name,'Vegetation')
for i in range(3):
    for j in range(5):rock((random.uniform(-.5,.5),random.uniform(-.4,.4),.3),(.45,.4,random.uniform(.35,.60)),['Pine','Moss','PineLight'][j%3])
    finish('Shrub_'+str(i+1),'Vegetation')
for i in range(2):
    for j in range(14):
        x,y=random.uniform(-.45,.45),random.uniform(-.45,.45);h=random.uniform(.16,.5)
        mesh('Grass blade',[(x-.045,y,0),(x+.045,y,0),(x+.12,y+.04,h)],[(0,1,2)],'Moss' if j%2 else 'Grass')
    finish('Grass_Clump_'+str(i+1),'Vegetation')
for i,sz in enumerate([(.6,.5,.55),(1.0,.8,.8),(1.5,1.15,1.3),(1,1,2.4)]):
    rock((0,0,sz[2]*.65),sz,sub=2)
    for j in range(5):rock((random.uniform(-.5,.5)*sz[0],random.uniform(-.5,.5)*sz[1],sz[2]*1.40),(.25,.26,.09),'Moss')
    finish('Moss_Rock_'+str(i+1),'Rocks')
for i in range(2):
    for j in range(5):rock(((j-2)*.72,random.uniform(-.25,.25),.65),(.65,.65,random.uniform(.9,1.55)),sub=2)
    finish('Cliff_Cluster_'+str(i+1),'Rocks')
cone((0,0,.32),.45,.32,.64,'Wood');cone((0,0,.65),.30,.30,.025,'WoodLight')
for j in range(5):
    a=j*math.tau/5;beam((0,0,.18),(.7*math.cos(a),.7*math.sin(a),.06),.11)
finish('Stump','Props')
beam((-1.25,0,.28),(1.25,0,.28),.28)
beam((-.3,0,.4),(-.1,.48,.68),.10)
for x in [-1.26,1.26]:
    o=cone((x,0,.28),.235,.235,.016,'WoodLight');o.rotation_euler.y=math.pi/2
finish('Fallen_Log','Props')
for style in ['Rail','Palisade']:
    if style=='Rail':
        for x in [-1,1]:cone((x,0,.75),.13,.1,1.5,'Wood')
        for z in [.45,1.05]:beam((-1.12,0,z),(1.12,0,z),.095,'WoodLight')
    else:
        for j in range(7):
            x=(j-3)*.3;cone((x,0,.9),.17,.14,1.8,'Wood');cone((x,0,1.96),.14,0,.32,'WoodLight')
        for z in [.4,1.3]:cube((0,-.16,z),(2.16,.13,.14),'Iron',.015)
    finish('Fence_'+style,'Structures')
for row in range(3):
    for j in range(4):cube(((j-1.5)*.57+(row%2)*.12,0,.22+row*.42),(.54,.60,.40),'StoneLight' if (j+row)%3==0 else 'Stone')
finish('Wall_Straight','Structures')
for z in range(5):cube((0,0,.23+z*.43),(.7,.7,.40),'Stone')
cube((0,0,2.24),(.85,.85,.18),'StoneLight');finish('Wall_Pillar','Structures')
# Real open arch: wedge blocks follow an ellipse; paved deck follows same curve.
N=16
for i in range(N):
    t0=math.pi*i/N;t1=math.pi*(i+1)/N
    vs=[]
    for y in [-1.1,1.1]:
        for a,r,zr in [(t0,2.65,1.7),(t1,2.65,1.7),(t1,2.65,1.98),(t0,2.65,1.98)]:vs.append((r*math.cos(a),y,.12+zr*math.sin(a)))
    mesh('Arch voussoir',vs,[(0,3,2,1),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)],'StoneLight' if i%3==0 else 'Stone')
for i in range(15):
    x=-2.8+i*.4;z=.15+1.97*math.sqrt(max(0,1-(x/2.85)**2))
    for j in range(4):cube((x,(j-1.5)*.52,z),(.39,.50,.15),'StoneLight',.025)
    for y in [-1.16,1.16]:cube((x,y,z+.32),(.39,.25,.64),'Stone',.025)
finish('Bridge_Stone_Arch','Structures')
for x in [-1.45,1.45]:
    cone((x,0,1.6),.19,.14,3.2,'Wood');cone((x,0,3.35),.18,0,.30,'WoodLight')
beam((-1.65,0,2.85),(1.65,0,2.85),.12)
vs=[(-1.15,-.05,2.7),(1.15,-.05,2.7),(1.10,-.05,1.25),(0,-.10,.8),(-1.10,-.05,1.25)]
o=mesh('Swallow banner',vs,[(0,1,2,3,4)],'Banner')
sol=o.modifiers.new('Cloth thickness','SOLIDIFY');sol.thickness=.025;bpy.context.view_layer.objects.active=o;bpy.ops.object.modifier_apply(modifier=sol.name)
cube((0,-.085,1.85),(.08,.035,1.05),'Gold',0);cube((0,-.085,2.05),(.65,.035,.08),'Gold',0)
finish('Banner_Gate','Structures')
for typ in ['Straight','Corner','Junction','Ground','River']:
    cube((0,0,-.12),(4,4,.24),'Soil',0)
    water=typ=='River'
    grid=16
    for ix in range(grid):
        for iy in range(grid):
            x=-2+(ix+.5)*4/grid;y=-2+(iy+.5)*4/grid
            path=abs(y)<.72 if typ=='Straight' else (abs(math.hypot(x+2,y+2)-2)<.72 if typ=='Corner' else (abs(y)<.72 or (abs(x)<.72 and y>0)) if typ=='Junction' else abs(y)<.95 if water else False)
            material=('Water' if water else 'Path') if path else 'Grass'
            cube((x,y,-.012),(4/grid,4/grid,.024),material,0)
    for j in range(28):
        x,y=random.uniform(-1.9,1.9),random.uniform(-1.9,1.9)
        if water and abs(y)<1.05:continue
        rock((x,y,.035),(.055,.065,.035),'Moss' if j%2 else 'StoneLight')
    if water:
        for x in [-1.65,-.8,.3,1.4]:
            for y in [-1.15,1.15]:rock((x,y,.15),(.35,.3,.3),'Stone')
        for j in range(12):cube((random.uniform(-1.8,1.8),random.uniform(-.8,.8),.013),(.22,.016,.004),'Foam',0)
    finish('Tile_'+typ+'_4m','Terrain')
# Presentation scene, kept separate from individually exported models.
parts.clear()
floor=cube((13.75,12,-.36),(35,32,.35),'StoneDark',.12);floor.name='DISPLAY_Ground';parts.clear()
for a in assets:
    o=a['object'];bpy.ops.object.text_add(location=(o.location.x-1.9,o.location.y-2,.012))
    t=bpy.context.object;t.name='Label_'+a['name'];t.data.body=a['name'].replace('BP_','').replace('_',' ');t.data.size=.25;t.data.extrude=.001;t.data.materials.append(materials['StoneLight'])
bpy.ops.object.camera_add(location=(42,-40,45));cam=bpy.context.object
cam.rotation_euler=(Vector((13.5,12,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='ORTHO';cam.data.ortho_scale=44;scene.camera=cam
for pos,power,size in [((8,-8,25),4200,18),((20,23,20),2800,14)]:
    bpy.ops.object.light_add(type='AREA',location=pos);o=bpy.context.object;o.data.energy=power;o.data.shape='DISK';o.data.size=size;o.rotation_euler=(Vector((13,12,0))-o.location).to_track_quat('-Z','Y').to_euler()
bpy.ops.object.light_add(type='SUN',location=(0,0,12));bpy.context.object.rotation_euler=(.45,-.5,-.4);bpy.context.object.data.energy=2
scene.world=bpy.data.worlds.new('Blackpine World');scene.world.use_nodes=True;scene.world.node_tree.nodes['Background'].inputs[0].default_value=(.14,.18,.23,1);scene.world.node_tree.nodes['Background'].inputs[1].default_value=.45
scene.render.engine='CYCLES';scene.cycles.samples=32
scene.render.resolution_x=1800;scene.render.resolution_y=1500;scene.render.resolution_percentage=100
scene.render.image_settings.file_format='PNG';scene.render.filepath=str(OUT/'Blackpine_Overview.png')
for screen in bpy.data.screens:
    for area in screen.areas:
        if area.type=='VIEW_3D':
            area.spaces.active.region_3d.view_perspective='CAMERA';area.spaces.active.shading.color_type='MATERIAL'
manifest=[{k:v for k,v in a.items() if k!='object'} for a in assets]
(OUT/'manifest.json').write_text(json.dumps(manifest,indent=2))
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_Environment.blend'))
(OUT/'build_complete.txt').write_text(str(len(assets))+' assets exported')
bpy.ops.render.render(write_still=True)
(OUT/'render_complete.txt').write_text('ok')
