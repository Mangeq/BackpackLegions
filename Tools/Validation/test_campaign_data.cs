var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(n,ok)=>rows.Add(new{test=n,passed=ok});
var chapters=UnityEditor.AssetDatabase.FindAssets("t:ChapterData",new[]{"Assets/Resources/Data/Chapters"}).Select(g=>UnityEditor.AssetDatabase.LoadAssetAtPath<ChapterData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).OrderBy(c=>c.chapterNumber).ToArray();
check("Five chapters",chapters.Length==5);
for(int i=0;i<chapters.Length;i++){
 var c=chapters[i];check("Chapter "+(i+1)+" linear rounds/enemies",c.waves.Count==5+2*i&&c.TotalEnemies==40+20*i);
 check("Chapter "+(i+1)+" preparation progression",c.PreparationFor(0)==60+15*i&&c.PreparationFor(c.waves.Count-1)==60+15*i+5*(c.waves.Count-1));
 check("Chapter "+(i+1)+" scene/theme",c.sceneName=="GameScane"&&c.themeIndex==0);
 check("Chapter "+(i+1)+" wave two mixed",c.waves[1].groups.Count==2);
 check("Chapter "+(i+1)+" wave three air",c.waves[2].groups.All(g=>g.enemyPrefab.GetComponent<Enemy>().IsAir));
 check("Chapter "+(i+1)+" three simultaneous groups",c.waves[3].groups.Count==3&&c.waves[3].groups.All(g=>g.startDelay==0));
 check("Chapter "+(i+1)+" single boss in final wave",c.waves.Last().groups[0].enemyCount==1&&c.waves.Last().groups[0].enemyPrefab.name.Contains("Warlord"));
 foreach(var w in c.waves){check("Valid groups C"+(i+1),w.groups.Count>=1&&w.groups.Count<=3&&w.groups.All(g=>g.enemyCount>0&&g.enemyPrefab!=null&&g.enemyPrefab.GetComponent<Enemy>().enemyData.element!=ElementType.Normal));var description=WavePreviewUI.Describe(w);check("Preview roster C"+(i+1),w.groups.All(g=>description.Contains(g.enemyPrefab.GetComponent<Enemy>().enemyData.enemyName+" × "+g.enemyCount))&&description.Contains("Toplam "+w.TotalEnemies));}
 foreach(var role in new[]{"Scout","Raider","Guard","Elite","SkyWisp","Warlord"}){var baseline=UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Resources/Data/EnemyData/Chapter01/"+role+".asset");var actual=UnityEditor.AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Resources/Data/EnemyData/Campaign/Chapter"+(i+1).ToString("00")+"/"+role+".asset");check("Linear stats "+i+" "+role,actual.health==Mathf.Round(baseline.health*(1+.25f*i))&&actual.damage==Mathf.RoundToInt(baseline.damage*(1+.15f*i)));}
}
ElementType[] cycle={ElementType.Water,ElementType.Fire,ElementType.Air,ElementType.Earth};
for(int defender=0;defender<4;defender++)for(int attacker=0;attacker<4;attacker++){
 var d=UnityEngine.ScriptableObject.CreateInstance<EnemyData>();d.element=cycle[defender];d.health=1000;
 var go=new GameObject("ElementDamageTest");go.SetActive(false);var enemy=go.AddComponent<Enemy>();enemy.enemyData=d;
 typeof(Enemy).GetField("currenthealth",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(enemy,1000f);
 float expected=(attacker+1)%4==defender?1.5f:(defender+1)%4==attacker?.5f:1;
 enemy.TakeDamage(100,cycle[attacker]);check("Actual damage "+cycle[attacker]+" -> "+cycle[defender],Mathf.Approximately(enemy.CurrentHealth,1000-100*expected));
 UnityEngine.Object.DestroyImmediate(go);UnityEngine.Object.DestroyImmediate(d);
}
return rows;
