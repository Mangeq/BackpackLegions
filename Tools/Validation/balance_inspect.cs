var towers=Resources.LoadAll<TowerData>("Data/TowerData").Select(t=>new{path=UnityEditor.AssetDatabase.GetAssetPath(t),t.type,t.damage,t.attackSpeed,t.range,t.cost,t.firstLevelExperience,t.experienceIncreasePerLevel,t.explosionRadius}).ToArray();
var items=Resources.LoadAll<ItemData>("Data/Items").Select(i=>new{path=UnityEditor.AssetDatabase.GetAssetPath(i),i.Name,i.cost,i.damageBonus,i.attackSpeedBonus,i.criticalChanceBonus,i.healthBonus,cells=i.itemShape.Count}).ToArray();
return new{towers,items};
