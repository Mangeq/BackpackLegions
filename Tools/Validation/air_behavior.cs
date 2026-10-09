var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(name,passed)=>rows.Add(new{test=name,passed=passed});
var air=UnityEngine.GameObject.Find("AirTest").GetComponent<Enemy>();var ground=UnityEngine.GameObject.Find("GroundTest").GetComponent<Enemy>();var unit=UnityEngine.GameObject.Find("UnitTest").GetComponent<Unit>();var agent=air.GetComponent<UnityEngine.AI.NavMeshAgent>();var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var route=new UnityEngine.AI.NavMeshPath();bool routeValid=UnityEngine.AI.NavMesh.CalculatePath(air.transform.position,UnityEngine.GameObject.FindGameObjectWithTag("Base").transform.position,agent.areaMask,route);
check("air has a complete multi-corner ground route",agent.isOnNavMesh&&routeValid&&route.status==UnityEngine.AI.NavMeshPathStatus.PathComplete&&route.corners.Length>2);
check("visual flies one world unit above navigation root",UnityEngine.Mathf.Abs(air.visualRoot.position.y-air.transform.position.y-air.enemyData.flightHeight)<.01f);
check("air has no ground avoidance and trigger body",agent.obstacleAvoidanceType==UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance&&air.GetComponent<UnityEngine.Collider>().isTrigger);
unit.Engage(air.gameObject);air.AttackTarget(unit.gameObject);check("both combat entry points reject air",unit.IsIdle&&air.currentUnit==null&&!agent.isStopped);
var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;var choose=typeof(Tower).GetMethod("SelectTarget",flags);
foreach(var prefab in new[]{ui.MegicTowerPrefab,ui.ArcherTowerPrefab}){
    var tower=UnityEngine.Object.Instantiate(prefab,new UnityEngine.Vector3(1000,0,1000),UnityEngine.Quaternion.identity).GetComponent<Tower>();tower.enabled=false;
    tower.SendMessage("OnTriggerEnter",air.GetComponent<UnityEngine.Collider>());tower.SendMessage("OnTriggerEnter",ground.GetComponent<UnityEngine.Collider>());
    tower.SetTargetType(1);check(prefab.name+" ground filter excludes air",(UnityEngine.GameObject)choose.Invoke(tower,null)==ground.gameObject);
    tower.SetTargetType(2);check(prefab.name+" air filter selects air",(UnityEngine.GameObject)choose.Invoke(tower,null)==air.gameObject);
    tower.SetTargetType(0);tower.SetTargetOrder(0);check(prefab.name+" both-first uses entry order",(UnityEngine.GameObject)choose.Invoke(tower,null)==air.gameObject);tower.SetTargetOrder(1);check(prefab.name+" both-last uses entry order",(UnityEngine.GameObject)choose.Invoke(tower,null)==ground.gameObject);
    var bp=tower is MegicTower?((MegicTower)tower).MegicBulletP:((ArcherTower)tower).ArcherBulletP;var bullet=UnityEngine.Object.Instantiate(bp,air.AimPosition,UnityEngine.Quaternion.identity).GetComponent<Bullet>();float hp=air.CurrentHealth;bullet.Initialize(air.transform,1,0);bullet.SendMessage("Update");check(prefab.name+" projectile hits elevated visual position",air.CurrentHealth<hp);UnityEngine.Object.Destroy(tower.gameObject);
}
var barracks=UnityEngine.Object.Instantiate(ui.UnitTowerPrefab,new UnityEngine.Vector3(1000,0,1000),UnityEngine.Quaternion.identity).GetComponent<UnitTower>();barracks.enabled=false;UnityEngine.Physics.SyncTransforms();ground.GetComponent<UnityEngine.Collider>().enabled=false;
var find=typeof(UnitTower).GetMethod("FindClosestUnclaimedEnemy",flags);check("barracks proximity scan excludes air",find.Invoke(barracks,new object[]{air.transform.position,2f})==null);ground.GetComponent<UnityEngine.Collider>().enabled=true;UnityEngine.Object.Destroy(barracks.gameObject);
unit.Engage(ground.gameObject);check("ground enemy still engages unit",!unit.IsIdle&&ground.currentUnit==unit.gameObject);
check("air health bar exists",typeof(HealthBarUI).GetField("bar",flags).GetValue(air.GetComponent<HealthBarUI>())!=null);
var info=UnityEngine.Object.FindObjectOfType<InfoPanelUI>();info.Show(air,new UnityEngine.Vector2(500,500));check("air tooltip reports path and unit bypass",info.bodyText.text.Contains("Hava · Yolu takip eder"));info.Hide();
UnityEngine.Object.Destroy(unit.gameObject);UnityEngine.Object.Destroy(ground.gameObject);
return rows;
