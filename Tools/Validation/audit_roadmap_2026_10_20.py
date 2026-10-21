from pathlib import Path
import re,json,collections
root=Path.cwd(); data=root/'Assets/Resources/Data'
def field(s,k):
 m=re.search(r'^  '+re.escape(k)+r': (.*)$',s,re.M);return m.group(1) if m else None
out={'date':'2026-10-20','method':'Static source and serialized asset inspection; no Unity runtime or build execution'}
out['counts']={p:len(list((data/p).rglob('*.asset'))) for p in ['Items/Shop','Items/Main','Items/Evolved','Items/Elements','EvolutionRecipes','SynergyRecipes','Skills','SkillTrees','EnemyData/Campaign','Chapters']}
out['skills']={}
for folder in (data/'Skills').iterdir():
 if folder.is_dir():
  items=[p.read_text(encoding='utf-8-sig') for p in folder.glob('*.asset')]
  out['skills'][folder.name]={'total':len(items),'available':sum(field(s,'available')=='1' for s in items)}
out['chapters']=[]
for p in sorted((data/'Chapters').glob('*.asset')):
 s=p.read_text(encoding='utf-8-sig');out['chapters'].append({'asset':p.name,**{k:field(s,k) for k in ['sceneName','startingGold','preparationSeconds','extraPreparationPerWave']},'waves':len(re.findall(r'^  - groups:',s,re.M)),'enemies':sum(map(int,re.findall(r'^      enemyCount: (\d+)',s,re.M)))})
out['towerData']=[]
for p in data.rglob('*.asset'):
 s=p.read_text(encoding='utf-8-sig')
 if field(s,'defaultWeapon') is not None:out['towerData'].append({'asset':str(p.relative_to(root)),**{k:field(s,k) for k in ['cost','damage','attackSpeed','range','elementChangeCost']}})
out['bossData']=[]
for p in (data/'EnemyData/Campaign').rglob('Warlord.asset'):
 s=p.read_text(encoding='utf-8-sig');out['bossData'].append({'asset':str(p.relative_to(data)),**{k:field(s,k) for k in ['health','isBoss','weaknessMultiplier','resistanceMultiplier','normalAttackMultiplier','baseDamageOverride']}})
# Check project-local script GUIDs only; package scripts can be valid without Assets .meta.
metas={}
for p in (root/'Assets').rglob('*.cs.meta'):
 m=re.search(r'^guid: (\w+)',p.read_text(encoding='utf-8-sig'),re.M)
 if m:metas[m.group(1)]=str(p.relative_to(root))[:-5]
out['sceneScriptUsage']={}
for p in (root/'Assets/Scenes').glob('*.unity'):
 refs=re.findall(r'm_Script: \{fileID: \d+, guid: (\w+)',p.read_text(encoding='utf-8-sig'))
 out['sceneScriptUsage'][p.name]=sorted(set(metas[g] for g in refs if g in metas))
path=root/'Docs/Validation/roadmap-audit-2026-10-20.json';path.write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in out.items() if k!='sceneScriptUsage'},ensure_ascii=False,indent=2))
