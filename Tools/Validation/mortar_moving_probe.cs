var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
UnityEngine.Object.FindObjectOfType<WaveManager>().StopWaves();
typeof(GameManager).GetProperty("CurrentState").SetValue(gm,GameManager.GameState.Combat,null);
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();
var e=UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter01/Raider.prefab"),wm.spawnPoint.position,Quaternion.identity).GetComponent<Enemy>();
e.name="MortarMovingProbe";e.enemyData=UnityEngine.Object.Instantiate(e.enemyData);e.enemyData.health=10000;
var t=UnityEngine.Object.Instantiate(ui.MortarTowerPrefab,wm.spawnPoint.position+Vector3.right*3,Quaternion.identity).GetComponent<MortarTower>();t.name="MortarProbe";t.enabled=false;
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
typeof(MortarTower).GetMethod("Start",flags).Invoke(t,null);typeof(Tower).GetField("currentCriticalChance",flags).SetValue(t,0f);
Time.timeScale=1;float begin=Time.time;bool fired=false;Vector3 point=Vector3.zero;
UnityEditor.SessionState.SetString("MortarMovingResult","pending");
UnityEditor.EditorApplication.CallbackFunction tick=null;
tick=()=>{
if(!UnityEditor.EditorApplication.isPlaying){UnityEditor.EditorApplication.update-=tick;return;}
if(!fired&&Time.time-begin>=1){point=e.transform.position;t.Attack(e.gameObject);fired=true;}
if(Time.time-begin>=2.3f){
Time.timeScale=0;UnityEditor.EditorApplication.update-=tick;
UnityEditor.SessionState.SetString("MortarMovingResult","HP="+e.CurrentHealth+"; speed="+e.speed+"; flight="+t.data.mortarFlightTime+"; distance="+Vector3.Distance(point,e.transform.position)+"; hit="+(e.CurrentHealth<10000));
}};
UnityEditor.EditorApplication.update+=tick;return "Single shot against actual moving NavMesh Raider";
