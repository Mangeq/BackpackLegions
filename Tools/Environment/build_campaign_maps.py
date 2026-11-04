"""Five original, low-poly tower-defense dioramas. Run with Blender --background --python."""
import bpy, math, random, json
from pathlib import Path
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Assets/Art/CampaignMaps'; OUT.mkdir(parents=True,exist_ok=True)
SOURCE=ROOT/'ArtSource/CampaignMaps'; SOURCE.mkdir(parents=True,exist_ok=True)
THEMES=[
 ('Pinewatch',[( -23,-8),(-14,-8),(-10,-3),(-13,4),(-5,8),(3,5),(7,-3),(15,-5),(23,5)],(0.25,.39,.19),(.57,.42,.25)),
 ('AmberCanyon',[(-23,8),(-15,8),(-11,1),(-17,-7),(-6,-10),(1,-3),(-2,5),(9,9),(16,3),(23,-6)],(.57,.32,.17),(.78,.58,.32)),
 ('Mistfen',[(-23,-9),(-15,-9),(-12,-1),(-17,7),(-7,10),(0,3),(-2,-6),(8,-9),(14,-2),(13,7),(23,8)],(.20,.32,.27),(.43,.42,.30)),
 ('Frostgate',[(-23,7),(-16,7),(-11,1),(-16,-7),(-7,-10),(0,-5),(-2,4),(6,10),(14,7),(13,-3),(23,-7)],(.64,.76,.80),(.38,.45,.49)),
 ('Crownfall',[(-23,-8),(-17,-8),(-13,0),(-17,8),(-8,11),(-1,5),(-3,-5),(5,-10),(13,-6),(11,2),(16,9),(23,7)],(.31,.35,.25),(.59,.53,.40)),
]
def mat(name,c):
 m=bpy.data.materials.new(name);m.diffuse_color=(*c,1);m.use_nodes=True
 p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*c,1);p.inputs['Roughness'].default_value=.88
 return m
def assign(o,name,m,group):
 o.name=name;o.data.materials.append(m);groups.setdefault(group,[]).append(o);return o
def cube(name,p,s,m,group='Scenery'):
 bpy.ops.mesh.primitive_cube_add(size=1,location=p);o=bpy.context.object;o.scale=s
 return assign(o,name,m,group)
def cone(name,p,r,depth,m,group='Scenery',top=0,verts=7):
 bpy.ops.mesh.primitive_cone_add(vertices=verts,radius1=r,radius2=top,depth=depth,location=p)
 return assign(bpy.context.object,name,m,group)
def rock(p,scale,m):
 bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1,radius=1,location=p);o=bpy.context.object;o.scale=scale;o.rotation_euler=(random.random(),random.random(),random.random());assign(o,'Stone',m,'Scenery')
def smooth(points):
 pts=[Vector(p) for p in [points[0]]+points+[points[-1]]];out=[]
 for i in range(1,len(pts)-2):
  a,b,c,d=pts[i-1:i+3]
  for n in range(18):
   t=n/18;out.append(.5*((2*b)+(-a+c)*t+(2*a-5*b+4*c-d)*t*t+(-a+3*b-3*c+d)*t*t*t))
 out.append(Vector(points[-1]));return out
def ribbon(path,width,z,m,name):
 vs=[]
 for i,p in enumerate(path):
  tangent=(path[min(i+1,len(path)-1)]-path[max(0,i-1)]).normalized();normal=Vector((-tangent.y,tangent.x))
  for sign in [-1,1]:
   v=p+normal*width*.5*sign;vs.append((v.x,v.y,z))
 fs=[(2*i,2*i+2,2*i+3,2*i+1) for i in range(len(path)-1)]
 me=bpy.data.meshes.new(name);me.from_pydata(vs,[],fs);me.update();o=bpy.data.objects.new(name,me);bpy.context.collection.objects.link(o);assign(o,name,m,name)
def nearest(p,path): return min((p-q).length for q in path)
def choose_slots(path,count):
 candidates=[]
 for i in range(12,len(path)-12,3):
  t=(path[i+1]-path[i-1]).normalized();n=Vector((-t.y,t.x))
  for sign in [-1,1]:
   p=path[i]+n*3.25*sign
   if abs(p.x)<20 and abs(p.y)<12.8 and nearest(p,path)>2.9:candidates.append((i,p))
 chosen=[]
 # Spread platforms over the length of the road; alternate sides where possible.
 for k in range(count):
  desired=(k+1)*len(path)/(count+1)
  options=[(i,p) for i,p in candidates if all((p-q).length>4.8 for _,q in chosen)]
  if not options:raise RuntimeError('No free platform')
  chosen.append(min(options,key=lambda ip:abs(ip[0]-desired)))
 return [p for _,p in chosen]
reports=[]
for chapter,(name,points,ground_color,road_color) in enumerate(THEMES,1):
 bpy.ops.wm.read_factory_settings(use_empty=True);random.seed(311+chapter);groups={}
 ground=mat('Meadow',ground_color);road=mat('Trail',road_color);edge=mat('TrailBorder',tuple(v*.78 for v in road_color))
 stone=mat('Stone',(.38,.42,.38) if chapter!=2 else (.55,.26,.13));stone_light=mat('CutStone',(.65,.65,.53));wood=mat('Timber',(.24,.14,.08))
 leaf=mat('Pine',(.08,.22,.14) if chapter!=4 else (.21,.36,.37));leaf2=mat('PineTips',(.18,.34,.17) if chapter!=4 else (.79,.87,.87))
 gold=mat('Brass',(.84,.59,.20));roof=mat('Roof',(.22,.29,.37));water=mat('Water',(.15,.35,.36));banner=mat('Banner',(.63,.15,.10))
 path=smooth(points);slots=choose_slots(path,chapter+4)
 cube('Diorama foundation',(0,0,-1.1),(54,35,1.8),stone,'Ground')
 cube('Earth',(0,0,-.25),(53,34,.65),ground,'Ground')
 ribbon(path,3.25,.09,edge,'RoadBorder');ribbon(path,2.8,.13,road,'Road')
 for idx,p in enumerate(slots,1):
  group=f'Pad_{idx:02}'
  cone('Foundation',(p.x,p.y,.18),1.48,.25,stone,group,top=1.48,verts=12)
  cone('Build platform',(p.x,p.y,.32),1.25,.12,stone_light,group,top=1.25,verts=12)
  # A clear gold rim makes available platforms readable against the landscape.
  for j in range(8):
   a=j*math.tau/8;cone('Rim',(p.x+1.30*math.cos(a),p.y+1.30*math.sin(a),.4),.10,.10,gold,group,top=.10,verts=5)
 # Scatter vegetation only outside roads and platforms; group batching keeps draw calls bounded.
 for _ in range(290):
  p=Vector((random.uniform(-25.3,25.3),random.uniform(-15.5,15.5)))
  if nearest(p,path)<2.7 or any((p-q).length<2.05 for q in slots) or (p-Vector(points[-1])).length<4:continue
  if chapter==2:
   rock((p.x,p.y,.45),(random.uniform(.5,1.4),random.uniform(.5,1.1),random.uniform(.6,2)),stone)
   if random.random()<.3:
    cone('Cactus',(p.x,p.y,1.2),.25,2.4,leaf,top=.22)
  elif random.random()<.65:
   h=random.uniform(1.8,3.7);cone('Trunk',(p.x,p.y,h*.32),.16,h*.64,wood,top=.12)
   for j in range(3):cone('Pine',(p.x,p.y,h*(.44+j*.18)),h*(.30-j*.065),h*.52,leaf2 if j==2 else leaf)
  else:rock((p.x,p.y,.35),(random.uniform(.3,.9),random.uniform(.3,.9),random.uniform(.4,1.0)),stone)
 # Theme landmarks: farms, canyon spires, marsh pools, icy crags and ruined walls.
 for j in range(5):
  x=-20+j*10;y=14 if j%2 else -14
  if chapter==1:
   cube('Cottage',(x,y,.8),(2.2,1.8,1.5),stone_light);o=cone('CottageRoof',(x,y,2),1.7,1.3,roof,verts=4);o.rotation_euler.z=math.pi/4
   for k in range(5):cube('Field',(x-1+k*.5,y-2.2,.13),(.23,1,.1),gold)
  elif chapter==2:
   for k in range(3):cone('Sandstone spire',(x+k*.9,y,1+k*.3),1.5-k*.2,2+k*.6,stone,top=.6,verts=6)
  elif chapter==3:
   cone('MarshPool',(x,y,.12),2,.07,water,top=2,verts=18)
   for k in range(6):cone('Reeds',(x+math.cos(k)*2,y+math.sin(k)*1.4,.6),.08,1.1,gold,top=.04,verts=4)
  elif chapter==4:
   rock((x,y,1.5),(2.7,2,2.7),stone);cone('Snowcap',(x,y,3.4),2.1,1.5,leaf2,verts=6)
  else:
   for k in range(4):cube('RuinedWall',(x+k*.8,y,.8+(.3 if k%2 else 0)),(.72,.6,1.7),stone_light)
 # Gate architecture at exit, with a road-width opening.
 end=Vector(points[-1]);prev=path[-5];t=(end-prev).normalized();n=Vector((-t.y,t.x))
 for sign in [-1,1]:
  p=end+n*2.3*sign
  cone('GateTower',(p.x,p.y,1.7),1,3.3,stone_light,top=.9,verts=8)
  cone('GateRoof',(p.x,p.y,3.9),1.25,1.2,roof,verts=8)
  cube('Flagpole',(p.x,p.y,4.9),(.08,.08,1.4),wood)
  cube('Flag',(p.x+.45,p.y,5.2),(.9,.08,.5),banner)
 cube('GateLintel',(end.x,end.y,3),(4.6,.6,.55),stone_light)
 # Batch into semantic meshes, including a separate road for navigation baking.
 for group,objs in groups.items():
  bpy.ops.object.select_all(action='DESELECT')
  for o in objs:o.select_set(True)
  bpy.context.view_layer.objects.active=objs[0]
  if len(objs)>1:bpy.ops.object.join()
  o=bpy.context.object;o.name=group;bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
 bpy.ops.object.select_all(action='SELECT')
 bpy.ops.export_scene.fbx(filepath=str(OUT/f'{chapter:02}_{name}.fbx'),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,bake_anim=False,add_leaf_bones=False)
 report={'chapter':chapter,'name':name,'slots':[[p.x,.4,p.y] for p in slots],'spawn':[points[0][0],.13,points[0][1]],'goal':[end.x,.13,end.y],'materials':[{'name':m.name,'color':list(m.diffuse_color)} for m in bpy.data.materials],'route':[[p.x,.13,p.y] for p in path]}
 (OUT/f'{chapter:02}_{name}.json').write_text(json.dumps(report,indent=2));reports.append(report)
 # Keep editable native sources with a useful camera and lighting.
 bpy.ops.object.light_add(type='AREA',location=(-12,-15,28));bpy.context.object.data.energy=15000;bpy.context.object.data.shape='DISK';bpy.context.object.data.size=20
 bpy.ops.object.light_add(type='SUN',location=(-12,-15,28));bpy.context.object.rotation_euler=(.45,-.4,-.5);bpy.context.object.data.energy=2.5;bpy.context.object.data.angle=.15
 bpy.ops.object.camera_add(location=(0,-37,45));cam=bpy.context.object;cam.rotation_euler=(Vector((0,0,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='ORTHO';cam.data.ortho_scale=61;bpy.context.scene.camera=cam
 scene=bpy.context.scene;scene.render.engine='BLENDER_EEVEE';scene.render.resolution_x=1440;scene.render.resolution_y=960;scene.render.resolution_percentage=100
 scene.world=bpy.data.worlds.new('Campaign World');scene.world.color=(.32,.36,.40);scene.render.filepath=str(SOURCE/f'{chapter:02}_{name}.png')
 bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/f'{chapter:02}_{name}.blend'))
 bpy.ops.render.render(write_still=True)
(OUT/'manifest.json').write_text(json.dumps({'maps':[{'chapter':r['chapter'],'name':r['name'],'slots':len(r['slots'])} for r in reports]},indent=2))
