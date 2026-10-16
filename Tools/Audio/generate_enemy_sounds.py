"""Deterministic original enemy SFX. Python standard library only; no source recordings."""
import math
import random
import struct
import wave
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Assets/Audio/Enemies'
SR = 44100
TAU = math.tau


def resonator(samples, hz, bandwidth):
    r = math.exp(-math.pi * bandwidth / SR)
    a = 2 * r * math.cos(TAU * hz / SR)
    b = r * r
    y1 = y2 = 0.0
    result = []
    for x in samples:
        y = (1-r) * x + a*y1 - b*y2
        result.append(y)
        y2, y1 = y1, y
    return result


def voice(duration, pitch_start, pitch_end, seed, formants, roughness=.25):
    rng = random.Random(seed)
    phase = 0.0
    source = []
    for i in range(round(duration*SR)):
        t = i/SR
        u = t/duration
        pitch = pitch_start * (pitch_end/pitch_start)**u
        pitch *= 1 + .035*math.sin(TAU*29*t) + .02*math.sin(TAU*47*t)
        phase += pitch/SR
        voiced = sum(math.sin(TAU*phase*h)/h for h in range(1, 15))
        source.append(voiced*.55 + rng.uniform(-1,1)*roughness)
    bands = [resonator(source, f, 140) for f in formants]
    result = []
    for i, x in enumerate(source):
        t = i/SR
        u = t/duration
        env = min(1, t/.018) * (1-u)**.8 * min(1, (duration-t)/.045)
        throat = .72 + .28*math.sin(TAU*37*t + 2*math.sin(TAU*7*t))
        result.append(math.tanh((sum(b[i] for b in bands)+x*.12)*1.8)*env*throat)
    return result


def magic(duration, seed, fire=False):
    rng = random.Random(seed)
    result = []
    low = phase = 0.0
    for i in range(round(duration*SR)):
        t = i/SR
        u = t/duration
        noise = rng.uniform(-1, 1)
        low += .08*(noise-low)
        phase += (760*(1-u)**1.7+170)/SR
        tone = math.sin(TAU*phase + 2.2*math.sin(TAU*phase*.502))
        env = min(1,t/.025)*(1-u)**1.8
        if fire:
            crack = noise*max(0, math.sin(TAU*83*t))**18
            x = low*2.5 + crack*.6 + math.sin(TAU*72*t)*.15
        else:
            x = (noise-low)*.14 + tone*.27 + math.sin(TAU*phase*1.501)*.12
        result.append(x*env)
    return result


def earth():
    rng = random.Random(81)
    result = []
    low = phase = 0.0
    for i in range(int(.62*SR)):
        t = i/SR
        low += .09*(rng.uniform(-1,1)-low)
        phase += (48+110*math.exp(-t*24))/SR
        x = math.sin(TAU*phase)*math.exp(-t*13) + low*1.8*math.exp(-t*8)
        for start in (.045,.09,.155):
            if t >= start:
                x += low*.8*math.exp(-(t-start)*45)
        result.append(x*min(1,t/.003)*min(1,(.62-t)/.04))
    return result


def echo(samples, delay=.065, gain=.16):
    result = samples[:] + [0.0]*int(SR*.22)
    offset = int(SR*delay)
    for i, x in enumerate(samples):
        for repeat in (1,2,3):
            if i+offset*repeat < len(result):
                result[i+offset*repeat] += x*gain**repeat
    return result


def write(path, samples, peak=.70):
    # DC removal and short edge fades prevent clicks; retain headroom for overlapping enemies.
    mean = sum(samples)/len(samples)
    samples = [x-mean for x in samples]
    fade = int(.006*SR)
    for i in range(fade):
        samples[i] *= i/fade
        samples[-1-i] *= i/fade
    scale = peak/max(abs(x) for x in samples)
    pcm = [round(x*scale*32767) for x in samples]
    with wave.open(str(path), 'wb') as f:
        f.setparams((1,2,SR,0,'NONE','not compressed'))
        f.writeframes(struct.pack('<'+'h'*len(pcm),*pcm))
    assert max(abs(x) for x in pcm) < 32767
    assert pcm[0] == pcm[-1] == 0
    print(f'{path.name}: {len(pcm)/SR:.2f}s, peak {20*math.log10(max(abs(x) for x in pcm)/32767):.1f} dBFS')
    return [x/32767 for x in pcm]


def main():
    OUT.mkdir(parents=True, exist_ok=True)
    sounds = [
        ('Goblin_Attack_01',voice(.32,180,105,11,(630,1450))),
        ('Goblin_Attack_02',voice(.38,155,120,12,(720,1650))),
        ('Goblin_Hurt_01',voice(.27,240,145,21,(850,1800),.4)),
        ('Goblin_Hurt_02',voice(.30,210,115,22,(780,1700),.4)),
        ('Goblin_Death',voice(.85,175,48,31,(530,1100),.45)),
        ('SkyWisp_Dissolve',echo(magic(.85,41))),
        ('FireRaider_Attack',magic(.48,51,fire=True)),
        ('EarthGuard_Impact',earth()),
        ('Boss_Growl',echo(voice(1.25,78,42,61,(270,620),.5))),
    ]
    preview = []
    for name, samples in sounds:
        normalized = write(OUT/(name+'.wav'), samples)
        preview.extend(normalized)
        preview.extend([0.0]*int(.45*SR))
    write(ROOT/'ArtSource/Audio/Enemy_SFX_Preview.wav',preview,peak=.70)


if __name__ == '__main__':
    main()
