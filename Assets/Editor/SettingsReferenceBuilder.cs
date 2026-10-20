using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;

public static class SettingsReferenceBuilder
{
    const string Folder="Assets/Resources/UI/SettingsReference/";
    public static Sprite Art(string name) { return Resources.Load<Sprite>("UI/SettingsReference/"+name); }
    static void Crop(string name,Rect bounds)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;var t=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"SettingsEmpty.png");
        var sprite=Sprite.Create(t,new Rect(bounds.x/1024*t.width,(874-bounds.y-bounds.height)/874*t.height,bounds.width/1024*t.width,bounds.height/874*t.height),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    static void Alpha(string name,int half)
    {
        if(AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset")!=null)return;var t=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"SettingsParts.png");var pixels=t.GetPixels32();int start=half*t.width/2,end=(half+1)*t.width/2,l=end,r=start,b=t.height,top=0;
        for(int y=0;y<t.height;y++)for(int x=start;x<end;x++)if(pixels[y*t.width+x].a>25){l=Mathf.Min(l,x);r=Mathf.Max(r,x);b=Mathf.Min(b,y);top=Mathf.Max(top,y);}
        if(r<=l||top<=b)throw new InvalidOperationException("Missing settings sprite: "+name);
        var sprite=Sprite.Create(t,new Rect(l,b,r-l+1,top-b+1),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");
    }
    static void Place(RectTransform r,Transform parent,float x,float y,float w,float h)
    {r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);r.localScale=Vector3.one;r.localRotation=Quaternion.identity;}
    static RectTransform Rect(string name,Transform parent,float x,float y,float w,float h)
    {var r=parent.Find(name) as RectTransform;if(r==null)r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();Place(r,parent,x,y,w,h);return r;}
    static Image Picture(string name,Transform p,string art,float x,float y,float w,float h,bool ray=false)
    {var r=Rect(name,p,x,y,w,h);var i=r.GetComponent<Image>();if(i==null)i=r.gameObject.AddComponent<Image>();i.sprite=Art(art);i.type=Image.Type.Simple;i.enabled=true;i.color=Color.white;i.raycastTarget=ray;return i;}
    static TMP_Text Label(TMP_Text text,string name,Transform parent,float x,float y,float w,float h,int size)
    {
        if(text==null){var r=Rect(name,parent,x,y,w,h);text=r.GetComponent<TMP_Text>();if(text==null)text=r.gameObject.AddComponent<TextMeshProUGUI>();}
        Place(text.rectTransform,parent,x,y,w,h);text.font=GameFonts.Body;text.color=new Color(1,.9f,.74f);text.fontSize=size;text.enableAutoSizing=true;text.fontSizeMin=12;text.fontSizeMax=size;text.enableWordWrapping=false;text.alignment=TextAlignmentOptions.Center;text.enabled=true;text.raycastTarget=false;return text;
    }
    static Button StyleButton(Button b,RectTransform card,string sprite,Rect rect)
    {
        Place((RectTransform)b.transform,card,rect.x,rect.y,rect.width,rect.height);var image=b.GetComponent<Image>();if(image==null)image=b.gameObject.AddComponent<Image>();image.sprite=Art(sprite);image.enabled=true;image.type=Image.Type.Simple;image.color=Color.white;image.raycastTarget=true;b.targetGraphic=image;
        var colors=ColorBlock.defaultColorBlock;colors.highlightedColor=new Color(1.1f,1.1f,1.1f);colors.selectedColor=Color.white;colors.disabledColor=new Color(.4f,.4f,.4f);b.colors=colors;return b;
    }
    static Slider Slider(Slider slider,string name,RectTransform card,float y)
    {
        if(slider==null){var r=Rect(name,card,442,y,402,36);slider=r.gameObject.AddComponent<Slider>();}else Place((RectTransform)slider.transform,card,442,y,402,36);
        slider.minValue=0;slider.maxValue=1;slider.wholeNumbers=false;slider.direction=UnityEngine.UI.Slider.Direction.LeftToRight;slider.fillRect=null;
        var track=slider.GetComponent<Image>();if(track==null)track=slider.gameObject.AddComponent<Image>();track.sprite=Art("Track");track.color=Color.white;track.enabled=true;track.raycastTarget=true;
        var area=Rect("ReferenceHandleArea",slider.transform,16,0,370,36);var knob=Picture("Knob",area,"Knob",0,0,31,31,true);knob.preserveAspect=true;var k=knob.rectTransform;k.anchorMin=k.anchorMax=k.pivot=Vector2.one*.5f;k.anchoredPosition=Vector2.zero;
        slider.handleRect=k;slider.targetGraphic=knob;slider.transition=Selectable.Transition.ColorTint;return slider;
    }
    static void Toggle(Toggle toggle,RectTransform card,float y)
    {
        Place((RectTransform)toggle.transform,card,922,y,40,40);var image=toggle.GetComponent<Image>();if(image==null)image=toggle.gameObject.AddComponent<Image>();image.sprite=Art("Unchecked");image.enabled=true;image.color=Color.white;image.raycastTarget=true;toggle.targetGraphic=image;
        var tick=Picture("ReferenceTick",toggle.transform,"Check",7,6,27,28,false);toggle.graphic=tick;toggle.toggleTransition=UnityEngine.UI.Toggle.ToggleTransition.None;
    }
    static void Field(SettingsPanelUI owner,string name,UnityEngine.Object value)
    {var so=new SerializedObject(owner);so.FindProperty(name).objectReferenceValue=value;so.ApplyModifiedPropertiesWithoutUndo();}
    static T Read<T>(SettingsPanelUI ui,string name) where T:UnityEngine.Object { return (T)new SerializedObject(ui).FindProperty(name).objectReferenceValue; }
    static void Listen(UnityEngine.Events.UnityEventBase ev,Action add,string method)
    {for(int i=0;i<ev.GetPersistentEventCount();i++)if(ev.GetPersistentMethodName(i)==method)return;add();}
    static void Configure(SettingsPanelUI ui)
    {
        var modal=Read<GameObject>(ui,"modal");var oldButtons=modal.GetComponentsInChildren<Button>(true);
        foreach(var graphic in modal.GetComponentsInChildren<Graphic>(true))graphic.enabled=false;
        foreach(var effect in modal.GetComponentsInChildren<Shadow>(true))effect.enabled=false;
        var dim=modal.transform.Find("DismissBackdrop").GetComponent<Image>();dim.enabled=true;dim.color=new Color(0,0,0,.58f);dim.raycastTarget=true;
        var card=Rect("ReferenceSettingsCard",modal.transform,0,0,1024,874);card.anchorMin=card.anchorMax=card.pivot=Vector2.one*.5f;card.anchoredPosition=Vector2.zero;card.SetAsLastSibling();var face=card.GetComponent<Image>();if(face==null)face=card.gameObject.AddComponent<Image>();face.sprite=Art("SettingsBackground");face.enabled=true;face.color=Color.white;face.raycastTarget=true;
        string[] names={"Close","Cancel","Apply","ResolutionPrevious","ResolutionNext","QualityPrevious","QualityNext"};string[] sprites={"Close","Cancel","Apply","Previous","Next","Previous","Next"};var positions=new[]{new Rect(901,42,66,66),new Rect(447,755,233,73),new Rect(702,755,260,73),new Rect(448,418,59,56),new Rect(904,418,59,56),new Rect(448,489,59,56),new Rect(904,489,59,56)};
        for(int i=0;i<names.Length;i++)StyleButton(oldButtons.First(b=>b.name==names[i]),card,sprites[i],positions[i]);
        var volume=Slider(Read<Slider>(ui,"volume"),"MasterVolume",card,299);Field(ui,"volumeText",Label(Read<TMP_Text>(ui,"volumeText"),"VolumeValue",card,852,298,83,38,27));
        var effects=Slider(Read<Slider>(ui,"effects"),"EffectsVolume",card,341);Field(ui,"effects",effects);Field(ui,"effectsText",Label(Read<TMP_Text>(ui,"effectsText"),"EffectsValue",card,852,340,83,38,27));
        Listen(effects.onValueChanged,()=>UnityEventTools.AddVoidPersistentListener(effects.onValueChanged,ui.RefreshLabels),"RefreshLabels");
        Label(Read<TMP_Text>(ui,"resolutionText"),"ResolutionValue",card,528,421,353,47,27);Label(Read<TMP_Text>(ui,"qualityText"),"QualityValue",card,528,492,353,47,26);
        var fps=Label(Read<TMP_Text>(ui,"fpsText"),"FPSValue",card,528,562,353,47,26);Field(ui,"fpsText",fps);
        for(int i=0;i<2;i++)
        {string name=i==0?"FPSPrevious":"FPSNext";var r=Rect(name,card,i==0?448:904,559,59,56);var b=r.GetComponent<Button>();if(b==null)b=r.gameObject.AddComponent<Button>();StyleButton(b,card,i==0?"Previous":"Next",new Rect(i==0?448:904,559,59,56));if(b.onClick.GetPersistentEventCount()==0)UnityEventTools.AddIntPersistentListener(b.onClick,ui.ChangeFPS,i==0?-1:1);Field(ui,i==0?"fpsPrevious":"fpsNext",b);}
        var full=Read<Toggle>(ui,"fullscreen");Toggle(full,card,621);var vsync=Read<Toggle>(ui,"vsync");Toggle(vsync,card,662);Listen(vsync.onValueChanged,()=>UnityEventTools.AddVoidPersistentListener(vsync.onValueChanged,ui.RefreshLabels),"RefreshLabels");
        // Language is intentionally presentation-only until localization is implemented.
        Picture("LanguagePreviousDisabled",card,"LanguagePrevious",560,146,58,56).color=new Color(.43f,.43f,.43f);
        Picture("LanguageNextDisabled",card,"LanguageNext",905,146,58,56).color=new Color(.43f,.43f,.43f);
        var skin=ui.GetComponent<SettingsReferenceSkin>();if(skin==null)skin=ui.gameObject.AddComponent<SettingsReferenceSkin>();skin.Configure(card);modal.SetActive(false);
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Settings")]
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play first.");
        foreach(var name in new[]{"SettingsEmpty","SettingsParts"}){var path=Folder+name+".png";AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=name=="SettingsParts";importer.isReadable=name=="SettingsParts";importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();}
        Crop("SettingsBackground",new Rect(0,0,1024,874));Crop("Close",new Rect(901,42,66,66));Crop("Cancel",new Rect(447,755,233,73));Crop("Apply",new Rect(702,755,260,73));Crop("Previous",new Rect(448,418,59,56));Crop("Next",new Rect(904,418,59,56));Crop("Track",new Rect(442,299,402,36));Crop("Unchecked",new Rect(922,621,40,40));Crop("LanguagePrevious",new Rect(560,146,58,56));Crop("LanguageNext",new Rect(905,146,58,56));Alpha("Knob",0);Alpha("Check",1);
        const string prefab="Assets/Prefabs/UI/SettingsUI.prefab";var root=PrefabUtility.LoadPrefabContents(prefab);try{Configure(root.GetComponent<SettingsPanelUI>());PrefabUtility.SaveAsPrefabAsset(root,prefab);}finally{PrefabUtility.UnloadPrefabContents(root);}
        foreach(var path in new[]{"Assets/Scenes/MainMenu.unity","Assets/Scenes/GameScane.unity"})
        {
            var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath(path);bool loaded=scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
            try{foreach(var ui in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<SettingsPanelUI>(true)))Configure(ui);foreach(var component in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);}
            finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
        }
        AssetDatabase.SaveAssets();return "Reference settings applied to shared prefab, menu and gameplay";
    }
}
