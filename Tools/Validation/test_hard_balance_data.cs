var results=new System.Collections.Generic.List<object>();
System.Action<string,bool> check=(name,passed)=>results.Add(new{test=name,passed});
var chapters=Resources.LoadAll<ChapterData>("Data/Chapters").OrderBy(c=>c.chapterNumber).ToArray();
for(int c=0;c<chapters.Length;c++)
{
    var chapter=chapters[c];check("Chapter "+(c+1)+" budget",chapter.startingGold==700+150*c);
    check("Chapter "+(c+1)+" linear population",chapter.TotalEnemies==180+80*c);
    check("Chapter "+(c+1)+" round count",chapter.waves.Count==5+2*c);
    check("Chapter "+(c+1)+" second wave mixed",chapter.waves[1].groups.Count==2);
    check("Chapter "+(c+1)+" third wave air",chapter.waves[2].groups.All(g=>g.enemyPrefab.GetComponent<Enemy>().IsAir));
    check("Chapter "+(c+1)+" one final boss",chapter.waves.Last().groups.Where(g=>g.enemyPrefab.GetComponent<Enemy>().enemyData.isBoss).Sum(g=>g.enemyCount)==1);
    check("Chapter "+(c+1)+" no early boss",chapter.waves.Take(chapter.waves.Count-1).All(w=>w.groups.All(g=>!g.enemyPrefab.GetComponent<Enemy>().enemyData.isBoss)));
    foreach(var w in chapter.waves)check("Valid populated groups chapter "+(c+1),w.groups.All(g=>g.enemyCount>0&&g.spawnInterval>=.1f&&g.startDelay>=0));
}
foreach(var data in Resources.LoadAll<EnemyData>("Data/EnemyData/Campaign"))
{
    check(data.name+" readable multipliers",data.GetDamageMultiplier(data.WeakTo)==2&&Mathf.Approximately(data.GetDamageMultiplier(data.ResistantTo),.3f));
    if(data.isBoss)check(data.name+" lethal escape",data.BaseDamage>=100&&Mathf.Approximately(data.GetDamageMultiplier(ElementType.Normal),.45f));
    // Exercise actual damage application, not just the multiplier getter.
    foreach(ElementType element in System.Enum.GetValues(typeof(ElementType)))
    {
        var obj=new GameObject("BalanceDamageProbe");obj.SetActive(false);var enemy=obj.AddComponent<Enemy>();enemy.enemyData=data;
        typeof(Enemy).GetField("currenthealth",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(enemy,10000f);
        enemy.TakeDamage(100,element);
        check(data.name+" actual damage "+element,Mathf.Abs(enemy.CurrentHealth-(10000-100*data.GetDamageMultiplier(element)))<.01f);
        UnityEngine.Object.DestroyImmediate(obj);
    }
}
foreach(var data in Resources.LoadAll<TowerData>("Data/TowerData"))
{
    check(data.name+" level20 cooldown not collapsed",data.attackSpeed/(1+19*data.levelAttackSpeedGrowth)>.7f);
    check(data.name+" level growth bounded",Mathf.Approximately(data.levelDamageGrowth,.05f)&&Mathf.Approximately(data.maximumCriticalChance,.6f));
}
check("No invalid item prices or stats",Resources.LoadAll<ItemData>("Data/Items").All(i=>i.cost>=0&&i.damageBonus>=0&&i.attackSpeedBonus>=0&&i.criticalChanceBonus>=0));
return results;
