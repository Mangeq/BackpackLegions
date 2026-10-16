var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();
if(gm.CurrentState!=GameManager.GameState.Preparation)throw new System.Exception("Fresh preparation required");
UnityEngine.Random.InitState(1701);
string scenario="SCENARIO";
var slots=UnityEngine.Object.FindObjectsOfType<TowerSlot>().Where(s=>s.transform.position.z<10).OrderBy(s=>s.transform.position.x).ToArray();
string[] types=scenario=="starter"?new[]{"ArcherTower1","ArcherTower1"}:scenario=="poor"?new[]{"MegicTower1","MegicTower1"}:new[]{"MortarTower","MegicTower1","UnitTower1","MegicTower1","ArcherTower1"};
int[] indices=types.Length==2?new[]{0,1}:new[]{0,1,2,3,4};
for(int i=0;i<types.Length;i++)
{
    var prefab=UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+types[i]+".prefab");
    var tower=UnityEngine.Object.Instantiate(prefab,slots[indices[i]].transform.position,Quaternion.identity).GetComponent<Tower>();tower.name="BalanceProbe"+i;
    typeof(Tower).GetMethod("InitializeTower",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(tower,null);
    // Deliberate combat fixture; purchases are recorded but do not depend on random offers.
    if(scenario!="starter")tower.AddExperience(scenario=="poor"?168:324);
    if(scenario=="strong"&&!tower.TrySelectElement(wm.ActiveChapter.waves.Last().groups[0].enemyPrefab.GetComponent<Enemy>().enemyData.WeakTo))throw new System.Exception("Element selection failed");
    var candidates=Resources.LoadAll<ItemData>("Data/Items/Shop").Where(d=>d.IsCompatibleWith(tower.data.type,tower.SelectedElement)&&d.element==tower.SelectedElement&&d.cost>0).ToArray();
    int budget=scenario=="starter"?100:scenario=="poor"?10000:340;
    int spent=tower.data.cost;
    for(int n=0;n<25;n++)
    {
        ItemData chosen=null;Vector2Int origin=Vector2Int.zero;int rotation=0;float best=0;
        foreach(var item in candidates)
        {
            if(item.cost>budget)continue;
            float score=(item.damageBonus+item.attackSpeedBonus+item.criticalChanceBonus*.5f+(tower is UnitTower?item.healthBonus*.7f:0))/item.cost;
            if(score<=best)continue;
            bool placed=false;
            for(int r=0;r<4&&!placed;r++)for(int y=0;y<5&&!placed;y++)for(int x=0;x<5&&!placed;x++)
                if(tower.Backpack.CanPlace(item,new Vector2Int(x,y),r)){chosen=item;origin=new Vector2Int(x,y);rotation=r;best=score;placed=true;}
        }
        if(chosen==null)break;tower.Backpack.Place(chosen,origin,rotation);budget-=chosen.cost;spent+=chosen.cost;
    }
    tower.CalculateStats();tower.name+="_cost"+spent;
}
if(scenario!="starter")wm.waves=new System.Collections.Generic.List<WaveRound>{new WaveRound{groups=new System.Collections.Generic.List<WaveEnemyGroup>{new WaveEnemyGroup{enemyPrefab=wm.ActiveChapter.waves.Last().groups[0].enemyPrefab,enemyCount=1,spawnInterval=1}}}};
gm.SetGameSpeed(1);gm.StartGame();gm.SetGameSpeed(3);
return new{scenario,gold=EconomyManager.Gold,baseHealth=GameObject.FindWithTag("Base").GetComponent<Base>().currentHealth,towers=UnityEngine.Object.FindObjectsOfType<Tower>().Where(t=>t.name.StartsWith("BalanceProbe")).Select(t=>new{t.name,t.Level,element=t.SelectedElement.ToString(),items=t.Backpack.Items.Count}).ToArray()};

