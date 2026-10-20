"""Download official OFL sources and produce static faces for Unity 2022/TMP."""
import sys,json,urllib.request
from pathlib import Path
sys.path.insert(0,'C:/Users/Emir/AppData/Local/Temp/codex-font-tools-01a14114')
from fontTools.ttLib import TTFont
from fontTools.varLib.instancer import instantiateVariableFont

root=Path(__file__).resolve().parents[2]
folder=root/'Assets/Resources/UI/Fonts';folder.mkdir(parents=True,exist_ok=True)
sources=root/'ArtSource/Fonts';sources.mkdir(parents=True,exist_ok=True)
report=[]
for family,weight,style in [('Cinzel',700,'Bold'),('Alegreya',500,'Medium')]:
    base='https://raw.githubusercontent.com/google/fonts/main/ofl/'+family.lower()+'/'
    source=sources/(family+'-Variable.ttf')
    source.write_bytes(urllib.request.urlopen(base+family+'%5Bwght%5D.ttf',timeout=45).read())
    license=(sources/(family+'-OFL.txt'))
    license.write_bytes(urllib.request.urlopen(base+'OFL.txt',timeout=45).read())
    (folder/(family+'-OFL.txt')).write_bytes(license.read_bytes())
    font=instantiateVariableFont(TTFont(source),{'wght':weight},inplace=False,updateFontNames=True)
    out=folder/(family+'-'+style+'.ttf');font.save(out)
    cmap=font.getBestCmap();missing=[c for c in 'ÇĞİÖŞÜçğıöşü0123456789' if ord(c) not in cmap]
    if missing:raise RuntimeError(f'{family}: missing {missing}')
    chars=''.join(chr(c) for c in sorted(cmap) if 32<=c<=0x24f or 0x2000<=c<=0x206f or c in [0x20ac,0x20ba,0x2190,0x2191,0x2192,0x2193,0x2212,0x2264,0x2265,0x2605])
    (sources/(family+'-Characters.txt')).write_text(chars,encoding='utf-8')
    report.append({'family':family,'weight':weight,'face':font['name'].getDebugName(17) or font['name'].getDebugName(2),'file':str(out.relative_to(root)),'characters':len(chars),'turkish':not missing,'source':base+family+'%5Bwght%5D.ttf'})
validation=root/'Docs/Validation/game-fonts';validation.mkdir(parents=True,exist_ok=True)
(validation/'source-report.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(report))
