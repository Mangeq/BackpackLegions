"""Original short cloth-and-wood inventory placement sound, no recordings needed."""
import math
import random
from generate_enemy_sounds import ROOT, SR, write


def main():
    rng = random.Random(105)
    duration = .28
    samples = []
    low = smooth = phase = 0.0
    for i in range(round(SR*duration)):
        t = i/SR
        noise = rng.uniform(-1, 1)
        low += .18*(noise-low)
        smooth += .035*(noise-smooth)
        # A brief fabric slide leads into a padded placement and a tiny clasp tap.
        cloth = (low-smooth)*.48*min(1,t/.012)*math.exp(-t*24)
        u = max(0, t-.028)
        phase += (150+125*math.exp(-u*65))/SR
        impact = 0.0
        if t >= .028:
            attack = min(1,u/.002)
            impact = attack*(.58*math.sin(math.tau*phase)*math.exp(-u*37)
                              + .18*math.sin(math.tau*510*u)*math.exp(-u*70)
                              + low*.4*math.exp(-u*85))
        v = t-.072
        clasp = 0.0 if v < 0 else min(1,v/.0015)*(
            .08*math.sin(math.tau*1250*v)*math.exp(-v*115)
            + .12*low*math.exp(-v*130))
        samples.append((cloth+impact+clasp)*min(1,(duration-t)/.025))
    out = ROOT/'Assets/Audio/UI'
    out.mkdir(parents=True, exist_ok=True)
    write(out/'Backpack_Item_Place.wav',samples,peak=.56)


if __name__ == '__main__':
    main()
