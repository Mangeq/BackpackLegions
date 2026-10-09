var air=UnityEngine.GameObject.Find("AirTest").GetComponent<Enemy>();var agent=air.GetComponent<UnityEngine.AI.NavMeshAgent>();var castle=UnityEngine.GameObject.FindGameObjectWithTag("Base").GetComponent<Base>();castle.currentHealth=castle.maxHealth;int before=castle.currentHealth;int damage=air.damage;var origin=air.transform.position;float begin=UnityEngine.Time.realtimeSinceStartup;bool moved=false,route=false;UnityEngine.Time.timeScale=1;
UnityEditor.SessionState.SetString("AirPathResult","running");UnityEditor.EditorApplication.CallbackFunction tick=null;
tick=()=>{
    if(!UnityEditor.EditorApplication.isPlaying){UnityEditor.EditorApplication.update-=tick;return;}
    if(air!=null){moved|=UnityEngine.Vector3.Distance(origin,air.transform.position)>1;route|=!agent.pathPending&&agent.path.corners.Length>2&&agent.isOnNavMesh;}
    if(air==null||UnityEngine.Time.realtimeSinceStartup-begin>35){
        UnityEngine.Time.timeScale=0;UnityEditor.EditorApplication.update-=tick;
        UnityEditor.SessionState.SetString("AirPathResult","moved="+moved+"; route="+route+"; arrived="+(air==null)+"; singleCastleHit="+(castle.currentHealth==before-damage)+"; health="+castle.currentHealth);
    }
};UnityEditor.EditorApplication.update+=tick;return true;

