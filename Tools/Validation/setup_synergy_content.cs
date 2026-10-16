if (UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Data/Items")) throw new System.Exception("Historical setup: content already migrated. Edit existing assets; do not recreate old folders.");
if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play");
if(!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Data/Evolutions"))UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Data","Evolutions");
System.Func<string,ItemData> item=(id)=>UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Resources/Data/ItemsV1/v1_"+id+".asset");
var keys=new[]{"apprentice_staff","shortbow","legion_sword"};var partners=new[]{"mana_elixir","archer_gloves","guard_belt"};var names=new[]{"Mana İşlemeli Asa","Çevik Avcı Yayı","Muhafız Kılıcı"};var recipes=new System.Collections.Generic.List<ItemEvolutionRecipe>();
for(int i=0;i<3;i++)
{
    var a=item(keys[i]);var b=item(partners[i]);if(a==null||b==null)throw new System.Exception("Missing sample item");
    var size=ItemShapeUtility.GetBoundingSize(a.itemShape);
    a.synergyRules=new System.Collections.Generic.List<ItemSynergyRule>{new ItemSynergyRule{description="Özel karede uygun eşya: +%8 hasar katkısı",cells=new System.Collections.Generic.List<UnityEngine.Vector2Int>{new UnityEngine.Vector2Int(size.x,0),new UnityEngine.Vector2Int(size.x,1)},filter=i==0?ItemSynergyRule.Filter.Type:i==1?ItemSynergyRule.Filter.SpecificItem:ItemSynergyRule.Filter.Any,acceptedType=ItemType.Potion,acceptedItem=b,bonus=new ItemBonus{damage=.08f}}};UnityEditor.EditorUtility.SetDirty(a);
    string productPath="Assets/Resources/Data/Evolutions/evolved_"+keys[i]+".asset";
    var product=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(productPath);
    if(product==null){product=UnityEngine.Object.Instantiate(a);UnityEditor.AssetDatabase.CreateAsset(product,productPath);}
    product.Name=names[i];product.itemID="evolved_"+keys[i];product.cost=a.cost+b.cost;product.Rarity=ItemRarity.Rare;
    product.damageBonus=a.damageBonus+b.damageBonus+.15f;product.attackSpeedBonus=a.attackSpeedBonus+b.attackSpeedBonus+.05f;product.criticalChanceBonus=a.criticalChanceBonus+b.criticalChanceBonus;product.healthBonus=a.healthBonus+b.healthBonus;
    product.synergyRules=new System.Collections.Generic.List<ItemSynergyRule>();UnityEditor.EditorUtility.SetDirty(product);
    string recipePath="Assets/Resources/Data/Evolutions/recipe_"+keys[i]+".asset";var recipe=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemEvolutionRecipe>(recipePath);
    if(recipe==null){recipe=UnityEngine.ScriptableObject.CreateInstance<ItemEvolutionRecipe>();UnityEditor.AssetDatabase.CreateAsset(recipe,recipePath);}
    recipe.first=a;recipe.second=b;recipe.result=product;UnityEditor.EditorUtility.SetDirty(recipe);recipes.Add(recipe);
}
// Sadece gerçek Tower bileşeni taşıyan prefab'lara uygun tarifleri ekle.
foreach(var guid in UnityEditor.AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs"}))
{
    string path=UnityEditor.AssetDatabase.GUIDToAssetPath(guid);var asset=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);if(asset.GetComponent<Tower>()==null)continue;
    var root=UnityEditor.PrefabUtility.LoadPrefabContents(path);try{
        var tower=root.GetComponent<Tower>();if(tower.data==null)continue;
        foreach(var recipe in recipes)if(System.Array.Exists(recipe.first.itemOwners,x=>x==tower.data.type)&&!tower.evolutionRecipes.Contains(recipe))tower.evolutionRecipes.Add(recipe);
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,path);
    }finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
}
UnityEditor.AssetDatabase.SaveAssets();return new{recipes=recipes.Count};
