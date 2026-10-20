using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>The reference campaign map driven by the existing chapter and save data.</summary>
public sealed class ChapterReferenceSkin : MonoBehaviour
{
    [SerializeField] RectTransform artwork;
    [SerializeField] ChapterData[] chapters;
    [SerializeField] Image[] markers;
    [SerializeField] TMP_Text[] numbers, names, rows;
    [SerializeField] Image[] mapStars, detailStars;
    [SerializeField] TMP_Text chapterNumber, chapterName, totalStars, points;
    [SerializeField] Button play;
    [SerializeField] Sprite node, selectedNode;
    int selected;
    public static Sprite Art(string name) { return Resources.Load<Sprite>("UI/ChapterReference/"+name); }
    public void Configure(RectTransform stage,ChapterData[] data,Image[] buttons,TMP_Text[] digits,TMP_Text[] labels,TMP_Text heading,TMP_Text title,TMP_Text[] details,Image[] stars,Image[] largeStars,TMP_Text total,TMP_Text available,Button playButton)
    {
        artwork=stage;chapters=data;markers=buttons;numbers=digits;names=labels;chapterNumber=heading;chapterName=title;rows=details;mapStars=stars;detailStars=largeStars;totalStars=total;points=available;play=playButton;node=Art("ChapterNode");selectedNode=Art("SelectedNode");Refresh(0,data);Fit();
    }
    void OnEnable() { SkillTreeProgress.Changed+=ProgressChanged; if(chapters!=null&&chapters.Length>0)Refresh(selected,chapters); }
    void OnDisable() { SkillTreeProgress.Changed-=ProgressChanged; }
    void ProgressChanged() { Refresh(selected,chapters); }
    public void Refresh(int index,ChapterData[] data)
    {
        if(data==null||data.Length==0||markers==null||markers.Length==0)return;
        chapters=data;selected=Mathf.Clamp(index,0,data.Length-1);var save=new SkillTreeProgress();int earned=0,count=0;
        for(int i=0;i<markers.Length;i++)
        {
            bool available=i<data.Length&&data[i]!=null;markers[i].gameObject.SetActive(available);names[i].transform.parent.gameObject.SetActive(available);
            for(int s=0;s<3;s++)mapStars[i*3+s].gameObject.SetActive(available);
            if(!available)continue;
            count++;int best=Mathf.Clamp(save.BestPoints(data[i].chapterNumber),0,3);earned+=best;
            markers[i].sprite=i==selected?selectedNode:node;markers[i].color=Color.white;
            numbers[i].text=data[i].chapterNumber.ToString();numbers[i].color=Color.white;
            names[i].text=data[i].chapterNumber+" · "+data[i].chapterName;
            for(int s=0;s<3;s++)Star(mapStars[i*3+s],s<best);
        }
        totalStars.text=earned+"/"+(count*3);points.text=save.AvailablePoints+" YP";
        var chapter=data[selected];play.interactable=chapter!=null;
        if(chapter==null)return;
        chapterNumber.text="BÖLÜM "+chapter.chapterNumber;chapterName.text=chapter.chapterName;
        bool ground=false,air=false;
        foreach(var wave in chapter.waves)if(wave!=null)foreach(var group in wave.GetGroups())
        {if(group==null||group.enemyPrefab==null||group.enemyCount<=0)continue;var enemy=group.enemyPrefab.GetComponent<Enemy>();if(enemy==null||enemy.enemyData==null)continue;if(enemy.enemyData.movementType==EnemyData.MovementType.Air)air=true;else ground=true;}
        string movement=ground&&air?"Kara ve hava":air?"Hava":"Kara";
        rows[0].text="DALGA: "+chapter.waves.Count;
        rows[1].text="DÜŞMAN: "+chapter.TotalEnemies+" · "+movement;
        rows[2].text="HAZIRLIK: "+chapter.preparationSeconds.ToString("0")+" sn · +"+chapter.extraPreparationPerWave.ToString("0")+" sn/dalga";
        rows[3].text="BAŞLANGIÇ: "+chapter.startingGold+" altın";
        rows[4].text="EN İYİ SONUÇ: "+save.BestPoints(chapter.chapterNumber)+"/3 yıldız";
        for(int s=0;s<detailStars.Length;s++)Star(detailStars[s],s<save.BestPoints(chapter.chapterNumber));
    }
    static void Star(Image image,bool earned) { image.color=earned?new Color(1,.85f,.34f):new Color(.24f,.23f,.21f); }
    void LateUpdate() { Fit(); }
    void Fit()
    {
        if(artwork==null||artwork.parent==null)return;
        var area=((RectTransform)artwork.parent).rect;float scale=Mathf.Min(area.width/1024f,area.height/572f);artwork.localScale=new Vector3(scale,scale,1);artwork.anchoredPosition=Vector2.zero;
    }
}
