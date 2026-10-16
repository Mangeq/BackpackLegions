if (UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources/Data/Items")) throw new System.Exception("Historical setup: content already migrated. Edit existing assets; do not recreate old folders.");
if(UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Stop Play first");
var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
if(scene.path!="Assets/Scenes/GameScane.unity" || scene.isDirty) throw new System.Exception("Expected clean GameScane");
if(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/MortarTower.prefab")!=null) throw new System.Exception("Already installed");
UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene,"Backups/2026-10-09-mortar/BeforeMortar.unity",true);
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var shop=UnityEngine.Object.FindObjectOfType<ShopManager>();
var original=ui.ArcherTowerPrefab.GetComponent<Tower>();
UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/Data","MortarV1");
var cannonIcon=UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Prefabs/UI/PurchaseArt/Cannon.png");
var mapping=new System.Collections.Generic.Dictionary<ItemData,ItemData>();
var sources=shop.allItems.Where(i=>i.itemOwners.Contains(TowerType.ArrowTower)).ToList();
for(int e=0;e<5;e++)sources.Add(original.data.GetMainItem((ElementType)e));
foreach(var recipe in original.evolutionRecipes)sources.Add(recipe.result);
var itemNames=new System.Collections.Generic.Dictionary<string,string>{{"shortbow","Barut Kesesi"},{"archer_gloves","Topçu Eldivenleri"},{"scout_hood","Nişancı Başlığı"},{"hawk_pendant","Hedef Tılsımı"},{"warbow","Ağır Gülle"},{"ranger_boots","Seri Doldurma Takımı"},{"focus_ring","Odak Yüzüğü"},{"piercing_arrows","Patlayıcı Gülle"},{"ranger_cloak","Topçu Pelerini"},{"moonbow","Kuşatma Mühimmatı"}};
foreach(var source in sources.Distinct())
{
    var item=UnityEngine.Object.Instantiate(source);
    item.itemID="mortar_"+source.itemID;
    item.itemOwners=new[]{TowerType.CannonTower};
    string label="Güçlendirilmiş Barut";
    foreach(var pair in itemNames)if(source.itemID.Contains(pair.Key)){label=pair.Value;break;}
    bool main=original.data.IsMainItem(source);
    if(main) label="Havan Namlusu";
    else if(original.evolutionRecipes.Any(r=>r.result==source))label="Güçlendirilmiş Barut";
    item.Name=(source.element==ElementType.Normal?"":UİManager.ElementLabel(source.element)+" ")+label;
    if(main || item.Type==ItemType.Weapon) item.itemIcon=cannonIcon;
    UnityEditor.AssetDatabase.CreateAsset(item,"Assets/Resources/Data/MortarV1/"+item.itemID+".asset");
    mapping.Add(source,item);
}
foreach(var pair in mapping)
{
    var item=pair.Value;
    foreach(var rule in item.synergyRules)if(rule.acceptedItem!=null && mapping.ContainsKey(rule.acceptedItem))rule.acceptedItem=mapping[rule.acceptedItem];
    UnityEditor.EditorUtility.SetDirty(item);
}
var recipes=new System.Collections.Generic.List<ItemEvolutionRecipe>();
foreach(var source in original.evolutionRecipes)
{
    var recipe=UnityEngine.Object.Instantiate(source);recipe.first=mapping[source.first];recipe.second=mapping[source.second];recipe.result=mapping[source.result];
    UnityEditor.AssetDatabase.CreateAsset(recipe,"Assets/Resources/Data/MortarV1/recipe_"+recipe.result.itemID+".asset");recipes.Add(recipe);
}
var data=UnityEngine.Object.Instantiate(original.data);
data.type=TowerType.CannonTower;data.towerLevel=1;data.damage=70;data.attackSpeed=3;data.range=6;data.cost=350;
data.explosionRadius=1.5f;data.mortarFlightTime=.9f;data.mortarArcHeight=2.5f;data.mortarCriticalMultiplier=2;
data.defaultWeapon=mapping[original.data.defaultWeapon];data.fireWeapon=mapping[original.data.fireWeapon];data.waterWeapon=mapping[original.data.waterWeapon];data.earthWeapon=mapping[original.data.earthWeapon];data.airWeapon=mapping[original.data.airWeapon];
UnityEditor.AssetDatabase.CreateAsset(data,"Assets/Resources/Data/TowerData/MortarData.asset");
var ball=UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere);ball.name="MortarProjectile";ball.transform.localScale=Vector3.one*.22f;
UnityEngine.Object.DestroyImmediate(ball.GetComponent<Collider>());
var material=new Material(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ArcherBullet.prefab").GetComponent<Renderer>().sharedMaterial);
material.color=new Color(.25f,.22f,.16f);UnityEditor.AssetDatabase.CreateAsset(material,"Assets/Resources/Data/MortarV1/MortarShell.mat");ball.GetComponent<Renderer>().sharedMaterial=material;
ball.AddComponent<MortarProjectile>();var ballPrefab=UnityEditor.PrefabUtility.SaveAsPrefabAsset(ball,"Assets/Prefabs/MortarProjectile.prefab");UnityEngine.Object.DestroyImmediate(ball);
var go=new GameObject("MortarTower");
var model=(GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Envoirtments/Models/FBX format/weapon-cannon.fbx"));
model.transform.SetParent(go.transform,false);model.transform.localScale=Vector3.one*2;
var box=go.AddComponent<BoxCollider>();box.center=new Vector3(0,.45f,0);box.size=new Vector3(1.2f,.9f,1.2f);
var rb=go.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
var range=new GameObject("RangeTrigger");range.transform.SetParent(go.transform,false);var sphere=range.AddComponent<SphereCollider>();sphere.isTrigger=true;sphere.radius=6;
var tower=go.AddComponent<MortarTower>();tower.data=data;tower.evolutionRecipes=recipes;tower.projectilePrefab=ballPrefab.GetComponent<MortarProjectile>();
var muzzle=new GameObject("Muzzle");muzzle.transform.SetParent(go.transform,false);muzzle.transform.localPosition=new Vector3(0,1,0);tower.muzzle=muzzle.transform;
ui.MortarTowerPrefab=UnityEditor.PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/MortarTower.prefab");UnityEngine.Object.DestroyImmediate(go);
ui.mortarPurchasePortrait=cannonIcon;
foreach(var source in shop.allItems.ToArray())if(mapping.ContainsKey(source))shop.allItems.Add(mapping[source]);
var buttons=ui.transitionTypes.ToList();
var next=UnityEngine.Object.Instantiate(buttons[0],buttons[0].transform.parent);next.name="TypeCannonTower";next.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();
UnityEditor.Events.UnityEventTools.AddIntPersistentListener(next.onClick,ui.ChooseTransitionType,(int)TowerType.CannonTower);
next.GetComponentInChildren<UnityEngine.UI.Text>().text="Havan Kulesi";buttons.Add(next);ui.transitionTypes=buttons.ToArray();
for(int i=0;i<buttons.Count;i++){var rect=(RectTransform)buttons[i].transform;rect.sizeDelta=new Vector2(166,42);rect.anchoredPosition=new Vector2(28+i*178,-82);var label=buttons[i].GetComponentInChildren<UnityEngine.UI.Text>().rectTransform;label.sizeDelta=new Vector2(162,42);}
UnityEditor.EditorUtility.SetDirty(ui);UnityEditor.EditorUtility.SetDirty(shop);
UnityEditor.AssetDatabase.SaveAssets();UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
return new{items=mapping.Count,shopItems=shop.allItems.Count,recipes=recipes.Count,prefab=UnityEditor.AssetDatabase.GetAssetPath(ui.MortarTowerPrefab)};
