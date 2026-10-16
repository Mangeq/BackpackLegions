import bpy, math, random, json
from pathlib import Path
ROOT=Path('C:/Users/Emir/BackPackDefance')
OUT=ROOT/'ArtSource/BlackpineUIKit'
PNG=ROOT/'Assets/Art/BlackpineUIKit/Textures'
OUT.mkdir(parents=True,exist_ok=True);PNG.mkdir(parents=True,exist_ok=True)
random.seed(15)
W,H=1920,1080
font=bpy.data.fonts.load('C:/Windows/Fonts/georgia.ttf')
bold=bpy.data.fonts.load('C:/Windows/Fonts/georgiab.ttf')
sans=bpy.data.fonts.load('C:/Windows/Fonts/bahnschrift.ttf')
C={'ink':'111C1B','panel':'192724','panel2':'23352F','gold':'C8A66B','bright':'F0D399','cream':'ECE4CE','muted':'9CA99B','green':'537C59','red':'884936','parch':'B99B64','brown':'665231','line':'415048','black':'080F0F'}
def rgba(c):
    c=C.get(c,c)
    def linear(v):return v/12.92 if v<=.04045 else ((v+.055)/1.055)**2.4
    return tuple(linear(int(c[i:i+2],16)/255) for i in (0,2,4))+(1,)
def material(c):
    key='UI_'+c
    if key in bpy.data.materials:return bpy.data.materials[key]
    m=bpy.data.materials.new(key);m.diffuse_color=rgba(c);m.use_nodes=True
    n=m.node_tree.nodes;n.clear();out=n.new('ShaderNodeOutputMaterial');e=n.new('ShaderNodeEmission');e.inputs[0].default_value=rgba(c);m.node_tree.links.new(e.outputs[0],out.inputs[0]);return m
def poly(name,points,z,c):
    depth=z*20+len(bpy.context.scene.objects)*.015
    me=bpy.data.meshes.new(name);me.from_pydata([(x,H-y,depth) for x,y in points],[],[tuple(range(len(points)))]);me.update()
    o=bpy.data.objects.new(name,me);bpy.context.scene.collection.objects.link(o);o.data.materials.append(material(c));return o
def rect(x,y,w,h,c,z=1,cut=0,name='Panel'):
    return poly(name,[(x+cut,y),(x+w-cut,y),(x+w,y+cut),(x+w,y+h-cut),(x+w-cut,y+h),(x+cut,y+h),(x,y+h-cut),(x,y+cut)],z,c)
def line(points,c='gold',width=2,z=4,name='Line'):
    cu=bpy.data.curves.new(name,'CURVE');cu.dimensions='3D';cu.bevel_depth=width/2;cu.bevel_resolution=1;s=cu.splines.new('POLY');s.points.add(len(points)-1)
    for p,(x,y) in zip(s.points,points):p.co=(x,H-y,z*20+len(bpy.context.scene.objects)*.015,1)
    o=bpy.data.objects.new(name,cu);bpy.context.scene.collection.objects.link(o);o.data.materials.append(material(c));return o
def text(body,x,y,size=24,c='cream',kind='sans',align='LEFT',z=8):
    cu=bpy.data.curves.new('Text '+body,'FONT');cu.body=body;cu.font={'serif':font,'bold':bold,'sans':sans}[kind];cu.size=size;cu.align_x=align
    o=bpy.data.objects.new(body,cu);bpy.context.scene.collection.objects.link(o);o.location=(x,H-y-size*.8,z*20+len(bpy.context.scene.objects)*.015);o.data.materials.append(material(c));return o
def circle(x,y,r,c,z=3,n=48):return poly('Medallion',[(x+math.cos(i*math.tau/n)*r,y+math.sin(i*math.tau/n)*r) for i in range(n)],z,c)
def diamond(x,y,r,c='gold',z=5):return poly('Diamond',[(x,y-r),(x+r,y),(x,y+r),(x-r,y)],z,c)
def frame(x,y,w,h,fill='panel',edge='gold',z=2):
    rect(x,y,w,h,edge,z,12);rect(x+2,y+2,w-4,h-4,fill,z+.1,10)
    for xx,yy in [(x,y),(x+w,y),(x,y+h),(x+w,y+h)]:diamond(xx,yy,4,edge,z+.3)
def button(label,x,y,w=320,h=64,primary=False,z=10):
    rect(x,y,w,h,'bright' if primary else 'gold',z,9);rect(x+2,y+2,w-4,h-4,'green' if primary else 'panel',z+.2,8)
    line([(x+15,y+7),(x+w-15,y+7)],'gold',1,z+.3)
    text(label,x+w/2,y+h/2-13,26,'cream','sans','CENTER',z+1)
def star(x,y,r=11,c='gold',z=12):
    return poly('Star',[(x+math.cos(-math.pi/2+i*math.pi/5)*(r if i%2==0 else r*.42),y+math.sin(-math.pi/2+i*math.pi/5)*(r if i%2==0 else r*.42)) for i in range(10)],z,c)
def shield(x,y,s=1,c='gold',z=5):
    pts=[(-40,-45),(40,-45),(36,15),(24,35),(0,54),(-24,35),(-36,15)]
    poly('Shield rim',[(x+a*s,y+b*s) for a,b in pts],z,c)
    poly('Shield inset',[(x+a*s*.83,y+b*s*.83) for a,b in pts],z+.1,'panel2')
    rect(x-4*s,y-26*s,8*s,53*s,'bright',z+.2);rect(x-19*s,y-9*s,38*s,7*s,'bright',z+.3)
def gear(x,y,r=15):
    circle(x,y,r,'gold',20,12);circle(x,y,r*.53,'panel',21);circle(x,y,r*.23,'gold',22)
def tree(x,y,s=1,c='brown',z=3):
    line([(x,y),(x,y+28*s)],c,2*s,z)
    for k in range(3):
        yy=y+k*9*s;rr=(17-k*4)*s
        line([(x-rr,yy+12*s),(x,yy-12*s),(x+rr,yy+12*s)],c,1.6*s,z)
def mountain(x,y,s=1):
    line([(x-34*s,y+22*s),(x,y-32*s),(x+39*s,y+22*s)],'brown',2,3)
    line([(x-12*s,y-12*s),(x,y-4*s),(x+9*s,y-11*s),(x+17*s,y+2*s)],'brown',1.5,3)
def parchment():
    o=rect(0,0,W,H,'parch',0)
    m=bpy.data.materials.new('Parchment / procedural fibres');m.use_nodes=True;n=m.node_tree.nodes;n.clear()
    out=n.new('ShaderNodeOutputMaterial');e=n.new('ShaderNodeEmission');noise=n.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=85;noise.inputs['Detail'].default_value=3
    ramp=n.new('ShaderNodeValToRGB');ramp.color_ramp.elements[0].color=rgba('9C7B48');ramp.color_ramp.elements[1].color=rgba('D3B884');m.node_tree.links.new(noise.outputs['Fac'],ramp.inputs[0]);m.node_tree.links.new(ramp.outputs[0],e.inputs[0]);m.node_tree.links.new(e.outputs[0],out.inputs[0]);o.data.materials.clear();o.data.materials.append(m)
def backdrop():
    rect(0,0,W,H,'black',0)
    # Tonal forest silhouettes are real Blender geometry.
    for i in range(28):
        x=random.uniform(0,W);y=random.uniform(160,H);s=random.uniform(2,5)
        tree(x,y,s,'14221E',.5)
    for i in range(45):circle(random.uniform(50,W),random.uniform(100,H),random.uniform(1,2),'gold',1,8)
def topbar(section):
    rect(0,0,W,100,'ink',15);line([(48,99),(1872,99)],'gold',1,16)
    shield(72,48,.36,z=16);text('BACKPACK LEGIONS',108,35,25,'cream','bold',z=17)
    text(section,650,39,20,'muted',z=17)
    for x,label,value in [(1330,'ALTIN','2.450'),(1520,'KRİSTAL','38'),(1705,'YILDIZ','27 / 60')]:
        diamond(x,50,8,'gold',17);text(label,x+18,22,13,'muted',z=17);text(value,x+18,45,23,'cream',z=17)
def footer(active=''):
    rect(0,972,W,108,'ink',15);line([(48,972),(1872,972)],'gold',1,16)
    for i,label in enumerate(['SEFER HARİTASI','KAHRAMANLAR','ÇANTA','GELİŞTİRMELER']):
        x=455+i*315
        if label==active:rect(x-20,989,280,68,'panel2',16,6);line([(x,1062),(x+240,1062)],'gold',3,17)
        diamond(x+8,1023,9,'gold' if label==active else 'muted',18);text(label,x+34,1013,21,'cream' if label==active else 'muted',z=18)
    gear(80,1024);text('AYARLAR',110,1013,18,'muted',z=18)
def setup(name):
    s=bpy.data.scenes.new(name);bpy.context.window.scene=s
    s.render.engine='BLENDER_EEVEE'
    s.render.resolution_x=W;s.render.resolution_y=H;s.render.resolution_percentage=100
    s.render.image_settings.file_format='PNG';s.render.film_transparent=False
    s.view_settings.view_transform='Standard'
    ca=bpy.data.cameras.new('Orthographic UI');cam=bpy.data.objects.new('Camera',ca);s.collection.objects.link(cam);cam.location=(W/2,H/2,1500);ca.type='ORTHO';ca.ortho_scale=W;ca.lens=50;ca.clip_end=3000;s.camera=cam
    s.world=bpy.data.worlds.new(name+' World');s.world.color=(0,0,0)
    return s
scenes=[]
s=setup('01_Loading');scenes.append(s);backdrop()
# Large embossed crest with stepped metal outlines.
for r,c in [(159,'panel2'),(153,'gold'),(149,'ink'),(136,'line'),(133,'ink')]:circle(960,345,r,c,3)
shield(960,335,1.65,z=5)
for side in [-1,1]:
    line([(960+side*35,420),(960+side*104,286)],'gold',8,4)
    for i in range(7):diamond(960+side*(60+i*6),405-i*15,9,'gold',5)
text('BACKPACK',960,544,38,'gold','serif','CENTER')
text('LEGIONS',960,590,90,'cream','bold','CENTER')
text('Küçük bir çanta. Büyük bir krallık.',960,708,25,'muted','sans','CENTER')
text('KARAKOL HAZIRLANIYOR',460,844,17,'gold')
text('% 68',1460,844,19,'cream',align='RIGHT')
frame(460,879,1000,15,'black','line');rect(463,882,676,9,'gold',5);diamond(1139,886,7,'bright',6)
text('İPUCU',960,941,14,'gold',align='CENTER');text('Uyumlu eşyaları yan yana yerleştirerek sinerjilerini güçlendir.',960,971,21,'muted',align='CENTER')
text('v0.1  /  TASARIM ÖNİZLEMESİ',60,1030,13,'line')
s=setup('02_Main_Menu');scenes.append(s);backdrop()
text('BİR KRALLIK. SON BİR SAVUNMA.',120,126,17,'gold')
text('BACKPACK',116,187,42,'gold','serif');text('LEGIONS',110,242,101,'cream','bold')
line([(120,372),(630,372)],'gold',2)
text('Çantanı hazırla. Birliğini kur.',120,412,28,'cream');text('Karanlık ormanın sınırında destanını yaz.',120,455,23,'muted')
button('MACERAYA DEVAM ET',120,551,510,78,True)
button('YENİ SEFER',120,649,510,67)
button('AYARLAR',120,735,245,62);button('OYUNDAN ÇIK',385,735,245,62)
text('SON SEFER',120,858,14,'gold');text('Karaçam Ormanı  /  Bölüm 04',120,889,24,'cream')
# Monumental right-hand crest and cartographic rings.
for r,c in [(310,'panel'),(302,'gold'),(298,'ink'),(267,'line'),(264,'ink')]:circle(1305,510,r,c,3)
for i in range(36):
    a=i*math.tau/36;line([(1305+math.cos(a)*276,510+math.sin(a)*276),(1305+math.cos(a)*289,510+math.sin(a)*289)],'gold',2,4)
shield(1305,490,3.7,z=5)
for side in [-1,1]:
    line([(1305+side*125,698),(1305+side*205,355)],'gold',5,4)
    for i in range(10):diamond(1305+side*(136+i*7),678-i*30,15,'gold',5)
frame(1130,845,350,74,'panel','gold',8);text('SEFER SENİ BEKLİYOR',1305,870,22,'gold',align='CENTER',z=10)
line([(60,996),(1860,996)],'line',1);text('BACKPACK LEGIONS',60,1027,16,'muted');text('TR  /  TÜRKÇE',1840,1027,17,'muted',align='RIGHT')
s=setup('03_Chapter_Map');scenes.append(s);parchment()
# Map geography is editable curves and geometry, not a flattened screenshot.
for i in range(72):
    x=random.uniform(555,1850);y=random.uniform(140,895)
    if 830<x<1540 and 300<y<730:continue
    if i%3==0:mountain(x,y,random.uniform(.5,1.1))
    else:tree(x,y,random.uniform(.5,1.0))
river=[(1540,100),(1490,190),(1560,290),(1525,390),(1580,490),(1530,590),(1650,720),(1620,830),(1710,975)]
line(river,'8D9478',29,1);line(river,'B8BE9A',19,2)
text('K A R A Ç A M   O R M A N I',1120,164,30,'brown','serif','CENTER',4)
text('Sisli Sırtlar',1660,300,23,'brown','serif','CENTER',4);text('Eski Krallık Yolu',1060,845,23,'brown','serif','CENTER',4)
nodes=[(660,737),(805,658),(955,695),(1080,566),(1190,421),(1350,488),(1445,351),(1625,414),(1740,560)]
for i in range(len(nodes)-1):
    a,b=nodes[i],nodes[i+1]
    for j in range(12):
        t=j/12;circle(a[0]+(b[0]-a[0])*t,a[1]+(b[1]-a[1])*t,3,'cream' if i<3 else 'brown',5,8)
for i,(x,y) in enumerate(nodes):
    if i==3:
        circle(x,y,60,'cream',6);circle(x,y,55,'gold',7);circle(x,y,49,'green',8)
    else:circle(x,y,37,'gold' if i<3 else 'brown',6);circle(x,y,33,'panel' if i<3 else '8B7959',7)
    if i<4:shield(x,y,.50,z=9)
    else:
        rect(x-12,y-5,24,22,'parch',10,3);line([(x-8,y-5),(x-8,y-18),(x+8,y-18),(x+8,y-5)],'parch',4,10)
    if i<3:
        for dx in [-21,0,21]:
            star(x+dx,y-48,11,'brown',11);star(x+dx,y-48,8,'bright',12)
    text(str(i+1).zfill(2),x,y+(70 if i==3 else 48),17,'ink',align='CENTER',z=11)
frame(47,140,430,786,'ink','gold',15)
text('BÖLÜM I',82,178,16,'gold',z=17);text('Karaçam',80,223,51,'cream','bold',z=17);text('Ormanı',80,283,51,'cream','bold',z=17)
line([(82,360),(440,360)],'line',1,17)
text('04  /  ESKİ TAŞ KÖPRÜ',82,391,22,'gold',z=17)
text('Nehir geçidini savun. Ormanın',82,443,21,'muted',z=17);text('derinliklerinden gelen dalgaları durdur.',82,475,20,'muted',z=17)
for y,label,val in [(540,'DALGA','12'),(590,'ZORLUK','NORMAL'),(640,'ÖNERİLEN GÜÇ','240')]:
    text(label,82,y,15,'muted',z=17);text(val,439,y-3,21,'cream',align='RIGHT',z=17)
text('İLK ZAFER ÖDÜLLERİ',82,710,14,'gold',z=17)
for x,label in [(130,'250 altın'),(285,'3 kristal')]:diamond(x,770,13,'gold',18);text(label,x+23,760,21,'cream',z=18)
button('SAVAŞA BAŞLA',82,831,358,62,True,19)
topbar('SEFER  /  BÖLÜM I');footer('SEFER HARİTASI')
s=setup('04_Settings');scenes.append(s);backdrop();topbar('AYARLAR')
text('Kendi ritminde oyna.',100,154,43,'cream','serif');text('Ses, görüntü ve oyun tercihlerini düzenle.',100,216,23,'muted')
frame(98,292,340,574,'panel','line',3)
for i,label in enumerate(['GENEL','SES','GÖRÜNTÜ','KONTROLLER','DİL']):
    y=331+i*91
    if i==1:rect(115,y-12,305,67,'panel2',5,5);rect(115,y-12,4,67,'gold',6)
    text(label,151,y+7,25,'gold' if i==1 else 'muted',z=7)
frame(472,292,1348,574,'ink','line',3)
text('SES AYARLARI',515,327,20,'gold')
for i,(label,value) in enumerate([('Ana ses',80),('Müzik',60),('Ses efektleri',90)]):
    y=415+i*113;text(label,520,y-8,28)
    rect(935,y+8,665,6,'line',5);rect(935,y+8,665*value/100,6,'gold',6);diamond(935+665*value/100,y+11,11,'bright',7)
    frame(1640,y-14,102,49,'panel','line',5);text(str(value)+'%',1691,y-1,23,'cream',align='CENTER')
line([(519,744),(1768,744)],'line',1)
text('Arka planda sesi kapat',520,783,25);rect(1643,778,98,40,'green',5,20);circle(1718,798,15,'cream',7)
button('GERİ',99,934,215,64);button('VARSAYILANA DÖN',960,934,360,64);button('DEĞİŞİKLİKLERİ KAYDET',1340,934,480,64,True)
text('ESC  Geri',100,1034,16,'muted')
# Asset sheet / separately rendered transparent reusable components.
components=[('Button_Primary',lambda:button(' ',760,500,400,80,True)),('Button_Secondary',lambda:button(' ',760,500,400,80)),('Panel_Frame',lambda:frame(660,290,600,500)),('Shield_Badge',lambda:shield(960,540,2)),('Star_Gold',lambda:star(960,540,70)),('Diamond_Currency',lambda:diamond(960,540,65)),('Progress_Track',lambda:frame(460,525,1000,30,'black','line')),('Progress_Fill',lambda:rect(460,530,1000,20,'gold',5)),('Toggle_On',lambda:(rect(910,520,100,40,'green',5,20),circle(985,540,15,'cream',7)))]
components.extend([('Toggle_Off',lambda:(rect(910,520,100,40,'line',5,20),circle(935,540,15,'muted',7))),('Slider_Knob',lambda:diamond(960,540,16,'bright')),('Node_Selected',lambda:(circle(960,540,60,'cream',6),circle(960,540,55,'gold',7),circle(960,540,49,'green',8),shield(960,540,.5,z=9))),('Icon_Settings',lambda:gear(960,540,30))])
for name,fn in components:
    s=setup('Asset_'+name);s.render.film_transparent=True;fn()
    # Crop rendering around objects, retain native pixel density.
    objs=[o for o in s.objects if o.type!='CAMERA'];coords=[]
    from mathutils import Vector
    for o in objs:
        for co in o.bound_box:coords.append(o.matrix_world@Vector(co))
    # Depsgraph update is needed for curves and text bounds.
    bpy.context.view_layer.update();coords=[o.matrix_world@Vector(co) for o in objs for co in o.bound_box]
    xmin=min(p.x for p in coords)-8;xmax=max(p.x for p in coords)+8;ymin=min(p.y for p in coords)-8;ymax=max(p.y for p in coords)+8
    s.render.use_border=True;s.render.use_crop_to_border=True;s.render.border_min_x=max(0,xmin/W);s.render.border_max_x=min(1,xmax/W);s.render.border_min_y=max(0,ymin/H);s.render.border_max_y=min(1,ymax/H)
    s.render.filepath=str(PNG/(name+'.png'))
    bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'BackpackLegions_UI_Kit.blend'))
for s in scenes:
    bpy.context.window.scene=s;s.render.filepath=str(OUT/(s.name+'.png'))
bpy.context.window.scene=scenes[2]
for screen in bpy.data.screens:
    for area in screen.areas:
        if area.type=='VIEW_3D':area.spaces.active.region_3d.view_perspective='CAMERA';area.spaces.active.shading.type='MATERIAL'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'BackpackLegions_UI_Kit.blend'))
for s in scenes:
    bpy.context.window.scene=s;bpy.ops.render.render(write_still=True)
for s in bpy.data.scenes:
    if s.name.startswith('Asset_'):
        bpy.context.window.scene=s;bpy.ops.render.render(write_still=True)
(OUT/'complete.json').write_text(json.dumps({'screens':[s.name for s in scenes],'components':[c[0] for c in components],'resolution':[W,H]},indent=2))
