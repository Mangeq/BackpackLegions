using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

// Reads the same groups that WaveManager spawns; no duplicated enemy roster.
public class WavePreviewUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject panel;
    public TMPro.TMP_Text title;
    public TMPro.TMP_Text details;
    private WaveManager waves;
    private GameManager game;
    private bool hovering;
    private void Awake() { if(panel!=null)panel.SetActive(false); }
    private void OnDisable() { hovering=false;if(panel!=null)panel.SetActive(false); }
    private int lastWave = -1;
    private GameManager.GameState lastState;
    private void Start() { waves=FindObjectOfType<WaveManager>();game=FindObjectOfType<GameManager>();Refresh(); }
    private void Update()
    {
        if(waves==null||game==null)return;
        if(lastWave!=waves.UpcomingWaveIndex||lastState!=game.CurrentState)
        { Refresh(); }
        bool ended=game.CurrentState==GameManager.GameState.Victory||game.CurrentState==GameManager.GameState.Defeat;
        if(panel!=null)panel.SetActive(!ended&&!game.IsPaused&&hovering);
    }
    public void Refresh()
    {
        if(waves==null||game==null)return;
        lastWave=waves.UpcomingWaveIndex;lastState=game.CurrentState;
        title.text=waves.UpcomingWave==null?"SON DALGA":"SIRADAKİ DALGA  " + (lastWave+1) + " / " + waves.waves.Count;
        details.text=Describe(waves.UpcomingWave);
        var skin=GetComponent<WavePreviewSkin>();
        if(skin!=null)skin.Render(waves.UpcomingWave,lastWave,waves.waves.Count);
    }
    public static string Describe(WaveRound wave)
    {
        if(wave==null)return "Bu dalgadan sonra bölüm tamamlanır.";
        var text=new StringBuilder();
        foreach(var group in wave.GetGroups())
        {
            if(group==null||group.enemyPrefab==null||group.enemyCount<=0)continue;
            var enemy=group.enemyPrefab.GetComponent<Enemy>();if(enemy==null||enemy.enemyData==null)continue;
            var data=enemy.enemyData;
            text.Append("<b>").Append(data.enemyName).Append(" × ").Append(group.enemyCount).AppendLine("</b>");
            text.Append(UİManager.ElementLabel(data.element)).Append(" · ").Append(data.movementType==EnemyData.MovementType.Air?"Hava":"Kara").Append(" · ").Append(data.health.ToString("0")).AppendLine(" can");
            if(data.element!=ElementType.Normal)text.Append("Zayıf: ").Append(UİManager.ElementLabel(data.WeakTo)).Append(" ×").Append(data.weaknessMultiplier.ToString("0.##")).Append(" | Direnç: ").Append(UİManager.ElementLabel(data.ResistantTo)).Append(" ×").AppendLine(data.resistanceMultiplier.ToString("0.##"));
            text.Append("Normal ×").Append(data.normalAttackMultiplier.ToString("0.##"));
            if(data.isBoss)text.Append(" · BOSS: kaçarsa yenilgi!");
            text.AppendLine();
        }
        text.Append("Toplam ").Append(wave.TotalEnemies).Append(" düşman · Çantanı buna göre hazırla.");
        return text.ToString().TrimEnd();
    }
    public void OnPointerEnter(PointerEventData eventData) { hovering=true;Refresh();Update(); }
    public void OnPointerExit(PointerEventData eventData) { hovering=false;if(panel!=null)panel.SetActive(false); }
}
