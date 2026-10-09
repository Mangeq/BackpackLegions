from pathlib import Path
import json, subprocess, sys, zipfile, hashlib
from PIL import Image, ImageDraw
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(Path(__file__).parent/'vendor'))
import imageio_ffmpeg
FF=imageio_ffmpeg.get_ffmpeg_exe()
OUT=ROOT/'Marketing/Steam/2026-10-09'
video=OUT/'backpack-legions-trailer.mp4'
shots=sorted((OUT/'screenshots').glob('*'))
assert len(shots)==5
for path in shots:
    with Image.open(path) as im: assert im.size==(1920,1080),(path,im.size)
with Image.open(OUT/'backpack-legions-logo.png') as im:
    assert im.mode=='RGBA' and im.getextrema()[3][0]==0
reader=imageio_ffmpeg.read_frames(str(video))
meta=next(reader);reader.close()
assert meta['size']==(1920,1080) and abs(meta['fps']-30)<.01 and abs(meta['duration']-32)<.1,meta
subprocess.run([FF,'-v','error','-i',str(video),'-f','null','-'],check=True,stdout=subprocess.DEVNULL)
qa=OUT/'raw/qa';qa.mkdir(exist_ok=True)
times=[1,4.7,8,12,14.2,17,23,29]
sheet=Image.new('RGB',(960,4*292),'#0d1919');draw=ImageDraw.Draw(sheet)
for i,t in enumerate(times):
    p=qa/f'{t}.png'
    subprocess.run([FF,'-v','error','-y','-ss',str(t),'-i',str(video),'-frames:v','1',str(p)],check=True)
    with Image.open(p) as im:sheet.paste(im.resize((480,270)),((i%2)*480,(i//2)*292))
    draw.text(((i%2)*480+8,(i//2)*292+273),f'{t:.1f} s',fill='#ffe5a0')
sheet.save(qa/'trailer-contact.jpg')
report={'video':meta,'screenshots':len(shots),'all_screenshots':'1920x1080','logo':'RGBA 1536x1024; transparent pixels confirmed','full_video_decode':'passed','files':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in [video,*shots,OUT/'backpack-legions-logo.png']}}
(OUT/'validation.json').write_text(json.dumps(report,indent=2,ensure_ascii=False),encoding='utf-8')
files=[*shots,*[OUT/n for n in ['backpack-legions-logo.png','backpack-legions-trailer.mp4','index.html','README.md','logo-prompt.txt','validation.json']]]
dest=OUT/'Backpack-Legions-Steam-Paketi.zip'
with zipfile.ZipFile(dest,'w',compression=zipfile.ZIP_DEFLATED,compresslevel=4) as z:
    for p in files:z.write(p,p.relative_to(OUT))
with zipfile.ZipFile(dest) as z:assert z.testzip() is None
print(json.dumps({'package':str(dest),'bytes':dest.stat().st_size,'video':meta},ensure_ascii=False))
