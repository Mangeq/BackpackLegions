using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Live wave groups displayed in the approved stone-framed artwork.</summary>
public sealed class WavePreviewSkin : MonoBehaviour
{
    [System.Serializable] public sealed class Portrait { public GameObject prefab; public Sprite sprite; }
    [System.Serializable] sealed class Row { public RectTransform root;public Image portrait;public TMP_Text name,identity,matchups,normal; }
    public List<Portrait> portraits=new List<Portrait>();
    [SerializeField] WavePreviewUI owner;
    [SerializeField] RectTransform panel;
    [SerializeField] List<Row> rows=new List<Row>();
    [SerializeField] TMP_FontAsset body,serif;
    [SerializeField] TMP_Text footer,empty;
    [SerializeField] Image bodyCover;
    [SerializeField] Sprite rowFace;
    const string Path="UI/WaveReference/";
    static readonly Color Cream=new Color(1,.91f,.72f);
    public static string Number(float value)=>value.ToString("0.##",CultureInfo.GetCultureInfo("tr-TR"));
    public void Configure(WavePreviewUI view)
    {
        owner=view;panel=(RectTransform)view.panel.transform;
        body=GameFonts.Body;
        serif=GameFonts.Title;
        var image=panel.GetComponent<Image>();image.sprite=Resources.Load<Sprite>(Path+"Frame");image.type=Image.Type.Sliced;image.color=Color.white;image.raycastTarget=true;
        owner.details.enabled=false;
        Place(owner.title.rectTransform,panel,275,34,470,54);
        owner.title.font=serif;owner.title.fontSize=41;owner.title.color=new Color(1,.79f,.39f);owner.title.alignment=TextAlignmentOptions.Center;
        owner.title.enableAutoSizing=true;owner.title.fontSizeMin=26;owner.title.fontSizeMax=41;owner.title.raycastTarget=false;
        rowFace=Resources.Load<Sprite>(Path+"Row");
        var cover=panel.Find("BodyCover") as RectTransform;
        if(cover==null)cover=NewRect("BodyCover",panel);
        bodyCover=cover.GetComponent<Image>();if(bodyCover==null)bodyCover=cover.gameObject.AddComponent<Image>();
        bodyCover.sprite=Resources.Load<Sprite>(Path+"Body");bodyCover.color=Color.white;bodyCover.raycastTarget=false;cover.SetAsFirstSibling();
        if(footer==null)footer=Label("Summary",panel,0,0,800,42,28,true);
        if(empty==null)empty=Label("NoUpcomingWave",panel,195,125,750,80,25,false);
        panel.transform.SetAsLastSibling();
    }
    public void Render(WaveRound wave,int index,int waveCount)
    {
        if(panel==null)return;
        var groups=new List<WaveEnemyGroup>();
        if(wave!=null)foreach(var source in wave.GetGroups())
        {
            if(source==null||source.enemyPrefab==null||source.enemyCount<=0)continue;
            var enemy=source.enemyPrefab.GetComponent<Enemy>();if(enemy==null||enemy.enemyData==null)continue;
            var same=groups.Find(x=>x.enemyPrefab.GetComponent<Enemy>().enemyData==enemy.enemyData);
            if(same!=null)same.enemyCount+=source.enemyCount;
            else groups.Add(new WaveEnemyGroup{enemyPrefab=source.enemyPrefab,enemyCount=source.enemyCount});
        }
        int visible=Mathf.Max(1,groups.Count);float height=176+132*visible;
        panel.anchorMin=panel.anchorMax=panel.pivot=new Vector2(0,1);panel.anchoredPosition=new Vector2(220,-100);panel.sizeDelta=new Vector2(1024,height);
        var canvas=panel.GetComponentInParent<Canvas>(true);var area=((RectTransform)canvas.transform).rect;
        panel.localScale=Vector3.one*Mathf.Min(1,Mathf.Min((area.width-250)/1024,(area.height-130)/height));
        Place(bodyCover.rectTransform,panel,39,97,946,height-177);
        while(rows.Count<groups.Count)rows.Add(CreateRow(rows.Count));
        int total=0;
        for(int i=0;i<rows.Count;i++)
        {
            var row=rows[i];row.root.gameObject.SetActive(i<groups.Count);if(i>=groups.Count)continue;
            var group=groups[i];var data=group.enemyPrefab.GetComponent<Enemy>().enemyData;total+=group.enemyCount;
            Place(row.root,panel,60,103+i*132,914,128);
            row.name.text=data.enemyName+" × "+group.enemyCount;
            row.identity.text=Element(data.element)+" · "+(data.movementType==EnemyData.MovementType.Air?"Hava":"Kara")+" · "+Number(data.health)+" can";
            row.matchups.text="<color=#DF8174>Zayıf:</color> "+Matchup(data,true)+" | <color=#9AC58B>Direnç:</color> "+Matchup(data,false);
            row.normal.text="Normal ×"+Number(data.GetDamageMultiplier(ElementType.Normal))+(data.isBoss?" · <color=#FF8578>BOSS: kaçarsa yenilgi!</color>":"");
            var icon=portraits.Find(p=>p.prefab==group.enemyPrefab);row.portrait.sprite=icon!=null?icon.sprite:null;row.portrait.enabled=row.portrait.sprite!=null;
        }
        owner.title.text=wave==null?"SON DALGA":"SIRADAKİ DALGA "+(index+1)+" / "+waveCount;
        footer.text=wave==null?"Bu dalgadan sonra bölüm tamamlanır.":"Toplam "+total+" düşman · Çantanı buna göre hazırla.";
        Place(footer.rectTransform,panel,180,height-76,800,43);
        empty.gameObject.SetActive(groups.Count==0);empty.text=wave==null?"Sırada başka düşman yok.":"Bu dalgada düşman bulunmuyor.";
    }
    static string Matchup(EnemyData data,bool weak)
    {
        if(data.element!=ElementType.Normal)return Element(weak?data.WeakTo:data.ResistantTo)+" ×"+Number(weak?data.weaknessMultiplier:data.resistanceMultiplier);
        var values=weak?data.weaknesses:data.resistances;
        if(values==null||values.Count==0)return "Yok";
        return string.Join(", ",values.ConvertAll(x=>Element(x)))+" ×"+Number(weak?data.weaknessMultiplier:data.resistanceMultiplier);
    }
    static string Element(ElementType e)
    {
        string color=e==ElementType.Fire?"EF9A65":e==ElementType.Water?"98D8EF":e==ElementType.Earth?"C5AE7C":e==ElementType.Air?"ABD49A":"F5E4C2";
        return "<color=#"+color+">"+UİManager.ElementLabel(e)+"</color>";
    }
    Row CreateRow(int index)
    {
        var r=NewRect("EnemyRow"+index,panel);var image=r.gameObject.AddComponent<Image>();image.sprite=rowFace;image.raycastTarget=false;
        var portrait=NewRect("Portrait",r).gameObject.AddComponent<Image>();Place(portrait.rectTransform,r,8,6,115,115);portrait.preserveAspect=true;portrait.raycastTarget=false;
        return new Row{root=r,portrait=portrait,name=Label("EnemyName",r,147,0,750,37,32,true),identity=Label("Identity",r,147,36,750,30,25,false),matchups=Label("Matchups",r,147,65,750,30,24,false),normal=Label("NormalDamage",r,147,94,750,29,24,false)};
    }
    TMP_Text Label(string name,Transform parent,float x,float y,float w,float h,int size,bool bold)
    {
        var r=NewRect(name,parent);Place(r,parent,x,y,w,h);var label=r.gameObject.AddComponent<TextMeshProUGUI>();label.font=body;label.fontSize=size;label.fontStyle=bold?FontStyles.Bold:FontStyles.Normal;label.color=Cream;label.enableWordWrapping=false;label.raycastTarget=false;
        label.enableAutoSizing=true;label.fontSizeMax=size;label.fontSizeMin=size-5;label.alignment=TextAlignmentOptions.MidlineLeft;return label;
    }
    static RectTransform NewRect(string name,Transform parent){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);return r;}
    static void Place(RectTransform r,Transform parent,float x,float y,float w,float h){r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.localScale=Vector3.one;r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);}
}
