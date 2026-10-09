"""Rebuild the trailer from unretouched Unity captures and original music."""
from pathlib import Path
import subprocess, sys, json
ROOT=Path(__file__).resolve().parents[2]
sys.path.insert(0,str(Path(__file__).parent/'vendor'))
import imageio_ffmpeg
FF=imageio_ffmpeg.get_ffmpeg_exe()
OUT=ROOT/'Marketing/Steam/2026-10-09'
RAW=OUT/'raw'
TMP=RAW/'edit'
TMP.mkdir(exist_ok=True)
FONT='C\\:/Windows/Fonts/arialbd.ttf'

def run(args):
    subprocess.run([FF,'-hide_banner','-loglevel','error','-y',*map(str,args)],check=True,cwd=OUT)

ENC=['-c:v','libx264','-preset','medium','-crf','17','-pix_fmt','yuv420p','-r','30','-an']
segments=[]
def footage(name,folder,start,frames,caption=None):
    p=TMP/(name+'.mp4')
    filt='setsar=1'
    if caption:
        (TMP/(name+'.txt')).write_text(caption,encoding='utf-8')
        filt+=f",drawbox=x=540:y=934:w=1320:h=90:color=0x101d20@0.86:t=fill,drawtext=fontfile='{FONT}':textfile='raw/edit/{name}.txt':fontsize=42:fontcolor=0xffe5a0:x=580:y=958"
    run(['-framerate','30','-start_number',start,'-i',RAW/folder/'%05d.jpg','-frames:v',frames,'-vf',filt,*ENC,p])
    segments.append(p)

def card(name,title,subtitle,duration):
    (TMP/(name+'.txt')).write_text(title,encoding='utf-8')
    (TMP/(name+'-sub.txt')).write_text(subtitle,encoding='utf-8')
    vf=f"drawbox=x=850:y=380:w=220:h=5:color=0xcda95f:t=fill,drawtext=fontfile='{FONT}':textfile='raw/edit/{name}.txt':fontsize=66:fontcolor=0xffedc7:x=(w-tw)/2:y=455,drawtext=fontfile='{FONT}':textfile='raw/edit/{name}-sub.txt':fontsize=28:fontcolor=0x9ab5af:x=(w-tw)/2:y=565,fade=t=in:d=0.12,fade=t=out:st={duration-.16}:d=0.16"
    p=TMP/(name+'.mp4')
    run(['-f','lavfi','-i','color=c=0x101d20:s=1920x1080:r=30','-t',duration,'-vf',vf,*ENC,p]);segments.append(p)

footage('01-hook','combat',1110,120,'SAVUNMANI KUR.')
card('02-pack-card','HER HÜCRE BİR KARAR.','YERLEŞTİR  /  DÖNDÜR  /  GÜÇLEN',1.5)
footage('03-pack','inventory',0,240)
card('04-element-card','ELEMENTİNİ SEÇ.','KULENİN YOLUNU BELİRLE',1.5)
footage('05-element','elements',0,150)
footage('06-battle','combat',1470,210,'LEJYONUNU SAVUN.')
p=TMP/'07-end.mp4'
(TMP/'cta.txt').write_text('YAKINDA  •  STEAM',encoding='utf-8')
filt=f"[0:v][1:v]overlay=(W-w)/2:70,drawtext=fontfile='{FONT}':textfile='raw/edit/cta.txt':fontsize=35:fontcolor=0xffe5a0:x=(w-tw)/2:y=930,fade=t=in:d=0.3,fade=t=out:st=4.5:d=0.5[v]"
run(['-f','lavfi','-i','color=c=0x101d20:s=1920x1080:r=30','-loop','1','-i',OUT/'backpack-legions-logo.png','-filter_complex',f"[1:v]scale=1230:820[logo];"+filt.replace('[1:v]overlay','[logo]overlay'),'-map','[v]','-t','5',*ENC,p]);segments.append(p)
(TMP/'concat.txt').write_text('\n'.join("file '"+p.as_posix()+"'" for p in segments),encoding='utf-8')
total=4+1.5+8+1.5+5+7+5
run(['-f','concat','-safe','0','-i',TMP/'concat.txt','-i',RAW/'original-score.wav','-map','0:v:0','-map','1:a:0','-c:v','libx264','-preset','medium','-b:v','8M','-minrate','8M','-maxrate','8M','-bufsize','16M','-x264-params','nal-hrd=cbr:force-cfr=1','-pix_fmt','yuv420p','-c:a','aac','-b:a','192k','-af',f'afade=t=out:st={total-2}:d=2','-t',total,'-movflags','+faststart',OUT/'backpack-legions-trailer.mp4'])
print(json.dumps({'duration':total,'resolution':'1920x1080','fps':30,'path':str(OUT/'backpack-legions-trailer.mp4')},ensure_ascii=False))
