import json,time,pathlib
import run_stage1 as m
root=pathlib.Path(__file__).resolve().parents[2]
m.rpc('initialize',{'protocolVersion':'2024-11-05','capabilities':{},'clientInfo':{'name':'SkillAuthoringChecks','version':'1'}})
m.tool('set_active_instance',instance='BackPackDefance@64cdf686e603a5ef')
backup=m.code('return new {exists=PlayerPrefs.HasKey(SkillTreeProgress.SaveKey),value=PlayerPrefs.GetString(SkillTreeProgress.SaveKey,"")};')
report={}
try:
 report['inspector']=m.code('''var go=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/SkillTreeUI.prefab");var editor=UnityEditor.Editor.CreateEditor(go.GetComponent<SkillTreePanelUI>());var visual=editor.CreateInspectorGUI();if(visual==null)throw new System.Exception("Missing authoring inspector");int count=visual.childCount;UnityEngine.Object.DestroyImmediate(editor);return new {inspectorSections=count};''')
 m.code('''string[] names={"Magic","Mortar","Unit"};TowerType[] types={TowerType.MagicTower,TowerType.CannonTower,TowerType.UnitTower};
for(int i=0;i<3;i++){var path="Assets/Resources/Data/Skills/Validation"+names[i]+".asset";if(UnityEditor.AssetDatabase.LoadAssetAtPath<SkillData>(path)!=null)throw new System.Exception("Test path exists");var skill=ScriptableObject.CreateInstance<SkillData>();skill.id="validation."+names[i];skill.displayName="Validation "+names[i];skill.towerType=types[i];skill.pointCost=2;skill.effects.Add(new SkillData.Effect{type=SkillData.EffectType.DamagePercent,value=7});skill.effects.Add(new SkillData.Effect{type=SkillData.EffectType.AttackSpeedPercent,value=9});skill.effects.Add(new SkillData.Effect{type=SkillData.EffectType.CriticalChancePoints,value=4});skill.effects.Add(new SkillData.Effect{type=i==2?SkillData.EffectType.UnitHealthPercent:SkillData.EffectType.RangePercent,value=i==2?13:8});UnityEditor.AssetDatabase.CreateAsset(skill,path);}UnityEditor.AssetDatabase.SaveAssets();PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");return true;''')
 m.tool('manage_editor',action='play');time.sleep(2);m.wait_ready()
 report['genericEffects']=m.code('''var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;System.Func<Tower,string,float> stat=(t,n)=>(float)typeof(Tower).GetField(n,flags).GetValue(t);var p=new SkillTreeProgress();for(int c=1;c<=3;c++)p.RecordResult(c,true,100,100);
string[] names={"Magic","Mortar","Unit"};string[] prefabs={"MegicTower1","MortarTower","UnitTower1"};
for(int i=0;i<3;i++){
 var template=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+prefabs[i]+".prefab");GameObject go;Tower tower;Unit unit=null;
 if(i==2){go=new GameObject("ValidationUnitTower");tower=go.AddComponent<UnitTower>();tower.data=template.GetComponent<Tower>().data;var u=new GameObject("ValidationUnit");unit=u.AddComponent<Unit>();((UnitTower)tower).ActiveUnits.Add(u);}else{go=UnityEngine.Object.Instantiate(template);tower=go.GetComponent<Tower>();}
 typeof(Tower).GetMethod("InitializeTower",flags).Invoke(tower,null);float damage=stat(tower,"currentDamage"),cooldown=stat(tower,"currentAttackSpeed"),crit=stat(tower,"currentCriticalChance"),range=tower.CurrentRange;int hp=unit!=null?unit.MaxHealth:0;
 var skill=Resources.Load<SkillData>("Data/Skills/Validation"+names[i]);var tree=ScriptableObject.CreateInstance<SkillTreeData>();tree.towerType=tower.data.type;tree.nodes.Add(new SkillTreeData.Node{skill=skill});
 if(!p.TryUnlock(tree,skill))throw new System.Exception("Unlock "+names[i]);
 if(Mathf.Abs(stat(tower,"currentDamage")-damage*1.07f)>.001f||Mathf.Abs(stat(tower,"currentAttackSpeed")-cooldown/1.09f)>.001f||Mathf.Abs(stat(tower,"currentCriticalChance")-Mathf.Min(tower.data.maximumCriticalChance,crit+.04f))>.001f)throw new System.Exception("Generic stats "+names[i]);
 if(unit!=null){if(unit.MaxHealth!=Mathf.RoundToInt(hp*1.13f))throw new System.Exception("Unit health");}else if(Mathf.Abs(tower.CurrentRange-range*1.08f)>.001f)throw new System.Exception("Range");
 if(p.ResetTree(tower.data.type)!=2||Mathf.Abs(stat(tower,"currentDamage")-damage)>.001f)throw new System.Exception("Reset "+names[i]);
 if(unit!=null)UnityEngine.Object.DestroyImmediate(unit.gameObject);UnityEngine.Object.DestroyImmediate(go);UnityEngine.Object.DestroyImmediate(tree);
}return "User-created data applied 7% damage, 9% speed, +4 crit points, 8% range / 13% unit health to correct types; refund reverted effects";''')
 report['console']=m.tool('read_console',action='get',types=['error','warning'],count=10,include_stacktrace=False)
finally:
 m.tool('manage_editor',action='stop');time.sleep(.3)
 if backup['exists']:m.code('PlayerPrefs.SetString(SkillTreeProgress.SaveKey,'+json.dumps(backup['value'])+');PlayerPrefs.Save();return true;')
 else:m.code('PlayerPrefs.DeleteKey(SkillTreeProgress.SaveKey);PlayerPrefs.Save();return true;')
 m.tool("execute_code",action="execute",safety_checks=False,code='foreach(var name in new[]{"Magic","Mortar","Unit"})UnityEditor.AssetDatabase.DeleteAsset("Assets/Resources/Data/Skills/Validation"+name+".asset");return true;')
 m.code((root/'Tools/Validation/campaign_ready.cs').read_text(encoding='utf-8'))
 (root/'Docs/Validation/skill-data-authoring-2026-10-15.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False))
