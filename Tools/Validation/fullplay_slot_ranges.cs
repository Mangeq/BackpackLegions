var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();if(wm==null)return "no game";
var path=new UnityEngine.AI.NavMeshPath();var destination=UnityEngine.GameObject.FindWithTag("Base");UnityEngine.AI.NavMesh.CalculatePath(wm.spawnPoint.position,destination.transform.position,UnityEngine.AI.NavMesh.AllAreas,path);
var output=new System.Collections.Generic.List<object>();
foreach(var slot in UnityEngine.Object.FindObjectsOfType<TowerSlot>()){
 float distance=float.MaxValue;var p=slot.transform.position;p.y=0;
 for(int i=1;i<path.corners.Length;i++){var a=path.corners[i-1];var b=path.corners[i];a.y=0;b.y=0;var v=b-a;float t=UnityEngine.Mathf.Clamp01(UnityEngine.Vector3.Dot(p-a,v)/v.sqrMagnitude);distance=UnityEngine.Mathf.Min(distance,UnityEngine.Vector3.Distance(p,a+v*t));}
 output.Add(new{slot=slot.name,nearestPathDistance=distance,position=slot.transform.position.ToString()});
}
return Newtonsoft.Json.JsonConvert.SerializeObject(new{pathStatus=path.status.ToString(),corners=path.corners,slots=output},new Newtonsoft.Json.JsonSerializerSettings{ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore});
