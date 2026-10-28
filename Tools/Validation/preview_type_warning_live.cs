var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var tower=System.Array.Find(UnityEngine.Object.FindObjectsOfType<Tower>(),t=>t.transform.position.x>900);UİManager.activeTower=tower;
BackpackUIManager.Instance.OpenBackpack(tower);ui.OpenTowerTypeChange();ui.ChooseTransitionType((int)TowerType.MagicTower);
return "visible="+ui.transitionDetails.enabled+" text="+ui.transitionDetails.text;

