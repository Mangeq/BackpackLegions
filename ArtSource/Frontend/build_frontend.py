"""Original Blender sources for Backpack Legions front-end. No external assets."""
import bpy, math, random, os, json
from mathutils import Vector
random.seed(37)
ROOT=os.path.abspath(os.path.join(os.path.dirname(__file__),'../..'))
OUT=os.path.join(ROOT,'Assets/Art/Frontend'); os.makedirs(OUT,exist_ok=True)
def mat(name,color,metal=0):
    m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True
    p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=.83;p.inputs['Metallic'].default_value=metal
    return m
stone=mat('Warm limestone',(.36,.39,.34));dark=mat('Charcoal slate',(.075,.12,.105));gold=mat('Aged brass',(.48,.31,.115),.45)
green=mat('Pine needles',(.09,.20,.135));lightgreen=mat('Sage canopy',(.19,.30,.18));wood=mat('Wood',(.19,.115,.055));soil=mat('Earth',(.18,.23,.15));water=mat('River jade',(.14,.34,.34));road=mat('Sandstone trail',(.58,.48,.28));paper=mat('Parchment',(.56,.47,.31));mountain=mat('Mountain',(.32,.34,.28));crystal=mat('Crystal',(.31,.22,.44));cloth=mat('Banner',(.48,.18,.11))
def finish(o,name,material):o.name=name;o.data.materials.append(material);return o
def box(name,loc,scale,material,bevel=0):
    bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.scale=scale;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);finish(o,name,material)
    if bevel:mod=o.modifiers.new('Soft stone edges','BEVEL');mod.width=bevel;mod.segments=2;o.modifiers.new('Weighted normals','WEIGHTED_NORMAL')
    return o
def cone(name,loc,r,depth,material,r2=0,verts=8):
    bpy.ops.mesh.primitive_cone_add(vertices=verts,radius1=r,radius2=r2,depth=depth,location=loc);return finish(bpy.context.object,name,material)
def pine(x,y,z=.1,s=1):
    cone('Tree trunk',(x,y,z+.35*s),.09*s,.7*s,wood,r2=.07*s)
    for h,r in [(.65,.5),(1.0,.4),(1.32,.28)]:cone('Pine canopy',(x,y,z+h*s),r*s,.8*s,random.choice([green,lightgreen]))
def path(points,material,width,name):
    cu=bpy.data.curves.new(name,'CURVE');cu.dimensions='3D';cu.resolution_u=20;cu.bevel_depth=width;cu.bevel_resolution=3;s=cu.splines.new('BEZIER');s.bezier_points.add(len(points)-1)
    for p,co in zip(s.bezier_points,points):p.co=(co[0],co[1],co[2]*10);p.handle_left_type='AUTO';p.handle_right_type='AUTO'
    o=bpy.data.objects.new(name,cu);bpy.context.collection.objects.link(o);o.data.materials.append(material);o.scale.z=.1;cu.use_fill_caps=True
def fort(x,y,z=0,s=1):
    box('Keep foundation',(x,y,z+.15*s),(2.5*s,2.2*s,.3*s),dark,.1*s)
    box('Keep',(x,y,z+.8*s),(1.5*s,1.35*s,1.3*s),stone,.06*s)
    cone('Slate roof',(x,y,z+1.7*s),1.18*s,.65*s,dark,verts=4).rotation_euler.z=math.pi/4
    for dx,dy in [(-1,-.8),(1,-.8),(-1,.8),(1,.8)]:
        cone('Tower stone',(x+dx*s,y+dy*s,z+.85*s),.38*s,1.7*s,stone,r2=.32*s,verts=12)
        cone('Tower cap',(x+dx*s,y+dy*s,z+1.86*s),.46*s,.55*s,dark,verts=8)
        cone('Brass finial',(x+dx*s,y+dy*s,z+2.17*s),.07*s,.2*s,gold)
    box('Gate',(x,y-.69*s,z+.45*s),(.44*s,.04*s,.75*s),wood,.08*s)
    box('Banner',(x+.38*s,y-.7*s,z+1*s),(.24*s,.04*s,.53*s),cloth)
    for dx in [-.47,0,.47]:box('Keep window',(x+dx*s,y-.7*s,z+1.2*s),(.12*s,.03*s,.22*s),gold,.025*s)
def scene(name,w,h):
    sc=bpy.data.scenes.new(name);bpy.context.window.scene=sc
    sc.render.engine='CYCLES';sc.cycles.samples=24;sc.cycles.use_denoising=True
    sc.render.resolution_x=w;sc.render.resolution_y=h;sc.render.resolution_percentage=100
    sc.world=bpy.data.worlds.new(name+' world');sc.world.use_nodes=True;sc.world.node_tree.nodes['Background'].inputs[0].default_value=(.12,.18,.16,1);sc.world.node_tree.nodes['Background'].inputs[1].default_value=.45
    sc.view_settings.view_transform='AgX'
    return sc
def camera(loc,target,scale):
    bpy.ops.object.camera_add(location=loc);c=bpy.context.object;c.rotation_euler=(Vector(target)-c.location).to_track_quat('-Z','Y').to_euler();c.data.type='ORTHO';c.data.ortho_scale=scale;bpy.context.scene.camera=c
def lights():
    bpy.ops.object.light_add(type='AREA',location=(-4,-6,12));l=bpy.context.object;l.data.energy=1800;l.data.shape='DISK';l.data.size=9
    bpy.ops.object.light_add(type='AREA',location=(6,4,8));l=bpy.context.object;l.data.energy=1200;l.data.color=(.95,.73,.44);l.data.size=7
# Hero: a small fortress above a forest road, deliberately weighted to the right.
sc=scene('MainMenuWorld',1920,1080)
box('Backdrop',(0,0,-1),(200,200,1),dark)
cone('Island lower',(2,1,-.1),7,1.2,soil,r2=6.4,verts=12)
cone('Moss plateau',(2,1,.35),6.5,.35,soil,r2=6.3,verts=12)
fort(2,1,.55,1.6)
path([(-4,-4,.6),(-2,-3,.6),(1,-3,.6),(2,-1,.6)],road,.34,'Winding approach')
path([(-3,5,.58),(-2,3,.58),(-3,1,.58),(-4,-1,.58),(-3,-5,.58)],water,.3,'River')
for i in range(95):
    x=random.uniform(-4,8);y=random.uniform(-4,6)
    if (x-2)**2+(y-1)**2>12 and (x-2)**2+(y-1)**2<36 and abs(x+3)>.7:pine(x,y,.55,random.uniform(.6,1.25))
for x,y,s in [(4,5,1.8),(6,4,1.3),(-1,5,1.1)]:cone('Far mountain',(x,y,.7+s/2),s,s*2,mountain,verts=5)
camera((14,-20,17),(0,0,1),22);lights();sc.render.filepath=os.path.join(OUT,'MenuFortress.png');bpy.ops.render.render(write_still=True)
# Campaign: relief map, with a quiet parchment base and winding route.
sc=scene('CampaignRelief',1600,1000)
box('Parchment board',(0,0,-.15),(19,12,.3),paper,.15)
path([(-8,4,.03),(-4,3,.03),(-1,4,.03),(1,2,.03),(2,-1,.03),(6,-3,.03),(9,-3,.03)],water,.25,'Map river')
nodes=[(-5,-2.5),(-2,-.8),(1,1.2),(4,.2),(5.8,3)]
path([(x,y,.08) for x,y in nodes],road,.12,'Campaign road')
for i in range(150):
    x=random.uniform(-8,8);y=random.uniform(-4.7,4.7)
    if all((x-a)**2+(y-b)**2>1.3 for a,b in nodes) and (y>2.5 or x<-5.8 or y<-3.5):pine(x,y,.05,random.uniform(.23,.42))
for x,y in [(-3,1.4),(-2,2),(-4,1.8),(3,-2.8),(4,-2.7),(5,-2.4),(6,1.6)]:cone('Relief mountain',(x,y,.45),random.uniform(.4,.7),.9,mountain,verts=5)
fort(-5,-2.5,.1,.35);fort(5.8,3,.1,.30)
for x,y in [(-6,1),(0,-3),(6,-1)]:
    for i in range(3):box('Map ruin',(x+i*.18,y,.17),(.12,.35,.34),stone,.02)
camera((0,-9,22),(0,0,0),18);lights();sc.render.filepath=os.path.join(OUT,'CampaignMap.png');bpy.ops.render.render(write_still=True)
from bpy_extras.object_utils import world_to_camera_view
with open(os.path.join(ROOT,'ArtSource/Frontend/map_nodes.json'),'w') as f:json.dump([list(world_to_camera_view(sc,sc.camera,Vector((x,y,.2))))[:2] for x,y in nodes],f)
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(ROOT,'ArtSource/Frontend/BackpackLegions_Frontend.blend'))
print('FRONTEND_ART_COMPLETE',flush=True)
