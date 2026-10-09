// Execute as a method body with Unity MCP execute_code in Play mode.
// Captures the actual Game View, including UI, without changing scene assets.
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();
var dir="C:/Users/Emir/BackPackDefance/Marketing/Steam/2026-10-09/raw/combat";
System.IO.Directory.CreateDirectory(dir);
UnityEngine.Time.captureFramerate=30;
var log=new System.Text.StringBuilder("frame,state,enemies,bullets,gold\n");
int preparationFrames=0;
gm.StartGame();
var sequence=System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0,1801),i=>{
    if(!UnityEditor.EditorApplication.isPlaying) return (object)null;
    if(i>0){
        var tex=UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();
        System.IO.File.WriteAllBytes(dir+"/"+(i-1).ToString("D5")+".jpg",UnityEngine.ImageConversion.EncodeToJPG(tex,94));
        UnityEngine.Object.Destroy(tex);
        log.Append((i-1)+","+gm.CurrentState+","+UnityEngine.Object.FindObjectsOfType<Enemy>().Length+","+UnityEngine.Object.FindObjectsOfType<Bullet>().Length+","+EconomyManager.Gold+"\n");
    }
    if(gm.CurrentState==GameManager.GameState.Preparation){
        preparationFrames++;
        if(preparationFrames>=30){gm.StartGame();preparationFrames=0;}
    }
    if(i==1800){UnityEngine.Time.captureFramerate=0;UnityEngine.Time.timeScale=0;System.IO.File.WriteAllText(dir+"/frames.csv",log.ToString());}
    return (object)new UnityEngine.WaitForEndOfFrame();
});
gm.StartCoroutine(sequence.GetEnumerator());
return "Recording 1800 genuine gameplay frames at 30 fps";
