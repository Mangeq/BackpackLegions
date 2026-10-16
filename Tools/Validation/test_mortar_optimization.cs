var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(n,v)=>rows.Add(new{test=n,passed=v});
Time.timeScale=0;var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
var tower=GameObject.Find("MortarProbe").GetComponent<MortarTower>();
var created=new System.Collections.Generic.List<UnityEngine.Object>();
var position=new Vector3(900,0,900);
System.Func<Vector3,bool,Enemy> make=(pos,air)=>{var go=new GameObject("MortarOptimizationTarget");created.Add(go);go.transform.position=pos;var e=go.AddComponent<Enemy>();e.enabled=false;e.enemyData=ScriptableObject.CreateInstance<EnemyData>();created.Add(e.enemyData);e.enemyData.health=100000;e.enemyData.movementType=air?EnemyData.MovementType.Air:EnemyData.MovementType.Ground;e.enemyData.weaknesses=new System.Collections.Generic.List<ElementType>();e.enemyData.resistances=new System.Collections.Generic.List<ElementType>();typeof(Enemy).GetField("currenthealth",flags).SetValue(e,100000f);go.AddComponent<SphereCollider>().radius=.1f;return e;};
var explode=typeof(MortarProjectile).GetMethod("Explode",flags);var update=typeof(MortarProjectile).GetMethod("Update",flags);
System.Action<MortarProjectile> finish=p=>{p.GetComponentInChildren<ParticleSystem>().Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);update.Invoke(p,null);};
try{
check("Moving speed-four Raider takes damage",UnityEditor.SessionState.GetString("MortarMovingResult","").Contains("hit=True"));
check("Eight prepared projectiles inactive after flight and effect",tower.GetComponentsInChildren<MortarProjectile>(true).Length==8&&tower.GetComponentsInChildren<MortarProjectile>().Length==0);
check("Model and solid collider reduced without changing range",tower.transform.Find("weapon-cannon").localScale==Vector3.one&&tower.GetComponent<BoxCollider>().size.x==.6f&&tower.GetComponentInChildren<SphereCollider>().radius==6);
var e=make(position,false);var air=make(position,true);var far=make(position+Vector3.right*5,false);var child=new GameObject("SecondCollider");child.transform.SetParent(e.transform,false);child.AddComponent<SphereCollider>().radius=.1f;Physics.SyncTransforms();
var all=tower.GetComponentsInChildren<MortarProjectile>(true);var ids=all.Select(p=>p.GetInstanceID()).ToArray();
for(int n=0;n<100;n++){
tower.Attack(e.gameObject);var shell=tower.GetComponentsInChildren<MortarProjectile>().Single();explode.Invoke(shell,null);explode.Invoke(shell,null);finish(shell);
}
check("One hundred shots reuse exact same eight projectile objects",tower.GetComponentsInChildren<MortarProjectile>(true).Select(p=>p.GetInstanceID()).SequenceEqual(ids));
check("One hundred impacts retain only eight effects",tower.GetComponentsInChildren<ParticleSystem>(true).Length==8);
check("Pooled shots reset damage and multi-collider guard",Mathf.Abs(e.CurrentHealth-93000)<.01f);
check("Air and distant targets remain unharmed",air.CurrentHealth==100000&&far.CurrentHealth==100000);
var dense=new System.Collections.Generic.List<Enemy>();for(int n=0;n<80;n++)dense.Add(make(position+Vector3.forward*.5f,false));Physics.SyncTransforms();tower.Attack(e.gameObject);var blast=tower.GetComponentsInChildren<MortarProjectile>().Single();explode.Invoke(blast,null);check("Overlap buffer expands without losing any of eighty targets",dense.All(x=>x.CurrentHealth==99930));finish(blast);
tower.Attack(e.gameObject);var travel=tower.GetComponentsInChildren<MortarProjectile>().Single();var start=travel.transform.position;update.Invoke(travel,null);check("Pause still freezes reused projectile",travel.transform.position==start);e.transform.position+=Vector3.right*5;Physics.SyncTransforms();float hp=e.CurrentHealth;explode.Invoke(travel,null);check("Fixed impact point still allows escape",e.CurrentHealth==hp);finish(travel);
var fx=tower.GetComponentsInChildren<ParticleSystem>(true)[0];check("Effect uses six twelve-triangle particles",fx.emission.GetBurst(0).count.constant==6&&fx.GetComponent<ParticleSystemRenderer>().mesh.triangles.Length/3==12);
check("Effect shadows disabled",fx.GetComponent<ParticleSystemRenderer>().shadowCastingMode==UnityEngine.Rendering.ShadowCastingMode.Off&&!fx.GetComponent<ParticleSystemRenderer>().receiveShadows);
}finally{foreach(var item in created)if(item!=null)UnityEngine.Object.DestroyImmediate(item);}
return rows;
