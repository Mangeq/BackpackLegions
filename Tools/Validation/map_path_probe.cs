var map=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().First(g=>g.name.StartsWith("Blackpine"));var road=map.transform.Find("Road"); var filter=new UnityEngine.AI.NavMeshQueryFilter{agentTypeID=-1372625422,areaMask=-1};
var points=new[]{map.transform.Find("EnemySpawner").position,map.transform.Find("V4_Compact_Dark_Base").GetComponent<Renderer>().bounds.center};
return points.Select(p=>{UnityEngine.AI.NavMeshHit h;bool ok=UnityEngine.AI.NavMesh.SamplePosition(p,out h,20,filter);return new{input=p.ToString(),ok,point=h.position.ToString()};}).ToArray();
