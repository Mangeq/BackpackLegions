// Read-only verification after the 2026-10-10 migration; writes only its report.
var root="Assets/Resources/Data";
var rows=new System.Collections.Generic.List<object>();
System.Action<string,bool> check=(name,ok)=>rows.Add(new{test=name,passed=ok});
var doc=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("Docs/Validation/item-data-migration-2026-10-10.json"));
var changed=new System.Collections.Generic.List<string>();
foreach(var p in (Newtonsoft.Json.Linq.JArray)doc["plans"]){
    var guid=(string)p["guid"];var path=UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
    check("GUID and path: "+path,path==(string)p["newPath"]);
    var obj=UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
    if(obj==null){changed.Add(path);continue;}
    var before=(Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.Linq.JObject.Parse((string)doc["snapshots"][guid])["MonoBehaviour"];
    var after=(Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.Linq.JObject.Parse(UnityEditor.EditorJsonUtility.ToJson(obj))["MonoBehaviour"];
    before.Remove("m_Name");after.Remove("m_Name");
    // Only the five inert empty rules are intentionally removed; all other fields must match.
    var rules=before["synergyRules"] as Newtonsoft.Json.Linq.JArray;
    if(rules!=null){foreach(var rule in rules.ToArray()){
        var bonus=rule["bonus"] as Newtonsoft.Json.Linq.JObject;var cells=rule["cells"] as Newtonsoft.Json.Linq.JArray;var shape=before["itemShape"] as Newtonsoft.Json.Linq.JArray;
        if(string.IsNullOrEmpty((string)rule["description"])&&bonus!=null&&bonus.Properties().All(v=>(float)v.Value==0)&&cells!=null&&(cells.Count==0||cells.All(c=>shape.Any(s=>Newtonsoft.Json.Linq.JToken.DeepEquals(s,c)))))rule.Remove();
    }}
    if(!Newtonsoft.Json.Linq.JToken.DeepEquals(before,after))changed.Add(path);
}
check("All gameplay data and references preserved except five inert rules",changed.Count==0);
var items=UnityEditor.AssetDatabase.FindAssets("t:ItemData",new[]{root+"/Items"}).Select(g=>UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
var recipes=UnityEditor.AssetDatabase.FindAssets("t:ItemEvolutionRecipe",new[]{root+"/EvolutionRecipes"}).Select(g=>UnityEditor.AssetDatabase.LoadAssetAtPath<ItemEvolutionRecipe>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
var shop=UnityEngine.Object.FindObjectOfType<ShopManager>();
check("240 items and 21 recipes",items.Length==240&&recipes.Length==21);
check("200 unique shop entries",shop!=null&&shop.allItems.Count==200&&shop.allItems.All(x=>x!=null)&&shop.allItems.Distinct().Count()==200);
check("Unique nonempty item IDs",items.All(x=>!string.IsNullOrEmpty(x.itemID))&&items.Select(x=>x.itemID).Distinct().Count()==items.Length);
foreach(var x in items){
    check("Item data: "+x.name,x.itemIcon!=null&&x.itemShape!=null&&x.itemShape.Count>0&&x.itemShape.Distinct().Count()==x.itemShape.Count&&x.itemShape.All(c=>c.x>=0&&c.y>=0)&&x.itemOwners!=null&&x.itemOwners.Length>0&&x.cost>=0);
    if(x.synergyRules!=null)foreach(var rule in x.synergyRules)check("Synergy: "+x.name,rule!=null&&rule.cells!=null&&rule.cells.Count>0&&rule.cells.All(c=>!x.itemShape.Contains(c)&&x.itemShape.Any(s=>System.Math.Abs(s.x-c.x)+System.Math.Abs(s.y-c.y)==1))&&(rule.filter!=ItemSynergyRule.Filter.SpecificItem||rule.acceptedItem!=null));
}
foreach(var r in recipes)check("Recipe: "+r.name,r.first!=null&&r.second!=null&&r.result!=null&&r.first.element==r.second.element&&r.first.element==r.result.element&&r.first.itemOwners.Intersect(r.second.itemOwners).Intersect(r.result.itemOwners).Any());
foreach(TowerType t in System.Enum.GetValues(typeof(TowerType)))foreach(ElementType e in System.Enum.GetValues(typeof(ElementType)))foreach(ItemRarity rarity in System.Enum.GetValues(typeof(ItemRarity)))check("Shop pool "+t+"/"+e+"/"+rarity,shop.allItems.Any(i=>i.element==e&&i.Rarity==rarity&&i.itemOwners.Contains(t)));
foreach(var g in UnityEditor.AssetDatabase.FindAssets("t:TowerData",new[]{root+"/TowerData"})){var t=UnityEditor.AssetDatabase.LoadAssetAtPath<TowerData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g));foreach(ElementType e in System.Enum.GetValues(typeof(ElementType))){var x=t.GetMainItem(e);check("Main "+t.type+"/"+e,x!=null&&x.IsCompatibleWith(t.type,e)&&x.element==e&&x.itemShape.SequenceEqual(t.defaultWeapon.itemShape));}}
foreach(var path in new[]{"Assets/Prefabs/ArcherTower1.prefab","Assets/Prefabs/MegicTower1.prefab","Assets/Prefabs/UnitTower1.prefab","Assets/Prefabs/MortarTower.prefab"}){var t=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path).GetComponent<Tower>();check("Prefab recipe references "+t.name,t.evolutionRecipes!=null&&t.evolutionRecipes.Count>=5&&t.evolutionRecipes.All(r=>r!=null&&r.first!=null&&r.second!=null&&r.result!=null));}
System.IO.File.WriteAllText("Docs/Validation/item-data-verification-2026-10-10.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{rows,changed},Newtonsoft.Json.Formatting.Indented));
return new{checks=rows.Count,changed,rows};
