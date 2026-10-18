var results=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(name,passed)=>results.Add(new{test=name,passed=passed});
var p=UnityEngine.Object.FindObjectOfType<WavePreviewUI>(true);var skin=p.GetComponent<WavePreviewSkin>();var w=UnityEngine.Object.FindObjectOfType<WaveManager>();var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
var eventData=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);p.OnPointerEnter(eventData);
check("wave indicator hover opens reference panel",p.panel.activeInHierarchy&&p.panel.GetComponent<UnityEngine.UI.Image>().sprite.texture.name=="Panel");
check("old unstructured text hidden",!p.details.enabled);
bool headers=true,names=true,identities=true,portraits=true,totals=true,types=true,normal=true,counts=true,unchanged=true;int tested=0;
foreach(var chapter in w.chapters){string before=JsonUtility.ToJson(chapter);for(int index=0;index<chapter.waves.Count;index++)
{
 var wave=chapter.waves[index];skin.Render(wave,index,chapter.waves.Count);tested++;
 var groups=wave.GetGroups().Where(g=>g!=null&&g.enemyPrefab!=null&&g.enemyCount>0).ToArray();
 var nodes=p.panel.transform.Cast<Transform>().Where(t=>t.name.StartsWith("EnemyRow")&&t.gameObject.activeSelf).ToArray();
 headers&=p.title.text=="SIRADAKİ DALGA "+(index+1)+" / "+chapter.waves.Count;
 counts&=nodes.Length==groups.Select(g=>g.enemyPrefab.GetComponent<Enemy>().enemyData).Distinct().Count();
 foreach(var row in nodes){int i=int.Parse(row.name.Replace("EnemyRow",""));var data=groups[i].enemyPrefab.GetComponent<Enemy>().enemyData;
 string identity=System.Text.RegularExpressions.Regex.Replace(row.Find("Identity").GetComponent<TMPro.TMP_Text>().text,"<[^>]*>","");
 string match=System.Text.RegularExpressions.Regex.Replace(row.Find("Matchups").GetComponent<TMPro.TMP_Text>().text,"<[^>]*>","");
 names&=row.Find("EnemyName").GetComponent<TMPro.TMP_Text>().text==data.enemyName+" × "+groups.Where(g=>g.enemyPrefab.GetComponent<Enemy>().enemyData==data).Sum(g=>g.enemyCount);
 identities&=identity.Contains(WavePreviewSkin.Number(data.health)+" can")&&identity.Contains(data.movementType==EnemyData.MovementType.Air?"Hava":"Kara")&&identity.Contains(UİManager.ElementLabel(data.element));
 types&=data.element==ElementType.Normal||(match.Contains(UİManager.ElementLabel(data.WeakTo)+" ×"+WavePreviewSkin.Number(data.weaknessMultiplier))&&match.Contains(UİManager.ElementLabel(data.ResistantTo)+" ×"+WavePreviewSkin.Number(data.resistanceMultiplier)));
 normal&=row.Find("NormalDamage").GetComponent<TMPro.TMP_Text>().text.StartsWith("Normal ×"+WavePreviewSkin.Number(data.GetDamageMultiplier(ElementType.Normal)));
 portraits&=row.Find("Portrait").GetComponent<UnityEngine.UI.Image>().sprite!=null;
 }
 totals&=p.panel.transform.Find("Summary").GetComponent<TMPro.TMP_Text>().text.StartsWith("Toplam "+wave.TotalEnemies+" düşman");
}unchanged&=JsonUtility.ToJson(chapter)==before;}
check("all "+tested+" real wave headers are dynamic",headers);check("only actual enemy types get rows",counts);check("enemy names and grouped counts match spawn data",names);check("health elements and air/ground match enemy data",identities);check("weaknesses and resistances use configured multipliers",types);check("normal damage uses actual combat multiplier",normal);check("every campaign enemy has real prefab portrait",portraits);check("totals match actual wave counts",totals);check("UI never mutates chapter or wave data",unchanged);
var one=w.chapters[0].waves[0];skin.Render(one,0,5);float small=p.panel.GetComponent<RectTransform>().rect.height;skin.Render(w.chapters[0].waves[3],3,5);check("panel grows from one to three rows",p.panel.GetComponent<RectTransform>().rect.height>small);
var first=one.GetGroups().First();var duplicate=new WaveRound();duplicate.groups.Add(new WaveEnemyGroup{enemyPrefab=first.enemyPrefab,enemyCount=2});duplicate.groups.Add(new WaveEnemyGroup{enemyPrefab=first.enemyPrefab,enemyCount=3});skin.Render(duplicate,0,1);check("repeated enemy groups combine without mutating source",p.panel.transform.Find("EnemyRow0/EnemyName").GetComponent<TMPro.TMP_Text>().text.EndsWith("× 5")&&duplicate.groups[0].enemyCount==2);
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;typeof(WaveManager).GetField("currentWaveIndex",flags).SetValue(w,3);typeof(WaveManager).GetField("waveRunning",flags).SetValue(w,true);p.Refresh();check("combat preview follows next wave rather than running wave",p.title.text=="SIRADAKİ DALGA 5 / 5");
typeof(WaveManager).GetField("currentWaveIndex",flags).SetValue(w,w.waves.Count-1);p.Refresh();check("last combat wave has no invented next enemies",p.title.text=="SON DALGA"&&p.panel.transform.Cast<Transform>().Where(t=>t.name.StartsWith("EnemyRow")).All(t=>!t.gameObject.activeSelf));
typeof(WaveManager).GetField("currentWaveIndex",flags).SetValue(w,3);typeof(WaveManager).GetField("waveRunning",flags).SetValue(w,false);p.Refresh();
skin.Render(w.chapters[0].waves[4],4,5);check("boss data adds real escape warning",p.panel.transform.Find("EnemyRow0/NormalDamage").GetComponent<TMPro.TMP_Text>().text.Contains("BOSS: kaçarsa yenilgi!"));
p.OnPointerExit(eventData);check("leaving indicator closes preview",!p.panel.activeSelf);p.OnPointerEnter(eventData);typeof(GameManager).GetProperty("IsPaused").SetValue(gm,true,null);p.SendMessage("Update");check("pause hides preview",!p.panel.activeSelf);typeof(GameManager).GetProperty("IsPaused").SetValue(gm,false,null);p.OnPointerEnter(eventData);
p.Refresh();UİManager.UpdateWaveText(4,w.waves.Count);
return results;
