using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// One-time content setup; all balancing data remains editable in ChapterData/EnemyData.
public static class CampaignBuilder
{
    static void Folder(string path)
    {
        if(AssetDatabase.IsValidFolder(path))return;
        var parent=System.IO.Path.GetDirectoryName(path).Replace('\\','/');Folder(parent);AssetDatabase.CreateFolder(parent,System.IO.Path.GetFileName(path));
    }
    public static string Build()
    {
        if(EditorApplication.isPlaying)throw new System.Exception("Exit Play mode first.");
        if(AssetDatabase.LoadAssetAtPath<ChapterData>("Assets/Resources/Data/Chapters/Chapter01.asset")!=null)throw new System.Exception("Campaign already exists. Edit it in place.");
        var original=SceneManager.GetActiveScene();if(original.isDirty)throw new System.Exception("Save your current scene before campaign setup.");
        Folder("Assets/Resources/Data/Chapters");Folder("Assets/Resources/Data/EnemyData/Campaign");Folder("Assets/Prefabs/CampaignEnemies");
        string[] roles={"Scout","Raider","Guard","Elite","SkyWisp","Warlord"};
        string[] names={"KARAÇAM GEÇİDİ","ESKİ TAŞ KÖPRÜ","SİSLİ SIRTLAR","KÜL VADİSİ","KUZEY KALESİ"};
        ElementType[] cycle={ElementType.Water,ElementType.Fire,ElementType.Air,ElementType.Earth};
        int[] roleElements={0,1,3,0,2,1};
        var chapters=new System.Collections.Generic.List<ChapterData>();
        for(int chapter=0;chapter<5;chapter++)
        {
            string key="Chapter"+(chapter+1).ToString("00");string dataFolder="Assets/Resources/Data/EnemyData/Campaign/"+key;string prefabFolder="Assets/Prefabs/CampaignEnemies/"+key;Folder(dataFolder);Folder(prefabFolder);
            var prefabs=new GameObject[roles.Length];
            for(int r=0;r<roles.Length;r++)
            {
                var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter01/"+roles[r]+".prefab");
                if(source==null)throw new System.Exception("Missing enemy: "+roles[r]);
                var baseData=source.GetComponent<Enemy>().enemyData;var data=Object.Instantiate(baseData);
                data.name=key+"_"+roles[r];data.health=Mathf.Round(baseData.health*(1+.25f*chapter));data.damage=Mathf.RoundToInt(baseData.damage*(1+.15f*chapter));
                data.goldReward=Mathf.RoundToInt(baseData.goldReward*(1+.1f*chapter));
                data.element=cycle[(roleElements[r]+chapter)%cycle.Length];
                AssetDatabase.CreateAsset(data,dataFolder+"/"+roles[r]+".asset");
                var instance=(GameObject)PrefabUtility.InstantiatePrefab(source);instance.name=key+"_"+roles[r];instance.GetComponent<Enemy>().enemyData=data;
                prefabs[r]=PrefabUtility.SaveAsPrefabAsset(instance,prefabFolder+"/"+roles[r]+".prefab");Object.DestroyImmediate(instance);
            }
            var config=ScriptableObject.CreateInstance<ChapterData>();config.chapterNumber=chapter+1;config.chapterName=names[chapter];config.sceneName="GameScane";config.themeIndex=chapter/5;config.preparationSeconds=60+15*chapter;config.extraPreparationPerWave=5;
            int rounds=5+2*chapter,total=40+20*chapter;
            for(int wave=0;wave<rounds;wave++)
            {
                int count=total/rounds+(wave>=rounds-total%rounds?1:0);bool boss=wave==rounds-1;
                int[] kinds=boss?new[]{5,2,1}:wave==0?new[]{0}:wave==2?new[]{4}:wave%2==1?new[]{1,2,3}:new[]{0,4};
                if(wave==1)kinds=new[]{0,1};
                var round=new WaveRound();int remaining=count;
                for(int g=0;g<kinds.Length;g++)
                {
                    int amount=boss&&g==0?1:remaining/(kinds.Length-g);
                    remaining-=amount;round.groups.Add(new WaveEnemyGroup{enemyPrefab=prefabs[kinds[g]],enemyCount=amount,spawnInterval=1.25f*kinds.Length,startDelay=0});
                }
                config.waves.Add(round);
            }
            AssetDatabase.CreateAsset(config,"Assets/Resources/Data/Chapters/"+key+".asset");chapters.Add(config);
        }
        var game=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity",OpenSceneMode.Single);var waveManager=Object.FindObjectOfType<WaveManager>();waveManager.chapters=chapters;EditorUtility.SetDirty(waveManager);
        var bar=GameObject.Find("WaveBar");if(bar==null)throw new System.Exception("WaveBar missing.");
        var preview=bar.AddComponent<WavePreviewUI>();var barImage=bar.GetComponent<UnityEngine.UI.Image>();if(barImage!=null)barImage.raycastTarget=true;
        var parent=bar.GetComponentInParent<Canvas>().transform;
        var panel=new GameObject("WavePreviewPanel",typeof(RectTransform),typeof(UnityEngine.UI.Image));panel.transform.SetParent(parent,false);
        var rect=panel.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(220,-100);rect.sizeDelta=new Vector2(480,380);
        panel.GetComponent<UnityEngine.UI.Image>().color=new Color(.06f,.105f,.085f,.96f);panel.GetComponent<UnityEngine.UI.Image>().raycastTarget=true;panel.transform.SetAsFirstSibling();
        var font=TMPro.TMP_Settings.defaultFontAsset;
        System.Func<string,string,float,float,float,TMPro.TMP_Text> label=(name,text,y,height,size)=>{var go=new GameObject(name,typeof(RectTransform),typeof(TMPro.TextMeshProUGUI));go.transform.SetParent(panel.transform,false);var r=go.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(20,-y);r.sizeDelta=new Vector2(440,height);var t=go.GetComponent<TMPro.TextMeshProUGUI>();t.font=font;t.fontSize=size;t.text=text;t.color=new Color(.93f,.90f,.81f);t.raycastTarget=false;return t;};
        preview.title=label("Title","SIRADAKİ DALGA",18,35,23);preview.title.fontStyle=TMPro.FontStyles.Bold;preview.title.color=new Color(.75f,.6f,.34f);
        preview.details=label("Details","Hazırlıkta otomatik açılır. Dalga göstergesine tıklayarak kapatabilirsin.",68,295,19);preview.panel=panel;
        EditorSceneManager.MarkSceneDirty(game);EditorSceneManager.SaveScene(game);
        var menu=EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity",OpenSceneMode.Single);var manager=Object.FindObjectOfType<MainMenuManager>();var so=new SerializedObject(manager);var property=so.FindProperty("chapters");property.arraySize=5;for(int i=0;i<5;i++)property.GetArrayElementAtIndex(i).objectReferenceValue=chapters[i];so.ApplyModifiedPropertiesWithoutUndo();
        foreach(var t in Object.FindObjectsOfType<TMPro.TMP_Text>(true)){if(t.name=="InfoFootnote")t.text="Hazırlık süresi bölüm ve dalgayla artar.\nSonraki dalgayı kontrol ederek hazırlan.";if(t.text=="YAKINDA"&&t.transform.parent.name=="Tag")t.text="BÖLGE";}
        EditorSceneManager.MarkSceneDirty(menu);EditorSceneManager.SaveScene(menu);AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity",OpenSceneMode.Single);
        return "5 chapters, 45 waves, 400 enemies total; 30 elemental enemy variants; preview and selection wired.";
    }
}
