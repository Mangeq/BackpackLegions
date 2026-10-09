var gm=UnityEngine.Object.FindObjectOfType<GameManager>();
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var shop=UnityEngine.Object.FindObjectOfType<ShopUIManager>();
var dir="C:/Users/Emir/BackPackDefance/Marketing/Steam/2026-10-09/raw/elements";
System.IO.Directory.CreateDirectory(dir);
var sequence=System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0,151),i=>{
    if(!UnityEditor.EditorApplication.isPlaying)return (object)null;
    if(i==45)ui.SelectTowerElement((int)ElementType.Fire);
    if(i==80)shop.RefreshShop();
    if(i>0){var tex=UnityEngine.ScreenCapture.CaptureScreenshotAsTexture();System.IO.File.WriteAllBytes(dir+"/"+(i-1).ToString("D5")+".jpg",UnityEngine.ImageConversion.EncodeToJPG(tex,94));if(i==150)System.IO.File.WriteAllBytes("C:/Users/Emir/BackPackDefance/Marketing/Steam/2026-10-09/screenshots/04-element-uzmanligi.png",UnityEngine.ImageConversion.EncodeToPNG(tex));UnityEngine.Object.Destroy(tex);}
    if(i==150)System.IO.File.WriteAllText(dir+"/complete.txt","150 frames; real Fire specialization and shop refresh");
    return (object)new UnityEngine.WaitForEndOfFrame();
});
gm.StartCoroutine(sequence.GetEnumerator());
return "Recording element selection";
