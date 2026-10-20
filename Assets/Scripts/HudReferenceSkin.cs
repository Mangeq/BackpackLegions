using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Reference HUD artwork with the original live UI values and speed callbacks.</summary>
public sealed class HudReferenceSkin : MonoBehaviour
{
    [SerializeField] UİManager ui;
    [SerializeField] Text goldSource,waveSource;
    [SerializeField] TMP_Text gold,wave,countdown;
    [SerializeField] TMP_Text[] speedLabels;
    public void Configure(UİManager owner,Text originalGold,Text originalWave,TMP_Text goldText,TMP_Text waveText,TMP_Text countdownText,TMP_Text[] labels)
    {ui=owner;goldSource=originalGold;waveSource=originalWave;gold=goldText;wave=waveText;countdown=countdownText;speedLabels=labels;Refresh();}
    void LateUpdate() { Refresh(); }
    public void Refresh()
    {
        if(gold!=null&&goldSource!=null)gold.text=goldSource.text;
        if(wave!=null&&waveSource!=null)wave.text=waveSource.text.Replace("WAVE: ","WAVE:\n");
        if(countdown!=null&&ui!=null&&ui.preparationCountdownText!=null)countdown.text=ui.preparationCountdownText.text;
        if(ui==null||speedLabels==null)return;
        for(int i=0;i<speedLabels.Length;i++)if(speedLabels[i]!=null)
            speedLabels[i].color=i<ui.speedHighlights.Length&&ui.speedHighlights[i]!=null&&ui.speedHighlights[i].enabled?new Color(1,.9f,.7f):new Color(.65f,.63f,.55f);
    }
}
