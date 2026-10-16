if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
UnityEditor.AssetDatabase.Refresh();
var ui=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SkillTreeUI.prefab").GetComponent<SkillTreePanelUI>();
System.Action<string> folder=null;folder=p=>{if(UnityEditor.AssetDatabase.IsValidFolder(p))return;int slash=p.LastIndexOf('/');folder(p.Substring(0,slash));UnityEditor.AssetDatabase.CreateFolder(p.Substring(0,slash),p.Substring(slash+1));};
folder("Assets/Resources/Data/Skills");folder("Assets/Resources/Data/SkillTrees");
string[] dirs={"Archer","Magic","Mortar","Unit"};TowerType[] types={TowerType.ArrowTower,TowerType.MagicTower,TowerType.CannonTower,TowerType.UnitTower};
for(int b=0;b<4;b++){
 folder("Assets/Resources/Data/Skills/"+dirs[b]);
 string tp="Assets/Resources/Data/SkillTrees/"+dirs[b]+".asset";
 if(UnityEditor.AssetDatabase.LoadAssetAtPath<SkillTreeData>(tp)!=null)throw new System.Exception("Migration already exists: "+tp);
 var tree=ScriptableObject.CreateInstance<SkillTreeData>();tree.towerType=types[b];tree.title=ui.branches[b].title;tree.subtitle=ui.branches[b].subtitle;tree.accent=ui.branches[b].accent;
 int count=b==0?11:12;var skills=new SkillData[count];
 for(int n=0;n<count;n++){
  var skill=ScriptableObject.CreateInstance<SkillData>();skills[n]=skill;skill.towerType=types[b];skill.icon=ui.branches[b].icons[n];skill.available=b==0;
  if(b==0){var old=SkillTreeProgress.ArcherSkills[n];skill.id=old.id;skill.displayName=old.name;skill.description=old.description;skill.pointCost=old.cost;}
  else{skill.id=dirs[b].ToLowerInvariant()+".draft."+n;skill.displayName=ui.branches[b].names[n];skill.description=ui.branches[b].descriptions[n];skill.pointCost=n==0?1:2;}
  var pos=b==0?ui.archerPositions[n]:ui.previewPositions[n];tree.nodes.Add(new SkillTreeData.Node{skill=skill,position=new Vector2(pos.x+73-300,-pos.y+40-280)});
  UnityEditor.AssetDatabase.CreateAsset(skill,"Assets/Resources/Data/Skills/"+dirs[b]+"/"+skill.id+".asset");
 }
 int[] draftParents={-1,0,0,0,1,2,3,4,4,5,6,6};
 for(int n=0;n<count;n++){
  int parent=b==0?SkillTreeProgress.ArcherSkills[n].parent:draftParents[n];if(parent>=0)skills[n].prerequisites.Add(skills[parent]);
  if(b==0){var effect=new SkillData.Effect();
   switch(n){case 0:case 6:effect.type=SkillData.EffectType.DamagePercent;effect.value=5;break;case 1:case 4:effect.type=SkillData.EffectType.AttackSpeedPercent;effect.value=5;break;case 2:case 5:effect.type=SkillData.EffectType.RangePercent;effect.value=5;break;case 3:effect.type=SkillData.EffectType.CriticalChancePoints;effect.value=3;break;case 7:effect.type=SkillData.EffectType.SameTargetHitSpeed;effect.value=2;effect.count=5;break;case 8:effect.type=SkillData.EffectType.OuterRangeDamage;effect.value=10;effect.threshold=50;break;case 9:effect.type=SkillData.EffectType.LowHealthDamage;effect.value=10;effect.threshold=30;break;case 10:effect.type=SkillData.EffectType.EveryNthAttackCritical;effect.value=0;effect.count=6;break;}
   skills[n].effects.Add(effect);
  }
  UnityEditor.EditorUtility.SetDirty(skills[n]);
 }
 UnityEditor.AssetDatabase.CreateAsset(tree,tp);
}
UnityEditor.AssetDatabase.SaveAssets();return "47 SkillData assets and four trees created; original archer IDs preserved.";
