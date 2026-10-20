import bpy, math, random, json
from pathlib import Path
from mathutils import Vector
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'ArtSource/BlackpineMapV2';OUT.mkdir(parents=True,exist_ok=True)
random.seed(1827)
# Source assets are read only. All outputs stay under ArtSource.
with bpy.data.libraries.load(str(ROOT/'ArtSource/BlackpineEnvironment/Blackpine_Environment.blend'),link=False) as (src,dst):
    dst.objects=[n for n in src.objects if n.startswith('BP_')]
library={o.name:o for o in dst.objects if o}
scene=bpy.data.scenes.new('Blackpine_Strategic_Map_V2');bpy.context.window.scene=scene
scene.unit_settings.system='METRIC'
groups={}
for name in ['Terrain','Road','River','Bridge','Tower_Pads_Exactly_5','Forest','Rocks','Props','Tactical_Overlay','Presentation']:
    c=bpy.data.collections.new(name);scene.collection.children.link(c);groups[name]=c
def move(o,group):
    for c in list(o.users_collection):c.objects.unlink(o)
    groups[group].objects.link(o);return o
def mat(name,color,metal=0):
    m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True
    p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=.82;p.inputs['Metallic'].default_value=metal;return m
stone=mat('Warm hewn limestone',(.30,.32,.26));light=mat('Worn stone edges',(.43,.44,.34));dark=mat('Slate inset',(.12,.17,.16));soil=mat('Earth cut',(.14,.10,.055));roadmat=mat('Packed ochre path',(.39,.29,.125));wood=mat('Weathered timber',(.20,.105,.043));woodlight=mat('End grain',(.38,.24,.10));iron=mat('Blackened iron',(.07,.095,.09),.5);gold=mat('Muted brass',(.62,.42,.13),.4);cloth=mat('Faded canvas',(.36,.32,.19));water=mat('River teal',(.025,.155,.18));moss=mat('Moss',(.19,.26,.065));amber=mat('Lantern glass',(.95,.48,.08));white=mat('Planning lines',(.75,.65,.30));ink=mat('Map ink',(.035,.06,.05))
def cube(name,loc,size,m,group='Props',bevel=.03):
    bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.name=name;o.scale=size;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);move(o,group)
    if bevel:
        b=o.modifiers.new('Soft worn edges','BEVEL');b.width=bevel;b.segments=2
    return o
def cyl(name,loc,r,depth,m,group='Props',verts=24,r2=None):
    bpy.ops.mesh.primitive_cone_add(vertices=verts,radius1=r,radius2=r if r2 is None else r2,depth=depth,location=loc);o=bpy.context.object;o.name=name;o.data.materials.append(m);move(o,group);return o
def beam(name,a,b,r,m=wood,group='Props'):
    a,b=Vector(a),Vector(b);o=cyl(name,(a+b)/2,r,(b-a).length,m,group);o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();return o
def mesh(name,vs,fs,m,group):
    me=bpy.data.meshes.new(name);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(name,me);groups[group].objects.link(o);me.materials.append(m);return o
def asset(name,x,y,scale=1,yaw=None,group='Props'):
    src=library['BP_'+name];o=src.copy();o.data=src.data;groups[group].objects.link(o);o.name=name;o.location=(x,y,0);o.rotation_euler=(0,0,random.uniform(0,math.tau) if yaw is None else yaw);o.scale=(scale,)*3;return o
def label(body,x,y,size=.8,group='Tactical_Overlay',material=gold,z=.18):
    cu=bpy.data.curves.new(body,'FONT');cu.body=body;cu.size=size;cu.align_x='CENTER';o=bpy.data.objects.new(body,cu);groups[group].objects.link(o);o.location=(x,y,z);o.data.materials.append(material);return o
def curve(name,points,r,m,group):
    cu=bpy.data.curves.new(name,'CURVE');cu.dimensions='3D';cu.bevel_depth=r;cu.bevel_resolution=1;s=cu.splines.new('POLY');s.points.add(len(points)-1)
    for p,co in zip(s.points,points):p.co=(*co,1)
    o=bpy.data.objects.new(name,cu);groups[group].objects.link(o);cu.materials.append(m);return o
controls=[(-28,-11),(-20,-11),(-12,-5),(-18,2),(-11,8),(-3,7),(3,2),(12,2),(18,9),(28,9)]
route=[]
for i in range(len(controls)-1):
    p0=Vector(controls[max(0,i-1)]);p1=Vector(controls[i]);p2=Vector(controls[i+1]);p3=Vector(controls[min(len(controls)-1,i+2)])
    for j in range(20):
        t=j/20;p=.5*((2*p1)+(-p0+p2)*t+(2*p0-5*p1+4*p2-p3)*t*t+(-p0+3*p1-3*p2+p3)*t*t*t);route.append(p)
route.append(Vector(controls[-1]))
def dist(x,y):return min((p-Vector((x,y))).length for p in route)
pads=[(-17,-3.3),(-11,3.3),(-3,10.5),(2,-1.5),(17,4)]
assert all(dist(x,y)>2.15 for x,y in pads),[(p,dist(*p)) for p in pads]
base=cube('56 x 48 m terrain plinth',(0,0,-.32),(56,48,.6),soil,'Terrain',.2)
grass=mat('Textured meadow',(.8,.85,.7));p=grass.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(.8,.85,.7,1)
img=bpy.data.images.load(str(ROOT/'Assets/Art/BlackpineEnvironment/Textures/Grass_Albedo.png'));img.pack();nodes=grass.node_tree.nodes;tex=nodes.new('ShaderNodeTexImage');tex.image=img;uv=nodes.new('ShaderNodeTexCoord');mapping=nodes.new('ShaderNodeVectorMath');mapping.operation='SCALE';mapping.inputs[3].default_value=14;grass.node_tree.links.new(uv.outputs['Generated'],mapping.inputs[0]);grass.node_tree.links.new(mapping.outputs[0],tex.inputs[0]);grass.node_tree.links.new(tex.outputs['Color'],p.inputs['Base Color']);bump=nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.13;bump.inputs['Distance'].default_value=.035;grass.node_tree.links.new(tex.outputs['Color'],bump.inputs['Height']);grass.node_tree.links.new(bump.outputs[0],p.inputs['Normal'])
cube('Meadow surface',(0,0,-.025),(56,48,.05),grass,'Terrain',0)
# Continuous gently turning road. Width changes create setup zones and a crossing choke.
vs=[];fs=[]
for i,p in enumerate(route):
    d=(route[min(i+1,len(route)-1)]-route[max(i-1,0)]).normalized();n=Vector((-d.y,d.x));half=.85 if p.x<4 or p.x>11 else .65
    for sign in [-1,1]:q=p+n*half*sign;vs.append((q.x,q.y,.045))
    if i: a=(i-1)*2;fs.append((a,a+1,a+3,a+2))
mesh('Single readable strategic route',vs,fs,roadmat,'Road')
# A recessed-looking channel and two continuously meandering stone banks.
def river_x(y):return 8+.45*math.sin(y*.19)
vs=[];fs=[]
for i in range(97):
    y=-24+i*.5;x=river_x(y);vs.extend([(x-.86,y,.06),(x+.86,y,.06)])
    if i:a=(i-1)*2;fs.append((a,a+1,a+3,a+2))
mesh('Narrow stream',vs,fs,water,'River')
for i in range(49):
    y=-24+i
    if abs(y-2)<1.3:continue
    for side in [-1,1]:asset('Moss_Rock_'+str(random.randint(1,3)),river_x(y)+side*1.03,y,random.uniform(.22,.42),group='River')
# Compact bridge: 3.8 m span, 1.7 m deck width, 0.25 m rise; no stair-step deck.
cx=river_x(2)
for i in range(19):
    x=cx-1.9+i*.20;z=.11+.25*math.sin(i/18*math.pi)
    for j in range(4):cube('Flush deck paver',(x,2+(j-1.5)*.4,z),(.195,.39,.16),light,'Bridge',.012)
    if i%2==0:
        for y in [1.12,2.88]:cube('Low bridge parapet',(x,y,z+.20),(.38,.18,.42),stone,'Bridge',.035)
for side in [-1,1]:
    cube('Bank abutment',(cx+side*1.5,2,.12),(.7,1.9,.33),stone,'Bridge',.06)
for x in [cx-1.8,cx+1.8]:
    for y in [1.1,2.9]:cyl('Bridge end post',(x,y,.52),.14,.75,stone,'Bridge',8)
# Five individually named placement clearings with modular radial paving and inset numbers.
for i,(x,y) in enumerate(pads,1):
    root=bpy.data.objects.new('Tower_Pad_%02d'%i,None);groups['Tower_Pads_Exactly_5'].objects.link(root);root['role']=['Opening hairpin / repeated exposure','Inner bend / crossfire','High clearing / long coverage','Crossing approach / choke','Exit bend / last defence'][i-1];root['build_radius_m']=1.22
    o=cyl('Pad %02d slate footprint'%i,(x,y,.055),1.22,.09,dark,'Tower_Pads_Exactly_5',48);o.parent=root
    for j in range(18):
        a=j*math.tau/18;o=cube('Pad rim %02d'%i,(x+1.35*math.cos(a),y+1.35*math.sin(a),.10),(.39,.26,.17),light,'Tower_Pads_Exactly_5',.035);o.rotation_euler.z=a+math.pi/2;o.parent=root
    label(str(i).zfill(2),x,y-.34,.85,'Tower_Pads_Exactly_5',gold,.115)
    for a in [0,math.pi/2,math.pi,3*math.pi/2]:cyl('Brass anchor',(x+math.cos(a),y+math.sin(a),.12),.07,.025,gold,'Tower_Pads_Exactly_5',12)
    for j in range(48):
        if j%2:continue
        a=j*math.tau/48;b=(j+.7)*math.tau/48
        curve('Pad %02d conceptual range'%i,[(x+6.2*math.cos(a),y+6.2*math.sin(a),.22),(x+6.2*math.cos(b),y+6.2*math.sin(b),.22)],.035,white,'Tactical_Overlay')
def blocked(x,y,margin=0):
    return dist(x,y)<1.65+margin or abs(x-river_x(y))<1.55+margin or any(math.hypot(x-a,y-b)<2.0+margin for a,b in pads)
# Preserve the denser foliage direction of the enhanced Unity prototype.
for variant,h,w in [('Tall',5.45,1.05),('Medium',4.05,.95),('Young',2.38,.58),('Wide',3.7,1.25)]:
    src=library['BP_Pine_'+variant];vs=[];fs=[];slots=[];n=20
    for tier in range(-1,10):
        start=len(vs);z=0 if tier<0 else h*(.18+tier*.072);height=h*.70 if tier<0 else h*.28;radius=w*.115 if tier<0 else w*(1-tier*.082)
        for ring in range(4):
            for j in range(n):
                a=j*math.tau/n+tier*.41;wave=1 if tier<0 else 1+.17*math.sin(j*2.1+tier)+.08*math.cos(j*4.3)
                r=radius*[.72,1,.55,.025][ring]*wave;zz=z+height*[.03,.17,.55,1][ring]
                if tier>=0 and ring<2:zz+=h*.025*math.sin(j*2.7+tier)
                vs.append((math.cos(a)*r,math.sin(a)*r,zz))
        for ring in range(3):
            for j in range(n):
                a=start+ring*n+j;b=start+ring*n+(j+1)%n
                fs.append((a,b,b+n,a+n));slots.append(0 if tier<0 else (2 if (j+tier)%7==0 else 3 if (j+tier)%5==0 else 1))
    me=bpy.data.meshes.new('Detailed pine '+variant);me.from_pydata(vs,[],fs);me.update()
    for name in ['BP_Wood','BP_Pine','BP_PineLight','BP_PineDark']:me.materials.append(bpy.data.materials[name])
    for p,slot in zip(me.polygons,slots):p.material_index=slot;p.use_smooth=True
    src.data=me
# Dense woodland border, lighter scattered understory inside tactical clearings.
for i in range(490):
    x=random.uniform(-27.4,27.4);y=random.uniform(-23.4,23.4);edge=abs(x)>23 or abs(y)>19
    if blocked(x,y,.4) or (not edge and random.random()>.18):continue
    asset(random.choice(['Pine_Tall','Pine_Medium','Pine_Wide','Pine_Young']),x,y,random.uniform(.65,1.10) if edge else random.uniform(.45,.75),group='Forest')
for i in range(220):
    x=random.uniform(-27,27);y=random.uniform(-23,23)
    if blocked(x,y):continue
    asset(random.choice(['Shrub_1','Shrub_2','Shrub_3','Grass_Clump_1','Moss_Rock_1','Moss_Rock_2','Moss_Rock_3']),x,y,random.uniform(.35,.85),group='Rocks' if i%3==0 else 'Props')
for i in range(11):asset('Cliff_Cluster_'+str(i%2+1),-24+i*4.8,22.0,random.uniform(.7,1.1),group='Rocks')
for x,y in [(-25,-11),(25,9)]:asset('Banner_Gate',x,y,.85,math.pi/2)
for x,y in [(-22,-5),(-6,-13),(19,15),(-7,16)]:
    asset('Stump',x,y,.9);asset('Fallen_Log',x+1.0,y-1,.9)
for i in range(4):asset('Fence_Rail',-23+i*2.1,-13,.9,0)
for i in range(3):asset('Wall_Straight',14+i*2.1,14,.8,0)
asset('Wall_Pillar',13,14,.8);asset('Wall_Pillar',20,14,.8)
# New environmental storytelling props: supply crates, iron-banded barrels, a camp,
# broken column, abandoned handcart, short dock, and warm lanterns.
def crate(x,y,s=.7):
    cube('Supply crate',(x,y,s/2),(s,s,s),wood)
    for z in [.12,s-.12]:cube('Crate brace',(x,y-s*.51,z),(s+.03,.07,.09),woodlight)
    for xx in [-s*.37,s*.37]:cube('Crate corner strap',(x+xx,y-s*.52,s/2),(.07,.04,s),iron)
def barrel(x,y):
    cyl('Barrel staves',(x,y,.43),.34,.86,wood,verts=16,r2=.30)
    for z in [.13,.68]:cyl('Iron barrel hoop',(x,y,z),.349,.055,iron,verts=24)
    cyl('Barrel lid',(x,y,.87),.29,.035,woodlight,verts=16)
for x,y in [(-21,-7),(-20.1,-6.9),(-8,15),(20,14.8),(21,15.1)]:crate(x,y)
for x,y in [(-21.7,-6.7),(-8.8,15),(19.5,15.3)]:barrel(x,y)
mesh('Canvas shelter',[(-9,15,0),(-6,15,0),(-7.5,15,1.9),(-9,18,0),(-6,18,0),(-7.5,18,1.9)],[(0,3,5,2),(1,2,5,4)],cloth,'Props')
for y in [14.8,18.2]:beam('Tent ridge support',(-7.5,y,0),(-7.5,y,2.0),.055)
for i in range(8):
    a=i*math.tau/8;cyl('Camp fire ring',(-4.5+math.cos(a)*.5,16+math.sin(a)*.5,.12),.17,.2,stone,verts=8)
beam('Cold firewood',(-4.9,15.8,.16),(-4.1,16.2,.16),.09);beam('Cold firewood',(-4.8,16.3,.2),(-4.2,15.7,.2),.08)
cube('Cart cargo bed',(-23,9,.62),(1.7,1.0,.2),wood)
for x in [-23.6,-22.4]:
    for y in [8.35,9.65]:
        o=cyl('Cart wheel',(x,y,.43),.4,.12,woodlight,verts=20);o.rotation_euler.x=math.pi/2
for y in [8.48,9.52]:cube('Cart side',(-23,y,.96),(1.7,.09,.55),wood)
beam('Cart shaft',(-23.8,8.7,.6),(-25.0,8.7,.35),.055);beam('Cart shaft',(-23.8,9.3,.6),(-25,9.3,.35),.055)
for i in range(7):cube('Small fishing dock',(10.2,-14+i*.29,.18),(2.3,.27,.16),wood,'Props',.025)
for x in [9.2,11.2]:
    for y in [-14,-12.2]:cyl('Dock post',(x,y,.24),.09,.8,wood,verts=12)
for x,y in [(-24,-9),(-13,10),(4.8,4.3),(12,4),(22,11)]:
    beam('Lantern post',(x,y,0),(x,y,1.9),.07);beam('Lantern bracket',(x,y,1.85),(x+.35,y,1.85),.055)
    cube('Lantern cage',(x+.35,y,1.53),(.26,.26,.38),iron,bevel=.025);cube('Amber panes',(x+.35,y-.139,1.53),(.17,.018,.25),amber,bevel=.008)
for i in range(12):
    x=random.uniform(13,22);y=random.uniform(13,17);cube('Ruined masonry',(x,y,.15),(random.uniform(.3,.7),.45,.3),stone)
# Render presentation and a separate tactical overhead drawing.
camdata=bpy.data.cameras.new('Map camera');cam=bpy.data.objects.new('Beauty Camera',camdata);groups['Presentation'].objects.link(cam);cam.location=(43,-57,66);cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();camdata.type='ORTHO';camdata.ortho_scale=76;scene.camera=cam
world=bpy.data.worlds.new('Forest daylight');world.use_nodes=True;world.node_tree.nodes['Background'].inputs[0].default_value=(.24,.30,.37,1);world.node_tree.nodes['Background'].inputs[1].default_value=.65;scene.world=world
bpy.ops.object.light_add(type='SUN',location=(0,0,30));sun=bpy.context.object;sun.rotation_euler=(.5,-.4,-.45);sun.data.energy=2.0;sun.data.angle=.13;move(sun,'Presentation')
bpy.ops.object.light_add(type='AREA',location=(-15,-10,30));bpy.context.object.data.energy=2200;bpy.context.object.data.shape='DISK';bpy.context.object.data.size=35;move(bpy.context.object,'Presentation')
scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=1920;scene.render.resolution_y=1400;scene.render.resolution_percentage=100;scene.render.image_settings.file_format='PNG';scene.view_settings.view_transform='AgX'
groups['Tactical_Overlay'].hide_render=True;groups['Tactical_Overlay'].hide_viewport=True
for screen in bpy.data.screens:
    for area in screen.areas:
        if area.type=='VIEW_3D':area.spaces.active.region_3d.view_perspective='CAMERA';area.spaces.active.shading.color_type='MATERIAL'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_Strategic_Map_V2.blend'))
scene.render.filepath=str(OUT/'Map_V2_Beauty.png');bpy.ops.render.render(write_still=True)
groups['Tactical_Overlay'].hide_render=False;cam.location=(0,0,80);cam.rotation_euler=(0,0,0);camdata.ortho_scale=64;scene.render.resolution_x=1800;scene.render.resolution_y=1600;scene.render.filepath=str(OUT/'Map_V2_Tactical.png');bpy.ops.render.render(write_still=True)
(OUT/'design.json').write_text(json.dumps({'size_m':[56,48],'tower_pads':[{'id':i+1,'position':list(p),'road_clearance_m':round(dist(*p),2)} for i,p in enumerate(pads)],'bridge':{'length_m':3.8,'deck_width_m':1.7,'rise_m':.25},'conceptual_range_m':6.2,'route_controls':controls,'unity_exported':False},indent=2))
