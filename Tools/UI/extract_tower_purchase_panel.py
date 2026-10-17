"""Extract the approved tower purchase skin without generating new artwork."""
from pathlib import Path
import argparse,json
from PIL import Image,ImageDraw,ImageFilter
import numpy as np

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'Assets/Resources/UI/TowerPurchase/Reference'

def extract(source):
    im=Image.open(source).convert('RGBA')
    assert im.size==(1024,572),im.size
    OUT.mkdir(parents=True,exist_ok=True)
    archive=ROOT/'ArtSource/TowerPurchaseReference';archive.mkdir(parents=True,exist_ok=True)
    im.save(archive/'Original.png')
    rects={
      'Card':(722,117,972,498),'Buy':(735,407,961,455),
      'Close':(934,125,960,153),'Rail':(973,177,1018,552),
      'MagicTab':(973,177,1018,235),'MortarTab':(973,235,1018,292),
      'ArcherTab':(973,292,1018,348),'UnitTab':(973,348,1018,406),
      'UnitPortrait':(756,181,820,272),
    }
    for name,rect in rects.items():im.crop(rect).save(OUT/(name+'.png'))
    # Only baked content is cleared; all visible framing/parchment comes from the
    # approved image. Empty texture is sampled from the same material and height.
    card=im.copy()
    def restore(rect,kind):
        x,y,r,b=rect
        box=(x-2,y-2,r+2,b+2)
        pixels=np.asarray(card.crop(box)).copy()
        rgb=pixels[:,:,:3].astype(float)
        if kind=='portrait':mask=np.ones((b-y,r-x),dtype=bool)
        else:
            center=rgb[2:-2,2:-2]
            mask=((center[:,:,0]>145)&(center[:,:,1]>105)) if kind=='light' else ((center[:,:,0]<145)&(center[:,:,1]<130))
            mask=np.asarray(Image.fromarray((mask*255).astype('uint8')).filter(ImageFilter.MaxFilter(7)))>0
        mask=np.pad(mask,2)
        known=~mask
        for _ in range((r-x)+(b-y)):
            sums=np.zeros_like(rgb);counts=np.zeros(known.shape)
            for axis,step in [(0,1),(0,-1),(1,1),(1,-1)]:
                neighbor=np.roll(known,step,axis)
                sums+=np.roll(rgb,step,axis)*neighbor[:,:,None];counts+=neighbor
            frontier=(~known)&(counts>0)
            if not frontier.any():break
            rgb[frontier]=sums[frontier]/counts[frontier,None];known[frontier]=True
        for _ in range(100 if kind=='portrait' else 12):
            average=sum(np.roll(rgb,step,axis) for axis,step in [(0,1),(0,-1),(1,1),(1,-1)])/4
            rgb[mask]=average[mask]
        pixels[:,:,:3]=np.clip(rgb,0,255).astype('uint8')
        card.paste(Image.fromarray(pixels),box[:2])
    restore((736,127,932,152),'light')
    restore((752,179,822,275),'portrait')
    restore((843,173,953,237),'dark')
    restore((742,295,953,383),'dark')
    restore((774,420,937,443),'light')
    card=card.crop(rects['Card'])
    shape=[(5,1),(244,1),(248,6),(248,347),(232,354),(211,364),
           (197,369),(174,374),(167,379),(148,374),(135,378),(116,372),
           (96,370),(80,366),(58,363),(30,356),(2,350),(1,8)]
    mask=Image.new('L',card.size);ImageDraw.Draw(mask).polygon(shape,fill=255)
    card.putalpha(mask);card.save(OUT/'Card.png')
    card.crop((13,290,239,338)).save(OUT/'Buy.png')

    # Original selected-tab rim, with its center cut out for all four tower types.
    selected=im.crop(rects['UnitTab']);mask=Image.new('L',selected.size,255)
    ImageDraw.Draw(mask).rectangle((5,5,39,52),fill=0)
    selected.putalpha(mask);selected.save(OUT/'SelectedRim.png')
    # Exact unit illustration, with only the surrounding parchment removed.
    portrait=im.crop(rects['UnitPortrait']);mask=Image.new('L',portrait.size)
    poly=[(12,87),(5,78),(8,60),(7,47),(10,33),(20,27),(23,14),(28,2),
          (34,5),(40,25),(48,34),(53,44),(51,60),(55,75),(48,86),(30,89)]
    ImageDraw.Draw(mask).polygon(poly,fill=255)
    portrait.putalpha(mask);portrait.save(OUT/'UnitPortrait.png')
    # Other tower portraits retain the game's original model renders. Trim only
    # their transparent padding so they occupy the same portrait area as the unit.
    for name,filename in [('MagicPortrait','Magic.png'),('MortarPortrait','Cannon.png'),('ArcherPortrait','Archer.png')]:
        portrait=Image.open(ROOT/'Assets/Prefabs/UI/PurchaseArt'/filename).convert('RGBA')
        bounds=portrait.getchannel('A').getbbox()
        portrait.crop(bounds if bounds else (0,0,*portrait.size)).save(OUT/(name+'.png'))
    (archive/'parts.json').write_text(json.dumps({'sourceSize':im.size,'rectangles':rects},indent=2),encoding='utf-8')
    print('Extracted approved card, parchment, purchase button, four tabs and unit illustration.')

if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('source');extract(p.parse_args().source)
