var rows=new System.Collections.Generic.List<object>();System.Action<string,bool> check=(n,ok)=>rows.Add(new{test=n,passed=ok});
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
var p=UnityEngine.Object.FindObjectOfType<WavePreviewUI>();
var tick=typeof(WavePreviewUI).GetMethod("Update",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
check("Initially hidden",!p.panel.activeSelf);
check("Chapter budget softened",EconomyManager.Gold==850);
check("Click pinning removed",!typeof(UnityEngine.EventSystems.IPointerClickHandler).IsAssignableFrom(typeof(WavePreviewUI)));
for(int i=0;i<5;i++)
{
    p.OnPointerEnter(pointer);check("Enter opens "+i,p.panel.activeSelf);
    tick.Invoke(p,null);check("Hover stays open "+i,p.panel.activeSelf);
    p.OnPointerExit(pointer);check("Exit closes immediately "+i,!p.panel.activeSelf);
    tick.Invoke(p,null);check("Stays closed "+i,!p.panel.activeSelf);
}
p.OnPointerEnter(pointer);gm.TogglePause();tick.Invoke(p,null);check("Pause hides",!p.panel.activeSelf);p.OnPointerExit(pointer);gm.ContinueGame();tick.Invoke(p,null);check("Resume outside stays hidden",!p.panel.activeSelf);
p.OnPointerEnter(pointer);p.enabled=false;check("Disable hides",!p.panel.activeSelf);p.enabled=true;tick.Invoke(p,null);check("Enable does not reopen",!p.panel.activeSelf);
var wm=UnityEngine.Object.FindObjectOfType<WaveManager>();typeof(WaveManager).GetField("currentWaveIndex",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(wm,1);tick.Invoke(p,null);check("New wave does not auto-open",!p.panel.activeSelf);
p.OnPointerEnter(pointer);check("New wave details correct",p.details.text==WavePreviewUI.Describe(wm.UpcomingWave));p.OnPointerExit(pointer);
foreach(var c in Resources.LoadAll<ChapterData>("Data/Chapters"))check("Chapter "+c.chapterNumber+" revised data",c.startingGold==850+150*(c.chapterNumber-1)&&c.TotalEnemies==180+80*(c.chapterNumber-1)&&c.waves.Last().groups.First(g=>g.enemyPrefab.GetComponent<Enemy>().enemyData.isBoss).enemyPrefab.GetComponent<Enemy>().enemyData.health==2400+1440*(c.chapterNumber-1));
return rows;

