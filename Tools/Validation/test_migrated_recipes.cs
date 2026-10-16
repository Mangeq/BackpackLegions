// Uses current assets and isolated runtime towers. Never saves Play-mode state.
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
UnityEngine.Object.FindObjectOfType<WaveManager>().StopWaves();Time.timeScale=0;
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic;
typeof(GameManager).GetProperty("CurrentState").SetValue(gm,GameManager.GameState.Preparation,null);
var prefabs=new[]{"Assets/Prefabs/ArcherTower1.prefab","Assets/Prefabs/MegicTower1.prefab","Assets/Prefabs/UnitTower1.prefab","Assets/Prefabs/MortarTower.prefab"}.Select(p=>UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(p)).ToArray();
var rows=new System.Collections.Generic.List<object>();
foreach(var guid in UnityEditor.AssetDatabase.FindAssets("t:ItemEvolutionRecipe",new[]{"Assets/Resources/Data/EvolutionRecipes"})){
    var recipe=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemEvolutionRecipe>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
    var prefab=prefabs.First(p=>recipe.first.itemOwners.Contains(p.GetComponent<Tower>().data.type)&&recipe.second.itemOwners.Contains(p.GetComponent<Tower>().data.type)&&recipe.result.itemOwners.Contains(p.GetComponent<Tower>().data.type));
    var go=UnityEngine.Object.Instantiate(prefab,new Vector3(700,0,700),Quaternion.identity);var tower=go.GetComponent<Tower>();
    try{
        typeof(Tower).GetMethod("InitializeTower",flags).Invoke(tower,null);
        typeof(Tower).GetProperty("SelectedElement").SetValue(tower,recipe.first.element,null);
        var bag=new Backpack(20,20);typeof(Tower).GetProperty("Backpack").SetValue(tower,bag,null);
        tower.evolutionRecipes=new System.Collections.Generic.List<ItemEvolutionRecipe>{recipe};
        bag.Place(tower.MainItem,new Vector2Int(15,15));var first=bag.Place(recipe.first,Vector2Int.zero);PlacedItem second=null;
        for(int y=0;y<10&&second==null;y++)for(int x=0;x<10&&second==null;x++){var origin=new Vector2Int(x,y);if(bag.CanPlace(recipe.second,origin)&&TowerItemEvolution.Adjacent(first,new PlacedItem(recipe.second,origin)))second=bag.Place(recipe.second,origin);}
        bool found=second!=null&&tower.Evolution.FindPairs().Count==1;
        tower.Evolution.BeginWave(0);bool waiting=bag.Items.Count==3;
        tower.Evolution.CompleteWave(0);bool evolved=bag.Items.Count==2&&bag.Items.Any(i=>i.Data==recipe.result)&&bag.Items.Any(i=>i.Data==tower.MainItem);
        tower.Evolution.CompleteWave(0);bool once=bag.Items.Count==2;
        rows.Add(new{test=recipe.name,passed=found&&waiting&&evolved&&once,found,waiting,evolved,once});
    }finally{UnityEngine.Object.DestroyImmediate(go);}
}
System.IO.File.WriteAllText("Docs/Validation/item-data-runtime-recipes-2026-10-10.json",Newtonsoft.Json.JsonConvert.SerializeObject(rows,Newtonsoft.Json.Formatting.Indented));
return rows;
