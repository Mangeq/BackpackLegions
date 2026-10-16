if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first.");
const string backup="Docs/Validation/before-soften-2026-10-14.json";
if(System.IO.File.Exists(backup))throw new System.Exception("Already applied; inspect before repeating.");
var chapters=Resources.LoadAll<ChapterData>("Data/Chapters");
var enemies=Resources.LoadAll<EnemyData>("Data/EnemyData/Campaign");
if(chapters.Length!=5||enemies.Length!=30)throw new System.Exception("Unexpected catalogue.");
var assets=chapters.Cast<UnityEngine.Object>().Concat(enemies).ToArray();
System.IO.File.WriteAllText(backup,"["+string.Join(",\n",assets.Select(a=>"{\"path\":\""+UnityEditor.AssetDatabase.GetAssetPath(a)+"\",\"data\":"+UnityEditor.EditorJsonUtility.ToJson(a)+"}"))+ "]");
foreach(var chapter in chapters)
{
    chapter.startingGold+=150;
    foreach(var wave in chapter.waves)foreach(var group in wave.groups)group.spawnInterval*=1.1f;
    UnityEditor.EditorUtility.SetDirty(chapter);
}
foreach(var enemy in enemies)
{
    enemy.health=Mathf.Round(enemy.health*(enemy.isBoss?.8f:.88f));
    enemy.goldReward=Mathf.CeilToInt(enemy.goldReward*1.1f);
    enemy.resistanceMultiplier=.4f;
    enemy.normalAttackMultiplier=enemy.isBoss?.6f:enemy.normalAttackMultiplier>=.9f?.95f:.85f;
    UnityEditor.EditorUtility.SetDirty(enemy);
}
UnityEditor.AssetDatabase.SaveAssets();
return chapters.OrderBy(c=>c.chapterNumber).Select(c=>new{c.chapterNumber,c.startingGold,enemies=c.TotalEnemies,boss=c.waves.Last().groups.First(g=>g.enemyPrefab.GetComponent<Enemy>().enemyData.isBoss).enemyPrefab.GetComponent<Enemy>().enemyData.health}).ToArray();
