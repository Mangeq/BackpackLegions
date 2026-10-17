"""Cut the approved raster into game UI layers. No generated/repainted artwork.

Run with the bundled Python runtime (Pillow + numpy). Coordinates use top-left pixels.
Only areas occupied by changing game data are cleared using empty pixels from the
same reference. The original and a pixel-exact reconstruction remain available.
"""
from pathlib import Path
import argparse
import json
from PIL import Image, ImageDraw
import numpy as np

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Assets/Resources/UI/TowerPanel/Reference'

def extract(source):
    OUT.mkdir(parents=True, exist_ok=True)
    im = Image.open(source).convert('RGBA')
    assert im.size == (1678, 937), im.size
    im.save(OUT / 'Original.png')
    manifest = {'sourceSize': list(im.size), 'parts': {}}
    def crop(name, rect):
        piece = im.crop(rect)
        piece.save(OUT / (name + '.png'))
        manifest['parts'][name] = list(rect)
        return piece

    # These remain exact, unaltered crops, including the lettering and highlights.
    rects = {
        'Header': (0, 0, 1678, 320),
        'Inventory': (0, 320, 636, 937),
        'Storage': (636, 320, 957, 937),
        'Shop': (957, 320, 1678, 937),
        'Fire': (914, 129, 1080, 183),
        'Water': (1087, 129, 1247, 183),
        'Earth': (1253, 129, 1414, 183),
        'Air': (1419, 129, 1598, 183),
        'Both': (1044, 188, 1228, 235),
        'Ground': (1230, 188, 1397, 235),
        'Flying': (1401, 188, 1584, 235),
        'First': (1043, 236, 1309, 285),
        'Last': (1314, 236, 1586, 285),
        'Reroll': (1054, 804, 1538, 883),
        'DamageIcon': (86, 84, 140, 131),
        'IntervalIcon': (88, 131, 140, 179),
        'CriticalIcon': (88, 178, 140, 224),
        'RangeIcon': (86, 224, 141, 273),
        'ProgressFill': (922, 77, 1492, 91),
        'FootprintCell': (1106, 507, 1125, 524),
        'Cell': (373, 775, 477, 867),
        'TitleReference': (81, 30, 566, 83),
        'DamageReference': (341, 85, 441, 125),
        'IntervalReference': (341, 133, 462, 171),
        'CriticalReference': (341, 180, 438, 219),
        'RangeReference': (341, 225, 462, 264),
    }
    for name, rect in rects.items(): crop(name, rect)

    clean = im.copy()
    def clear_from_strip(rect, strip):
        # Copy existing empty texture; never synthesize a replacement theme.
        x0, y0, x1, y1 = rect
        tile = im.crop(strip)
        clean.paste(tile.resize((x1-x0, y1-y0), Image.Resampling.BILINEAR), (x0,y0))

    clear_from_strip((80, 30, 800, 83), (805, 30, 875, 83))
    for y0,y1 in [(86,127),(133,173),(180,219),(225,269)]:
        clear_from_strip((337,y0,800,y1),(807,y0,875,y1))
    clear_from_strip((910,36,1588,68),(1595,36,1623,68))
    clear_from_strip((911,97,1607,124),(1610,97,1628,124))
    clear_from_strip((922,77,1492,91),(1532,77,1583,91))
    clear_from_strip((1145,370,1518,398),(1540,370,1608,398))

    # Rebuild the occupied grid solely from an empty cell in this same screenshot.
    # Keep all original surrounding stone, headings, bars and corner bolts.
    cell = im.crop(rects['Cell'])
    xs = [66,170,271,373,477,581]
    ys = [414,505,595,686,777,869]
    for row in range(5):
        for col in range(5):
            if (row==3 and col>=3) or (row==4 and col>=2):
                continue  # Already empty in the reference: preserve these pixels exactly.
            clean.paste(cell.resize((xs[col+1]-xs[col],ys[row+1]-ys[row]),Image.Resampling.BILINEAR),(xs[col],ys[row]))
    for x0,y0,x1,y1 in [(978,423,1281,589),(1317,423,1618,589),
                         (978,622,1281,787),(1317,622,1618,787)]:
        clear_from_strip((x0,y0,x1,y1),(x1-12,y0,x1,y1))
    clean.save(OUT/'Background.png')

    # Direct cutouts of the four illustrated shop items. Polygon masks only remove
    # the photographed panel behind them; all retained RGB pixels are untouched.
    icons = {
      'PiercingArrows': [(981,531),(992,509),(1029,464),(1061,442),(1048,481),(1081,457),(1061,507),(1028,544),(1006,555),(982,549)],
      'ScoutHood': [(982,709),(990,681),(1011,646),(1027,630),(1044,639),(1063,662),(1073,693),(1090,714),(1098,738),(1082,755),(1049,759),(1023,749),(1000,756),(978,737)],
      'LeatherGloves': [(1326,519),(1344,483),(1357,455),(1374,449),(1387,462),(1401,458),(1412,467),(1415,480),(1432,476),(1446,487),(1440,507),(1421,544),(1416,566),(1401,573),(1384,565),(1367,570),(1351,556),(1332,543)],
      'RangerBoots': [(1328,744),(1344,726),(1341,695),(1330,659),(1340,648),(1364,651),(1375,641),(1405,636),(1428,645),(1423,670),(1410,697),(1410,734),(1422,750),(1407,766),(1375,769),(1353,756),(1330,756)],
    }
    for name, polygon in icons.items():
        x0=min(p[0] for p in polygon)-2;y0=min(p[1] for p in polygon)-2
        x1=max(p[0] for p in polygon)+3;y1=max(p[1] for p in polygon)+3
        piece=im.crop((x0,y0,x1,y1));mask=Image.new('L',piece.size)
        ImageDraw.Draw(mask).polygon([(x-x0,y-y0) for x,y in polygon],fill=255)
        if name in ('PiercingArrows','ScoutHood'):
            pixels=np.asarray(piece)
            rgb=pixels[:,:,:3].astype(float)
            background=(rgb[:,:,0]>rgb[:,:,1]*1.16)&(rgb[:,:,1]>rgb[:,:,2]*1.12)
            alpha=np.asarray(mask).copy();alpha[background]=0
            mask=Image.fromarray(alpha)
        piece.putalpha(mask);piece.save(OUT/(name+'.png'))
        manifest['parts'][name]=[x0,y0,x1,y1]

    # Reconstruct the unedited source from its four non-overlapping layer crops.
    reconstruction=Image.new('RGBA',im.size)
    for name in ['Header','Inventory','Storage','Shop']:
        rect=rects[name];reconstruction.paste(Image.open(OUT/(name+'.png')),rect[:2])
    assert np.array_equal(np.asarray(im),np.asarray(reconstruction)), 'Crop reconstruction differs'
    manifest['unalteredCropReconstructionPixelDifference']=0
    (OUT/'parts.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
    proof=ROOT/'Docs/Validation/tower-panel-reference'
    proof.mkdir(parents=True,exist_ok=True)
    reconstruction.save(proof/'reference-reassembled.png')
    print(f'Extracted {len(manifest["parts"])} exact source crops; reconstruction pixel difference: 0')

if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('source');extract(parser.parse_args().source)
