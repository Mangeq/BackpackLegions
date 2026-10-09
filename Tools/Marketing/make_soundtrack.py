"""Original synthesized instrumental bed; no external samples or recordings."""
from pathlib import Path
import numpy as np
import wave

OUT=Path(__file__).resolve().parents[2]/'Marketing/Steam/2026-10-09/raw'
SR=48000
DURATION=40
y=np.zeros((SR*DURATION,2),np.float64)
rng=np.random.default_rng(1909)
beat=60/112

def note(start,duration,midi,volume,kind='pluck',pan=0):
    n=int(duration*SR); t=np.arange(n)/SR
    f=440*2**((midi-69)/12)
    if kind=='pad':
        a=np.sin(2*np.pi*f*t)+.3*np.sin(2*np.pi*f*2.002*t)+.12*np.sin(2*np.pi*f*3*t)
        env=np.minimum(t/.18,1)*np.minimum((duration-t)/.35,1)*.55
    else:
        a=np.sin(2*np.pi*f*t)+.3*np.sin(2*np.pi*f*2*t)+.09*np.sin(2*np.pi*f*4*t)
        env=(1-np.exp(-t*160))*np.exp(-t*(4 if kind=='pluck' else 2))
    a*=env*volume
    i=int(start*SR); n=min(n,len(y)-i)
    if n<=0:return
    y[i:i+n,0]+=a[:n]*np.sqrt((1-pan)/2)
    y[i:i+n,1]+=a[:n]*np.sqrt((1+pan)/2)

# D minor, B-flat, F, C: a restrained fantasy-strategy pulse.
chords=[(50,57,62,65),(46,53,58,62),(48,53,57,60),(48,55,60,64)]
for bar in range(19):
    start=bar*4*beat
    if start>=DURATION:break
    ch=chords[bar%4]
    for k,m in enumerate(ch):note(start,4*beat+.3,m,.033,'pad',(k-1.5)*.35)
    for j in range(8):note(start+j*beat/2,1.05,ch[[0,2,1,3,2,1,3,2][j]]+12,.065,'pluck',(-1)**j*.3)
    for j in (0,2):
        at=start+j*beat; note(at,.8,ch[0]-12,.12,'bass')
        n=int(.18*SR);t=np.arange(n)/SR
        drum=np.sin(2*np.pi*(46*t+8*(1-np.exp(-t*28))))*np.exp(-t*24)*.13
        ix=int(at*SR); n=min(n,len(y)-ix)
        if n>0:y[ix:ix+n]+=drum[:n,None]
    for j in (1,3):
        ix=int((start+j*beat)*SR);n=min(int(.12*SR),len(y)-ix)
        if n>0:
            t=np.arange(n)/SR;noise=rng.normal(0,1,n)
            hit=np.diff(noise,prepend=0)*np.exp(-t*40)*.021
            y[ix:ix+n]+=hit[:,None]
# Quiet stereo echoes and a gentle start/end, peak-limited without clipping.
delay=int(.27*SR)
y[delay:]+=y[:-delay,::-1]*.15
y*=np.minimum(np.arange(len(y))/SR/.7,1)[:,None]
y*=np.minimum((len(y)-np.arange(len(y)))/SR/2,1)[:,None]
y=np.tanh(y*1.4)
OUT.mkdir(parents=True,exist_ok=True)
with wave.open(str(OUT/'original-score.wav'),'wb') as w:
    w.setnchannels(2);w.setsampwidth(2);w.setframerate(SR)
    w.writeframes((y*32767).astype('<i2').tobytes())
print(OUT/'original-score.wav')
