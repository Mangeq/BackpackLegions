// Run through Unity execute_code, outside Play mode. AssetDatabase keeps GUIDs.
if (UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Exit Play mode first.");
if (System.IO.File.Exists("Docs/Validation/item-data-migration-2026-10-10.json")) throw new System.Exception("One-time migration already recorded. Verify/resume the existing manifest instead of overwriting it.");
var root = "Assets/Resources/Data";
var items = UnityEditor.AssetDatabase.FindAssets("t:ItemData", new[]{root}).Select(g => UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
var recipes = UnityEditor.AssetDatabase.FindAssets("t:ItemEvolutionRecipe", new[]{root}).Select(g => UnityEditor.AssetDatabase.LoadAssetAtPath<ItemEvolutionRecipe>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
var towers = UnityEditor.AssetDatabase.FindAssets("t:TowerData", new[]{root}).Select(g => UnityEditor.AssetDatabase.LoadAssetAtPath<TowerData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
var shop = UnityEngine.Object.FindObjectOfType<ShopManager>();
if(shop == null || shop.allItems == null) throw new System.Exception("ShopManager missing.");
var main = new System.Collections.Generic.HashSet<ItemData>(towers.SelectMany(t => new[]{t.defaultWeapon,t.fireWeapon,t.waterWeapon,t.earthWeapon,t.airWeapon}));
var offers = new System.Collections.Generic.HashSet<ItemData>(shop.allItems);
var evolved = new System.Collections.Generic.HashSet<ItemData>(recipes.Where(r=>r.result!=null).Select(r=>r.result));
var issues = new System.Collections.Generic.List<string>();
foreach(var x in items){
    if(x.itemIcon==null) issues.Add(x.name+": missing icon");
    if(x.itemShape==null || x.itemShape.Count==0 || x.itemShape.Distinct().Count()!=x.itemShape.Count || x.itemShape.Any(c=>c.x<0||c.y<0)) issues.Add(x.name+": invalid shape");
    if(x.itemOwners==null || x.itemOwners.Length==0) issues.Add(x.name+": missing owners");
    if(string.IsNullOrEmpty(x.itemID)) issues.Add(x.name+": missing ID");
    if(x.cost<0) issues.Add(x.name+": negative price");
    if(!main.Contains(x)&&!offers.Contains(x)&&!evolved.Contains(x)) issues.Add(x.name+": unclassified item");
    if(x.synergyRules!=null) foreach(var r in x.synergyRules){
        if(r==null) {issues.Add(x.name+": null synergy");continue;}
        if(r.filter==ItemSynergyRule.Filter.SpecificItem && r.acceptedItem==null) issues.Add(x.name+": missing synergy target");
        if(r.cells==null || r.cells.Count==0 || r.cells.Any(c=>x.itemShape.Contains(c)||!x.itemShape.Any(p=>System.Math.Abs(p.x-c.x)+System.Math.Abs(p.y-c.y)==1))) issues.Add(x.name+": invalid synergy cell");
    }
}
foreach(var g in items.GroupBy(x=>x.itemID).Where(g=>g.Count()>1)) issues.Add("Duplicate ID: "+g.Key);
foreach(var r in recipes){
    if(r.first==null||r.second==null||r.result==null){issues.Add(r.name+": incomplete recipe");continue;}
    if(!r.first.itemOwners.Intersect(r.second.itemOwners).Intersect(r.result.itemOwners).Any()) issues.Add(r.name+": no shared tower owner");
    if(r.first.element!=r.second.element || r.first.element!=r.result.element) issues.Add(r.name+": mixed elements");
}
if(shop.allItems.Any(x=>x==null)||offers.Count!=shop.allItems.Count) issues.Add("Shop null or duplicate entries");
foreach(var t in towers) foreach(ElementType e in System.Enum.GetValues(typeof(ElementType))){var x=t.GetMainItem(e);if(x==null||!x.IsCompatibleWith(t.type,e)||x.element!=e) issues.Add(t.name+": invalid main "+e);}
var plans=new System.Collections.Generic.List<object>();
var paths=new System.Collections.Generic.Dictionary<string,string>();
var snapshots=new System.Collections.Generic.Dictionary<string,string>();
foreach(var x in items){
    var old=UnityEditor.AssetDatabase.GetAssetPath(x);
    var role=main.Contains(x)?"Main":offers.Contains(x)?"Shop":"Evolved";
    var name=System.IO.Path.GetFileNameWithoutExtension(old);
    if(old.Contains("/MortarV1/")){
        var n=x.Name;
        foreach(var prefix in new[]{"Ateş ","Su ","Hava ","Toprak "}) if(n.StartsWith(prefix)) n=n.Substring(prefix.Length);
        n=n.ToLowerInvariant().Replace('ı','i').Replace('ş','s').Replace('ğ','g').Replace('ü','u').Replace('ö','o').Replace('ç','c').Replace('â','a');
        n=System.Text.RegularExpressions.Regex.Replace(n,"[^a-z0-9]+","_").Trim('_');
        name="mortar_"+n+"_"+x.element.ToString().ToLowerInvariant();
    }
    paths.Add(old,root+"/Items/"+role+"/"+x.element+"/"+name+".asset");
}
foreach(var r in recipes){
    var old=UnityEditor.AssetDatabase.GetAssetPath(r);var name=System.IO.Path.GetFileName(old);
    if(old.Contains("/MortarV1/")&&r.result!=null) name="recipe_"+System.IO.Path.GetFileName(paths[UnityEditor.AssetDatabase.GetAssetPath(r.result)]);
    paths.Add(old,root+"/EvolutionRecipes/"+(r.first==null?"Unconfigured":r.first.element.ToString())+"/"+name);
}
if(paths.Values.Distinct().Count()!=paths.Count) throw new System.Exception("Destination collision.");
foreach(var pair in paths){
    if(pair.Key!=pair.Value && UnityEditor.AssetDatabase.LoadMainAssetAtPath(pair.Value)!=null) throw new System.Exception("Destination exists: "+pair.Value);
    var guid=UnityEditor.AssetDatabase.AssetPathToGUID(pair.Key);
    snapshots.Add(guid,UnityEditor.EditorJsonUtility.ToJson(UnityEditor.AssetDatabase.LoadMainAssetAtPath(pair.Key)));
    plans.Add(new{guid,oldPath=pair.Key,newPath=pair.Value});
}
System.IO.Directory.CreateDirectory("Docs/Validation");
System.IO.File.WriteAllText("Docs/Validation/item-data-migration-2026-10-10.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{issues,plans,snapshots},Newtonsoft.Json.Formatting.Indented));
// Create only the approved destination folders.
foreach(var path in paths.Values){var dir=System.IO.Path.GetDirectoryName(path).Replace('\\','/');var current="Assets";foreach(var part in dir.Substring(7).Split('/')){var next=current+"/"+part;if(!UnityEditor.AssetDatabase.IsValidFolder(next)) UnityEditor.AssetDatabase.CreateFolder(current,part);current=next;}}
foreach(var pair in paths){if(pair.Key==pair.Value)continue;var error=UnityEditor.AssetDatabase.MoveAsset(pair.Key,pair.Value);if(!string.IsNullOrEmpty(error))throw new System.Exception(error);}
var failures=new System.Collections.Generic.List<string>();
foreach(var pair in snapshots){var x=UnityEditor.AssetDatabase.LoadMainAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(pair.Key));var before=Newtonsoft.Json.Linq.JObject.Parse(pair.Value);var after=Newtonsoft.Json.Linq.JObject.Parse(UnityEditor.EditorJsonUtility.ToJson(x));((Newtonsoft.Json.Linq.JObject)before["MonoBehaviour"]).Remove("m_Name");((Newtonsoft.Json.Linq.JObject)after["MonoBehaviour"]).Remove("m_Name");if(!Newtonsoft.Json.Linq.JToken.DeepEquals(before,after))failures.Add(pair.Key);}
if(failures.Count>0)throw new System.Exception("Unexpected data changes: "+string.Join(",",failures.ToArray()));
UnityEditor.AssetDatabase.SaveAssets();
return new{moved=paths.Count,items=items.Length,recipes=recipes.Length,shop=offers.Count,main=main.Count,evolved=items.Count(x=>!main.Contains(x)&&!offers.Contains(x)),issues,unchangedData=snapshots.Count,sceneDirty=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty};
