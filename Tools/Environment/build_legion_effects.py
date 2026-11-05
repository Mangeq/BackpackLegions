"""Blackpine projectile and impact silhouettes. Each burst is one renderable mesh."""
import bpy, math
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[2]
out=root/"Assets/Resources/BlackpineLegion/Effects"
out.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
def material(name,color):
    mat=bpy.data.materials.new(name);mat.diffuse_color=(*color,1)
    mat.use_nodes=True
    bsdf=mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value=(*color,1)
    bsdf.inputs["Roughness"].default_value=.55
    return mat
spark=material("Golden glint",(1,.77,.28))
fire=material("Ember",(1,.21,.025))
water=material("Water glass",(.08,.64,.94))
earth=material("Ochre shale",(.52,.32,.13))
air=material("Pale jade",(.42,1,.77))
steel=material("Bright steel",(.68,.76,.77))
wood=material("Arrow shaft",(.31,.15,.055))
violet=material("Spell crystal",(.55,.22,1))
def shard(pos,scale,mat,shape="gem"):
    if shape=="tip": bpy.ops.mesh.primitive_cone_add(vertices=5,radius1=1,depth=2)
    elif shape=="rod": bpy.ops.mesh.primitive_cylinder_add(vertices=6,radius=1,depth=2)
    else:bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1)
    ob=bpy.context.object;ob.location=pos;ob.scale=scale;ob.data.materials.append(mat)
    return ob
def ribbon(angle,mat):
    verts=[];faces=[]
    for i in range(9):
        t=i/8;a=angle+1.5*t;r=.10+.28*t;w=.024*math.sin(t*math.pi)+.003
        for h in (-w,w):verts.append((r*math.cos(a),r*math.sin(a),.08+.09*math.sin(t*math.pi)+h))
        if i:faces.append((2*i-2,2*i-1,2*i+1,2*i))
    mesh=bpy.data.meshes.new("wind");mesh.from_pydata(verts,[],faces);mesh.update()
    ob=bpy.data.objects.new("wind",mesh);bpy.context.collection.objects.link(ob);mesh.materials.append(mat)
    return ob
for kind in ("Normal","Fire","Water","Earth","Air","Critical","Arrow","Spell"):
    pieces=[]
    if kind in ("Arrow","Spell"):
        if kind=="Arrow":
            pieces.append(shard((0,0,0),(.022,.022,.21),wood,"rod"))
            pieces.append(shard((0,0,.22),(.065,.037,.09),steel,"tip"))
            for side in (-1,1):
                pieces.append(shard((side*.03,0,-.16),(.044,.018,.07),spark))
        else:
            pieces.append(shard((0,0,0),(.11,.11,.19),violet))
            for i in range(3):
                a=i*math.tau/3
                pieces.append(shard((.10*math.cos(a),.10*math.sin(a),-.07),(.035,.035,.075),spark))
        for ob in pieces:
            # FBX's Blender -Y axis points forward in Unity.
            ob.rotation_euler.x=math.pi/2
            x,y,z=ob.location;ob.location=(x,-z,y)
    else:
        for i in range(8 if kind=="Critical" else 5):
            a=i*math.tau/(8 if kind=="Critical" else 5);dx,dy=math.cos(a),math.sin(a)
            if kind=="Air":pieces.append(ribbon(a,air));continue
            mat={"Normal":spark,"Fire":fire,"Water":water,"Earth":earth,"Critical":steel}[kind]
            height=.14 if kind=="Fire" else .08
            ob=shard((.21*dx,.21*dy,height),(.036,.038,.13) if kind!="Earth" else (.068,.06,.077),mat,
                     "tip" if kind in ("Fire","Normal","Critical") else "gem")
            ob.rotation_euler=Vector((dx,dy,1.6 if kind=="Fire" else .4)).to_track_quat("Z","Y").to_euler()
            pieces.append(ob)
            if kind in ("Water","Fire","Critical"):
                pieces.append(shard((.36*dx,.36*dy,.16),(.029,.027,.043),spark if kind!="Water" else water))
    bpy.ops.object.select_all(action="DESELECT")
    for ob in pieces:ob.select_set(True)
    bpy.context.view_layer.objects.active=pieces[0];bpy.ops.object.join()
    ob=bpy.context.object
    bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
    ob.name=kind
    bpy.ops.export_scene.fbx(filepath=str(out/(kind+".fbx")),use_selection=True,bake_anim=False,
        axis_forward="-Z",axis_up="Y")
    ob.location.x=list(("Normal","Fire","Water","Earth","Air","Critical","Arrow","Spell")).index(kind)
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(root/"ArtSource/BlackpineLegion/Effects.blend"))
