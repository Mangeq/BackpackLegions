"""Build articulated low-poly figures and a single motion atlas for Unity."""
from pathlib import Path
from math import radians, sin, pi
import bpy
from mathutils import Vector

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "ArtSource/BlackpineLegion"
EXPORT = ROOT / "Assets/Resources/BlackpineLegion"
ROLES = ["Scout", "Raider", "Guard", "Elite", "Warlord", "SkyWisp", "Sentinel"]
palette = dict(moss=(.18,.32,.10), pine=(.035,.105,.085), hide=(.22,.105,.045),
               iron=(.16,.22,.24), edge=(.47,.55,.52), brass=(.72,.42,.105),
               ember=(1,.48,.07), cloth=(.35,.06,.045), mist=(.18,.71,.64),
               bone=(.86,.78,.51), ink=(.023,.036,.027))

def build(role):
    bpy.ops.wm.read_factory_settings(use_empty=True)
    mats = {}
    for name, rgb in palette.items():
        mat = bpy.data.materials.new(name)
        mat.diffuse_color = (*rgb, 1)
        mat.use_nodes=True
        bsdf=mat.node_tree.nodes.get("Principled BSDF")
        bsdf.inputs["Base Color"].default_value=(*rgb,1)
        bsdf.inputs["Roughness"].default_value=.72
        mats[name] = mat
    joints, groups = {}, {}
    def joint(name, pos, parent=None):
        obj = bpy.data.objects.new(name, None)
        bpy.context.collection.objects.link(obj)
        obj.parent = joints.get(parent)
        obj.location = pos
        joints[name] = obj
        groups[name] = []
    def part(j, pos, size, color, shape="gem", tilt=0):
        if shape == "gem": bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1)
        elif shape == "plate": bpy.ops.mesh.primitive_cylinder_add(vertices=8, radius=1, depth=2)
        elif shape == "spike": bpy.ops.mesh.primitive_cone_add(vertices=6, radius1=1, depth=2)
        else: bpy.ops.mesh.primitive_cube_add(size=2)
        ob = bpy.context.object
        ob.parent = joints[j]
        ob.location, ob.scale = pos, size
        ob.rotation_euler.y = radians(tilt)
        ob.data.materials.append(mats[color])
        groups[j].append(ob)
    air = role == "SkyWisp"
    heavy = role in ("Guard","Warlord")
    human = role == "Sentinel"
    joint("Body",(0,0,0))
    if air:
        part("Body",(0,0,.50),(.23,.19,.36),"mist")
        part("Body",(0,-.12,.63),(.17,.12,.13),"pine")
        for s in (-1,1):
            part("Body",(s*.085,-.21,.64),(.046,.035,.034),"ember")
            name = "Wing" + str(s)
            joint(name,(s*.12,0,.55),"Body")
            for k in range(3):
                part(name,(s*(.22-.035*k),.08*k,.13-.11*k),
                     (.10,.048,.32-.04*k),"mist" if k!=1 else "bone","gem",s*58)
        part("Body",(0,.05,.21),(.11,.12,.24),"brass","spike",180)
    else:
        w = .32 if heavy else .24
        armor = "iron" if heavy or human or role=="Elite" else "pine"
        part("Body",(0,0,.52),(w,.17,.29),armor)
        part("Body",(0,0,.36),(w*.86,.18,.075),"hide","plate")
        part("Body",(0,-.17,.37),(.058,.025,.048),"brass","box")
        part("Body",(0,-.015,.87),(.23,.17,.20),"bone" if human else "moss")
        for s in (-1,1):
            if not human: part("Body",(s*.23,0,.9),(.08,.085,.15),"moss","spike",-s*62)
            part("Body",(s*.088,-.15,.89),(.053,.035,.035),"ember" if not human else "ink")
            part("Body",(s*.097,-.152,.81),(.028,.03,.062),"bone","spike")
            leg="Leg"+str(s); joint(leg,(s*.13,0,.35),"Body")
            part(leg,(0,0,-.12),(.092,.10,.16),"hide")
            part(leg,(0,-.055,-.255),(.103,.14,.087),"ink")
            arm="Arm"+str(s); joint(arm,(s*w,0,.65),"Body")
            part(arm,(0,0,-.105),(.09,.10,.18),"moss" if not human else "hide")
            part(arm,(0,0,-.24),(.078,.09,.076),"hide")
            part(arm,(0,0,0),(.13,.13,.095),armor if not heavy else "edge")
        hood = role in ("Scout","Elite")
        part("Body",(0,.008,1.005),(.23,.18,.15 if hood else .095),
             "pine" if hood else "iron","spike" if hood else "plate")
        if role in ("Guard","Sentinel","Warlord"):
            part("Arm-1",(-.04,-.10,-.21),(.18,.07,.25),"iron","gem")
            part("Arm-1",(-.04,-.164,-.21),(.10,.04,.15),"brass","gem")
        if role=="Warlord":
            for s in (-1,0,1):
                part("Body",(s*.16,0,1.13),(.052,.07,.145),"brass","spike",s*20)
            part("Body",(0,.13,.49),(.31,.10,.35),"cloth")
        if role in ("Elite","Guard"):
            part("Arm1",(0,-.05,-.10),(.032,.032,.43),"hide","plate")
            part("Arm1",(0,-.05,.38),(.085,.04,.13),"edge","spike")
        elif role in ("Warlord","Raider"):
            part("Arm1",(0,-.06,-.12),(.045,.045,.24),"hide","plate")
            part("Arm1",(.065,-.06,.10),(.16,.043,.115),"edge")
        else:
            part("Arm1",(0,-.06,-.05),(.055,.033,.20),"edge","spike")
        # Regimental dress reads from the overhead battlefield camera.
        if role=="Raider":
            for side in (-1,1):
                part("Body",(side*.19,0,1.09),(.055,.06,.14),"bone","spike",side*43)
            part("Body",(0,-.159,.57),(.069,.031,.22),"cloth","box",-24)
            part("Body",(0,.11,.63),(.21,.13,.10),"cloth")
        if role=="Scout":
            part("Body",(0,-.14,.79),(.21,.092,.074),"pine")
            part("Body",(-.11,-.14,.59),(.047,.030,.21),"hide","box",-31)
            part("Body",(-.065,-.156,.60),(.032,.027,.047),"brass")
        if role in ("Guard","Warlord","Sentinel"):
            for side in (-1,1):
                part("Body",(side*.11,-.154,.62),(.112,.06,.12),"edge")
                part("Body",(side*.20,-.07,.89),(.052,.115,.11),"iron")
            part("Body",(0,-.202,.59),(.058,.025,.085),"brass")
        if role=="Elite":
            part("Body",(0,.13,.56),(.26,.105,.30),"pine")
            part("Body",(0,-.17,.62),(.13,.04,.105),"mist")
            for k in range(3):
                part("Arm-1",(-.09*k,.008,.07+k*.024),(.047,.065,.11),"bone","spike",-25-12*k)
            part("Arm1",(0,-.06,.47),(.10,.04,.19),"mist","spike")
        if role=="Warlord":
            for side in (-1,1):
                part("Arm"+str(side),(side*.09,0,.07),(.17,.155,.13),"brass")
                part("Arm"+str(side),(side*.18,0,.16),(.063,.064,.17),"bone","spike",side*39)
            part("Body",(0,-.19,.39),(.12,.03,.23),"cloth","gem")
        if role=="Scout":
            part("Body",(.15,.15,.51),(.09,.095,.22),"hide","plate",-18)
    # One mesh per moving joint, with the pivot at its articulation point.
    for name, meshes in groups.items():
        if not meshes: continue
        bpy.ops.object.select_all(action="DESELECT")
        for ob in meshes: ob.select_set(True)
        bpy.context.view_layer.objects.active=meshes[0]
        bpy.ops.object.join()
        bpy.context.object.name=name+"_Geo"
    scene=bpy.context.scene
    scene.render.fps=24
    scene.frame_end=144
    # Idle, locomotion, attack, flinch, fall. Contacts land at frame 78.
    for frame in range(1,145):
        mode=(frame-1)//24
        t=((frame-1)%24)/23
        p=sin(2*pi*t)
        for name, ob in joints.items():
            rx=ry=rz=0
            if name=="Body":
                ob.location.z=(.03*(1-p*p) if mode==1 else .012*p if mode==0 else 0)
                if mode==2: rx=-10*t
                if mode==3: rx=18*(1-t)
                if mode==4: rx=-14*sin(pi*t)
                if mode==5: rx=-82*t; ob.location.z=-.15*t
            elif name.startswith("Wing"):
                ry=(1 if name=="Wing1" else -1)*(18+30*p)
            elif name.startswith("Leg"):
                rx=(1 if name=="Leg1" else -1)*32*p if mode==1 else 0
            elif name.startswith("Arm"):
                side=1 if name=="Arm1" else -1
                rx=-side*27*p if mode==1 else -9+3*p
                if name=="Arm1" and mode==2: rx=-10-97*t
                if name=="Arm1" and mode==3: rx=(-107+139*min(1,t*4))*(1-max(0,(t-.25)/.75))
            ob.rotation_euler=(radians(rx),radians(ry),radians(rz))
            ob.keyframe_insert(data_path="rotation_euler",frame=frame)
            if name=="Body": ob.keyframe_insert(data_path="location",frame=frame)
    scene.frame_set(1)
    bpy.context.preferences.filepaths.save_version=0
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/(role+".blend")))
    bpy.ops.export_scene.fbx(filepath=str(EXPORT/(role+".fbx")),add_leaf_bones=False,
        bake_anim_use_all_actions=False,bake_anim_use_nla_strips=False,
        bake_anim_simplify_factor=.2,axis_forward="-Z",axis_up="Y")
for role in ROLES: build(role)
