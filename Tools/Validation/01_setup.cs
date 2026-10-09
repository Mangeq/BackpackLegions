var rows=new System.Collections.Generic.List<object>();
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
int missing=0;foreach(var root in gm.gameObject.scene.GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<UnityEngine.Transform>(true))missing+=UnityEditor.GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
rows.Add(new{test="Scene has no missing scripts",passed=missing==0});
rows.Add(new{test="Initial preparation and 60-second setting",passed=gm.CurrentState==GameManager.GameState.Preparation&&gm.autoStartDelay==60&&UnityEngine.Object.FindObjectsOfType<Enemy>().Length==0});
var slots=new System.Collections.Generic.List<TowerSlot>(UnityEngine.Object.FindObjectsOfType<TowerSlot>());var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();slots.Sort((a,b)=>UnityEngine.Vector3.Distance(a.transform.position,wm.spawnPoint.position).CompareTo(UnityEngine.Vector3.Distance(b.transform.position,wm.spawnPoint.position)));
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();int gold=EconomyManager.Gold;
int[] costs={ui.UnitTowerPrefab.GetComponent<Tower>().data.cost,ui.ArcherTowerPrefab.GetComponent<Tower>().data.cost,ui.MegicTowerPrefab.GetComponent<Tower>().data.cost};
for(int i=0;i<3;i++){slots[i].SendMessage("OnMouseDown");if(i==0)ui.UnitTowerBTN();else if(i==1)ui.ArcherTowerBTN();else ui.MegicTowerBTN();ui.BuyTower();rows.Add(new{test="Tower purchase "+i,passed=slots[i].isOccupied&&EconomyManager.Gold==gold-costs[i]});gold=EconomyManager.Gold;}
return rows;
