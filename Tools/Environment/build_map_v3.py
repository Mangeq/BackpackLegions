import bpy, math, random, json
from mathutils import Vector
from pathlib import Path
ROOT=Path('C:/Users/Emir/BackPackDefance');OUT=ROOT/'ArtSource/BlackpineMapV3';OUT.mkdir(parents=True,exist_ok=True)
scene=bpy.data.scenes['Blackpine_Strategic_Map_V2'];bpy.context.window.scene=scene;scene.name='Blackpine_Strategic_Map_V3_Base'
random.seed(2891)
def mat(name,color,metal=0):
    m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=.82;p.inputs['Metallic'].default_value=metal;return m
def existing(name):return bpy.data.materials.get(name) or mat(name,(.19,.26,.065))
stone=existing('Warm hewn limestone');light=existing('Worn stone edges');dark=existing('Slate inset');wood=existing('Weathered timber');woodlight=existing('End grain');iron=existing('Blackened iron');gold=existing('Muted brass');moss=existing('Moss');soil=existing('Earth cut')
slate=mat('Blue green roof slate',(.08,.145,.14));slate2=mat('Roof tile variation',(.13,.21,.18));red=mat('Legion oxblood banner',(.27,.045,.025));leaf=mat('Fern olive green',(.16,.25,.065));leaf2=mat('Understory sage',(.25,.31,.09));mulch=mat('Forest floor litter',(.18,.14,.065));cream=mat('Bone white',(.68,.61,.42));ember=mat('Warm flame',(.95,.27,.025))
p=ember.node_tree.nodes.get('Principled BSDF');p.inputs['Emission Color'].default_value=(1,.17,.01,1);p.inputs['Emission Strength'].default_value=2
groups={}
for name in ['V3_Understory','V3_Environmental_Stories','V3_Base_Architecture','V3_Base_Details']:
    c=bpy.data.collections.new(name);scene.collection.children.link(c);groups[name]=c
def move(o,g):
    for c in list(o.users_collection):c.objects.unlink(o)
    groups[g].objects.link(o);return o
def cube(name,loc,size,m,g='V3_Base_Details',bevel=.03):
    bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.name=name;o.scale=size;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);move(o,g)
    if bevel:b=o.modifiers.new('Worn corners','BEVEL');b.width=bevel;b.segments=2
    return o
def cyl(name,loc,r,h,m,g='V3_Base_Details',r2=None,n=16):
    bpy.ops.mesh.primitive_cone_add(vertices=n,radius1=r,radius2=r if r2 is None else r2,depth=h,location=loc);o=bpy.context.object;o.name=name;o.data.materials.append(m);move(o,g);return o
def beam(name,a,b,r,m=wood,g='V3_Base_Details'):
    a,b=Vector(a),Vector(b);o=cyl(name,(a+b)/2,r,(b-a).length,m,g);o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();return o
def mesh(name,vs,fs,m,g='V3_Base_Details'):
    me=bpy.data.meshes.new(name);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(name,me);groups[g].objects.link(o);me.materials.append(m);return o
templates={}
for o in scene.objects:
    for name in ['Shrub_1','Shrub_2','Moss_Rock_1','Moss_Rock_2','Moss_Rock_3','Pine_Young','Pine_Medium','Fallen_Log','Stump']:
        if o.name==name and o.type=='MESH':templates[name]=o
def scatter(name,x,y,s=1,g='V3_Understory'):
    o=templates[name].copy();o.data=templates[name].data;groups[g].objects.link(o);o.name='V3 '+name;o.location=(x,y,0);o.rotation_euler=(0,0,random.uniform(0,math.tau));o.scale=(s,)*3;return o
routeinfo=json.loads((ROOT/'ArtSource/BlackpineMapV2/design.json').read_text());controls=[Vector(p) for p in routeinfo['route_controls']];route=[]
for i in range(len(controls)-1):
    p0=controls[max(0,i-1)];p1=controls[i];p2=controls[i+1];p3=controls[min(len(controls)-1,i+2)]
    for j in range(20):t=j/20;route.append(.5*(2*p1+(-p0+p2)*t+(2*p0-5*p1+4*p2-p3)*t*t+(-p0+3*p1-3*p2+p3)*t*t*t))
pads=[Vector(p['position']) for p in routeinfo['tower_pads']]
def blocked(x,y,margin=0):
    p=Vector((x,y));return min((p-a).length for a in route)<1.4+margin or min((p-a).length for a in pads)<1.85+margin or abs(x-(8+.45*math.sin(y*.19)))<1.4+margin or (x>19 and 3<y<16)
# Hide the old exit decorations in the new fortress footprint, retaining them in source.
for o in list(scene.objects):
    if o.type!='MESH' or o.name.startswith(('Single readable','Meadow surface','56 x')):continue
    x,y=o.location.x,o.location.y
    if 19.2<x<28 and 3.5<y<16 and not any(c.name.startswith('Tower_Pads') for c in o.users_collection):o.hide_render=True;o.hide_set(True)
# Clusters, not a uniform random distribution: woodland floor, trailside tufts and
# fern patches create layers while preserving the road and five placement areas.
clusters=[(-22,-18),(-19,13),(-8,-15),(-5,17),(1,16),(14,-13),(17,18),(23,-7),(-23,5),(-3,-4)]
for cx,cy in clusters:
    for i in range(30):
        x=random.gauss(cx,2.0);y=random.gauss(cy,1.6)
        if abs(x)>27.2 or abs(y)>23 or blocked(x,y):continue
        scatter(random.choice(['Shrub_1','Shrub_2','Moss_Rock_1','Pine_Young']),x,y,random.uniform(.22,.65))
# Fine grass blades are batched into one mesh for a lighter Blender scene.
vs=[];fs=[]
for i in range(1900):
    x=random.uniform(-27.4,27.4);y=random.uniform(-23.3,23.3)
    if blocked(x,y):continue
    for j in range(3):
        a=random.uniform(0,math.tau);w=random.uniform(.035,.075);h=random.uniform(.12,.31);k=len(vs)
        vs.extend([(x-math.cos(a)*w,y-math.sin(a)*w,.02),(x+math.cos(a)*w,y+math.sin(a)*w,.02),(x+math.cos(a)*.12,y+math.sin(a)*.12,h)]);fs.append((k,k+1,k+2))
mesh('Thousands of meadow blades',vs,fs,leaf,'V3_Understory')
for cx,cy in clusters:
    if blocked(cx,cy):continue
    for k in range(4):
        x=cx+random.uniform(-1.4,1.4);y=cy+random.uniform(-1.4,1.4)
        if blocked(x,y):continue
        vs=[];fs=[]
        for j in range(7):
            a=j*math.tau/7;length=random.uniform(.35,.65)
            for q in range(1,5):
                t=q/5;mid=Vector((x+math.cos(a)*length*t,y+math.sin(a)*length*t,.35*math.sin(t*math.pi*.8)));perp=Vector((-math.sin(a),math.cos(a),0))*.13*(1-t);n=len(vs);vs.extend([tuple(mid-perp),tuple(mid+perp),tuple(mid+Vector((math.cos(a)*.16,math.sin(a)*.16,.04)))]);fs.append((n,n+1,n+2))
        mesh('Forest fern',vs,fs,leaf2,'V3_Understory')
# Road shoulders: scattered stones and leaf litter do not obscure the road.
for i in range(0,len(route),3):
    p=route[i];d=(route[min(i+1,len(route)-1)]-route[max(0,i-1)]).normalized();normal=Vector((-d.y,d.x))
    for side in [-1,1]:
        q=p+normal*random.uniform(1.25,1.75)*side
        if blocked(q.x,q.y,.05):continue
        scatter('Moss_Rock_1',q.x,q.y,random.uniform(.08,.18))
for i in range(110):
    cx,cy=random.choice(clusters);x=random.gauss(cx,1.5);y=random.gauss(cy,1.3)
    if blocked(x,y):continue
    cyl('Leaf litter mound',(x,y,.025),random.uniform(.12,.35),.025,mulch,'V3_Understory',n=7)
def crate(x,y,z=0,s=.6,g='V3_Base_Details'):
    cube('Provision crate',(x,y,z+s/2),(s,s,s),wood,g)
    for zz in [.1,s-.1]:cube('Crate crossbar',(x,y-s*.52,z+zz),(s+.02,.06,.08),woodlight,g)
    for xx in [-.38,.38]:cube('Crate strap',(x+s*xx,y-s*.54,z+s/2),(.045,.025,s),iron,g)
def barrel(x,y,g='V3_Base_Details'):
    cyl('Oak barrel',(x,y,.40),.29,.8,wood,g,r2=.26)
    for z in [.14,.63]:cyl('Barrel hoop',(x,y,z),.30,.055,iron,g,n=24)
    cyl('Barrel cap',(x,y,.81),.25,.025,woodlight,g)
def banner(x,y,z,g='V3_Base_Details'):
    mesh('Legion swallowtail',[(x,y,z),(x+1.05,y,z),(x+1.05,y,z-1.3),(x+.52,y,z-1.05),(x,y,z-1.3)],[(0,1,2,3,4)],red,g)
    beam('Banner rail',(x-.08,y,z+.02),(x+1.13,y,z+.02),.035,iron,g)
    cube('Legion golden vertical',(x+.52,y-.016,z-.50),(.07,.025,.65),gold,g,.005);cube('Legion golden cross',(x+.52,y-.018,z-.40),(.38,.025,.06),gold,g,.005)
# Small stories in empty corners: lumber camp, cart debris, shrine and graveyard.
for x,y in [(-21,15),(-18,15),(-20,17)]:
    scatter('Fallen_Log',x,y,.75,'V3_Environmental_Stories')
    for j in range(2):crate(x+j*.7,y+1,g='V3_Environmental_Stories')
for i in range(5):beam('Stacked timber',(-18,17+i*.3,.22),(-14.5,17+i*.3,.22),.15,g='V3_Environmental_Stories')
for x,y in [(-6,-16),(-5.1,-16.3),(-4.1,-16)]:
    cube('Old waystone',(x,y,.55),(.45,.25,1.1),stone,'V3_Environmental_Stories',.10);cube('Waystone cross',(x,y-.15,.70),(.29,.04,.07),gold,'V3_Environmental_Stories',.005)
for j in range(6):scatter('Moss_Rock_1',-6+random.uniform(-1.5,3),-17+random.uniform(-.6,.6),.25,'V3_Environmental_Stories')
cyl('Wayside shrine foot',(15,-16,.20),.85,.40,stone,'V3_Environmental_Stories',n=8);cyl('Shrine pillar',(15,-16,1.05),.35,1.3,stone,'V3_Environmental_Stories',n=8);cyl('Shrine crown',(15,-16,1.78),.6,.18,light,'V3_Environmental_Stories',n=8)
banner(14.5,-16.4,1.8,'V3_Environmental_Stories')
for j in range(4):cyl('Small shrine candles',(14.5+j*.32,-16.7,.5),.035,.22,cream,'V3_Environmental_Stories',n=8)
# Defensive base, contained within the eastern map boundary.
G='V3_Base_Architecture';D='V3_Base_Details'
cube('Base stone foundation',(24.1,9.4,.10),(8.0,10.8,.20),dark,G,.15)
for ix in range(12):
    for iy in range(16):
        x=20.35+ix*.65;y=4.4+iy*.65
        cube('Courtyard paving',(x,y,.22),(.63,.63,.08),light if random.random()<.15 else stone,G,.018)
def wall(a,b,height=1.25):
    a,b=Vector(a),Vector(b);d=b-a;length=d.length;steps=max(1,int(length/.55));dir=d.normalized()
    for row in range(3):
        for k in range(steps):
            p=a+dir*((k+.5)*length/steps)
            o=cube('Fortification masonry',(p.x,p.y,.45+row*.36),(length/steps-.025,.5,.34),stone,G,.035);o.rotation_euler.z=math.atan2(d.y,d.x)
    for k in range(0,steps,2):
        p=a+dir*((k+.5)*length/steps);o=cube('Wall merlon',(p.x,p.y,1.63),(.40,.56,.40),light,G,.035);o.rotation_euler.z=math.atan2(d.y,d.x)
wall((20.2,4.2),(27.9,4.2));wall((20.2,14.7),(27.9,14.7));wall((27.9,4.2),(27.9,14.7));wall((20.2,4.2),(20.2,7.4));wall((20.2,10.6),(20.2,14.7))
def tower(x,y,h=3.1):
    cyl('Watchtower plinth',(x,y,.42),.91,.50,dark,G,n=8)
    cyl('Stone watchtower',(x,y,1.6),.72,2.3,stone,G,n=8)
    cyl('Tower parapet',(x,y,2.86),.86,.30,light,G,n=8)
    for i in range(8):
        a=i*math.tau/8;cube('Tower crenellation',(x+.69*math.cos(a),y+.69*math.sin(a),3.16),(.30,.30,.38),light,G,.03)
    for z in [1.25,2.0]:cube('Tower arrow slit',(x,y-.723,z),(.12,.025,.42),iron,D,.01)
tower(20.2,4.2);tower(27.8,14.6)
for y in [7.3,10.7]:
    cube('Gateway stone pier',(20.2,y,1.25),(.8,.8,2.2),stone,G,.08);cube('Gate cap',(20.2,y,2.42),(1.0,1.0,.2),light,G)
beam('Gateway timber lintel',(20.2,7.0,2.5),(20.2,11,2.5),.18,wood,G)
for y in [7.5,10.5]:
    o=cube('Open gate leaf',(21.0,y,1.05),(1.4,.15,1.8),wood,G);o.rotation_euler.z=.25 if y<9 else -.25
# Stone keep with timber framing and individually stepped slate roof courses.
cube('Keep lower footing',(25.7,10.4,.55),(3.5,4.6,.65),dark,G,.12)
cube('Keep stone walls',(25.7,10.4,2.05),(3.2,4.3,2.55),stone,G,.08)
cube('Keep plaster upper storey',(25.7,10.4,3.60),(3.15,4.25,.65),cream,G,.04)
for x in [24.1,27.3]:
    for y in [8.25,10.4,12.55]:cube('Keep timber upright',(x,y,2.52),(.16,.16,2.7),wood,G)
for z in [1.2,3.2,3.95]:
    for x in [24.1,27.3]:cube('Keep belt timber',(x,10.4,z),(.18,4.4,.16),wood,G)
cube('Main keep door',(24.06,9.5,1.24),(.05,1.0,1.8),wood,D,.045)
for j in range(5):cube('Door iron nail',(24.02,9.12+j*.19,1.24),(.025,.045,.045),gold,D,.005)
for y in [8.8,11.8]:
    cube('Keep inset window',(24.04,y,2.6),(.04,.65,.8),iron,D,.04)
    cube('Window mullion',(24.01,y,2.6),(.04,.06,.78),gold,D,.005)
    cube('Window transom',(24.01,y,2.6),(.04,.62,.06),gold,D,.005)
for side in [-1,1]:
    for course in range(9):
        x=25.7+side*(.12+course*.21);z=5.25-course*.145
        for j in range(9):
            o=cube('Individual slate roof tile',(x,8.0+j*.60,z),(.35,.58,.075),slate if random.random()<.75 else slate2,G,.018);o.rotation_euler.y=side*.60
beam('Keep roof ridge',(25.7,7.7,5.34),(25.7,13.1,5.34),.12,iron,G)
cube('Chimney',(26.5,11.9,4.85),(.52,.55,1.35),stone,G,.04);cube('Chimney crown',(26.5,11.9,5.55),(.67,.68,.18),light,G)
banner(25.0,8.19,3.65);banner(26.2,8.19,3.65)
# The gable faces are timber triangles, not empty roof openings.
for y in [7.98,12.98]:mesh('Timbered gable',[(23.72,y,3.9),(27.68,y,3.9),(25.7,y,5.25)],[(0,1,2)],wood,G)
# Supply yard, small well, weapon rack and elevated signal flag.
for x,y in [(22,12.8),(22.7,12.8),(22,13.5)]:crate(x,y)
crate(22,12.8,.62,.56)
for x,y in [(26.6,5.5),(27.25,5.7),(26.9,6.4)]:barrel(x,y)
for i in range(12):
    a=i*math.tau/12;cyl('Well curb stone',(22.6+.48*math.cos(a),5.9+.48*math.sin(a),.62),.15,.60,stone,D,n=8)
cyl('Dark water in well',(22.6,5.9,.60),.38,.025,dark,D,n=32)
for x in [22.0,23.2]:beam('Well upright',(x,5.9,.3),(x,5.9,1.8),.06)
beam('Well crossbeam',(21.9,5.9,1.8),(23.3,5.9,1.8),.08)
for y in [7.8,10.2]:
    cyl('Gate brazier foot',(19.4,y,.32),.24,.5,stone,D,n=8);cyl('Brazier bowl',(19.4,y,.66),.32,.22,iron,D,r2=.43,n=12);cyl('Flame',(19.4,y,.97),.20,.48,ember,D,r2=0,n=7)
beam('Weapon rack top',(21.6,11.5,1.35),(23.4,11.5,1.35),.065)
for x in [21.7,23.3]:beam('Weapon rack foot',(x,11.5,.3),(x,11.5,1.4),.06)
for x in [22,22.5,23]:
    beam('Spear shaft',(x,11.42,.4),(x,11.42,1.75),.025,woodlight);cyl('Spearhead',(x,11.42,1.87),.075,.25,iron,r2=0,n=4)
beam('Base signal flagpole',(27,13.5,.3),(27,13.5,5.6),.07,wood);banner(27,13.5,5.5)
scene['base_role']='Defended exit / legion frontier outpost';scene['tower_pads']=5;scene['unity_exported']=False
cam=scene.camera;cam.location=(46,-60,70);cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=77
scene.render.resolution_x=2048;scene.render.resolution_y=1536;scene.render.resolution_percentage=100;scene.render.engine='BLENDER_EEVEE'
scene.render.filepath=str(OUT/'Map_V3_Base_Beauty.png')
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'Blackpine_Map_V3_With_Base.blend'))
bpy.ops.render.render(write_still=True)
cam.location=(38,-7,20);cam.rotation_euler=(Vector((23.8,9.5,2))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.ortho_scale=19;scene.render.resolution_x=1600;scene.render.resolution_y=1300;scene.render.filepath=str(OUT/'Base_Closeup.png');bpy.ops.render.render(write_still=True)
(OUT/'complete.json').write_text(json.dumps({'base':'Stone and timber legion frontier outpost','tower_pads':5,'new_objects':sum(len(c.objects) for c in groups.values()),'unity_exported':False},indent=2))
