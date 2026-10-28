var gm=UnityEngine.Object.FindObjectOfType<GameManager>();if(gm==null||gm.HasEnded)return "result screen";
gm.TogglePause();return Newtonsoft.Json.JsonConvert.SerializeObject(new{paused=gm.IsPaused,timeScale=UnityEngine.Time.timeScale,state=gm.CurrentState.ToString(),time=UnityEngine.Time.time});
