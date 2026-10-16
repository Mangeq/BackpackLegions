if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play first");
if(!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Data/SynergyRecipes"))UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Data","SynergyRecipes");
int count=0;var items=UnityEditor.AssetDatabase.FindAssets("t:ItemData").Select(g=>UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>(UnityEditor.AssetDatabase.GUIDToAssetPath(g))).ToArray();
foreach(var item in items){if(item.synergyRules==null)continue;
 for(int i=0;i<item.synergyRules.Count;i++){
  var old=item.synergyRules[i];if(old==null)continue;string path="Assets/Resources/Data/SynergyRecipes/"+item.name+"_"+i+".asset";
  var recipe=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemSynergyRecipe>(path);
  if(recipe==null){recipe=ScriptableObject.CreateInstance<ItemSynergyRecipe>();recipe.targetItem=item;recipe.supportItem=old.acceptedItem;recipe.filter=old.filter;recipe.supportType=old.acceptedType;recipe.description=old.description;recipe.bonus=old.bonus;recipe.placement=ItemSynergyRecipe.Placement.Cells;
    recipe.cells=old.cells.Where(cell=>!item.itemShape.Contains(cell)&&item.itemShape.Any(occupied=>Mathf.Abs(cell.x-occupied.x)+Mathf.Abs(cell.y-occupied.y)==1)).ToList();UnityEditor.AssetDatabase.CreateAsset(recipe,path);}
  if(!item.synergyRecipes.Contains(recipe))item.synergyRecipes.Add(recipe);count++;
 }
 UnityEditor.EditorUtility.SetDirty(item);
}
var sword=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Resources/Data/Items/Shop/Normal/v1_legion_sword.asset");
var crown=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Resources/Data/Items/Shop/Normal/v1_legion_crown.asset");
if(sword==null||crown==null)throw new System.Exception("Example items missing");
string examplePath="Assets/Resources/Data/SynergyRecipes/LegionSword_Crown.asset";var example=UnityEditor.AssetDatabase.LoadAssetAtPath<ItemSynergyRecipe>(examplePath);
if(example==null){example=ScriptableObject.CreateInstance<ItemSynergyRecipe>();example.targetItem=sword;example.supportItem=crown;example.placement=ItemSynergyRecipe.Placement.CellsOrAttached;example.cells.Add(new Vector2Int(sword.itemShape.Max(c=>c.x)+1,0));example.bonus=new ItemBonus{flatDamage=5};example.description="Sağ üst komşu karede veya kılıca takılıyken +5 sabit hasar. Tarif en fazla bir kez işler.";UnityEditor.AssetDatabase.CreateAsset(example,examplePath);}
if(!sword.synergyRecipes.Contains(example))sword.synergyRecipes.Add(example);UnityEditor.EditorUtility.SetDirty(sword);
UnityEditor.AssetDatabase.SaveAssets();return new {migrated=count,exampleTarget=sword.Name,exampleSupport=crown.Name};
