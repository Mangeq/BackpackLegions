using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WavePreviewReferenceBuilder
{
    const string Folder="Assets/Resources/UI/WaveReference/";
    static Texture2D Import(string file)
    {
        AssetDatabase.ImportAsset(Folder+file+".png");var importer=(TextureImporter)AssetImporter.GetAtPath(Folder+file+".png");
        importer.textureType=TextureImporterType.Default;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.alphaIsTransparency=true;importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+file+".png");
    }
    static Sprite Piece(string name,Rect rect,Vector4 border)
    {
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Folder+name+".asset");if(sprite!=null)return sprite;
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"Panel.png");float sx=texture.width/1024f,sy=texture.height/572f;
        sprite=Sprite.Create(texture,new Rect(rect.x*sx,(572-rect.y-rect.height)*sy,rect.width*sx,rect.height*sy),Vector2.one*.5f,100*sx,0,SpriteMeshType.FullRect,new Vector4(border.x*sx,border.y*sy,border.z*sx,border.w*sy));
        sprite.name=name;AssetDatabase.CreateAsset(sprite,Folder+name+".asset");return sprite;
    }
    static Sprite Portrait(GameObject prefab)
    {
        string key=AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));string path=Folder+"Portrait_"+key;
        var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path+".asset");if(sprite!=null)return sprite;
        var preview=new PreviewRenderUtility();
        try
        {
            var clone=UnityEngine.Object.Instantiate(prefab);preview.AddSingleGO(clone);clone.transform.position=Vector3.zero;
            foreach(var canvas in clone.GetComponentsInChildren<Canvas>())canvas.enabled=false;
            var renderers=clone.GetComponentsInChildren<Renderer>().Where(r=>r is MeshRenderer||r is SkinnedMeshRenderer).ToArray();
            if(renderers.Length==0)throw new InvalidOperationException("Enemy has no portrait geometry: "+prefab.name);
            var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
            var data=prefab.GetComponent<Enemy>().enemyData;
            if(data.element!=ElementType.Normal)
            {
                var tint=data.element==ElementType.Fire?new Color(1,.35f,.16f):data.element==ElementType.Water?new Color(.2f,.65f,1):data.element==ElementType.Earth?new Color(.65f,.48f,.23f):new Color(.55f,.90f,.77f);
                foreach(var renderer in renderers){var material=renderer.sharedMaterial;if(material==null)continue;var block=new MaterialPropertyBlock();foreach(var property in new[]{"_BaseColor","_Color"})if(material.HasProperty(property))block.SetColor(property,Color.Lerp(material.GetColor(property),tint,.38f));renderer.SetPropertyBlock(block);}
            }
            float radius=Mathf.Max(.1f,Mathf.Max(bounds.extents.x,Mathf.Max(bounds.extents.y,bounds.extents.z)));
            preview.camera.orthographic=true;preview.camera.orthographicSize=radius*1.18f;
            preview.camera.transform.position=bounds.center+new Vector3(.8f,.3f,3)*radius;preview.camera.transform.LookAt(bounds.center);
            preview.camera.nearClipPlane=.01f;preview.camera.farClipPlane=radius*12+10;preview.camera.clearFlags=CameraClearFlags.SolidColor;preview.camera.backgroundColor=Color.clear;
            preview.lights[0].intensity=1.3f;preview.lights[0].transform.rotation=Quaternion.Euler(30,200,0);preview.lights[1].intensity=.8f;preview.ambientColor=new Color(.6f,.6f,.6f);
            preview.BeginStaticPreview(new Rect(0,0,256,256));preview.camera.Render();var texture=preview.EndStaticPreview();
            File.WriteAllBytes(path+".png",texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);
        }
        finally{preview.Cleanup();}
        var portraitTexture=Import("Portrait_"+key);sprite=Sprite.Create(portraitTexture,new Rect(0,0,256,256),Vector2.one*.5f);sprite.name=prefab.name+"Portrait";AssetDatabase.CreateAsset(sprite,path+".asset");return sprite;
    }
    [MenuItem("Tools/Backpack Defense/Apply Reference Wave Preview")]
    public static void Apply()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play Mode first.");
        var scene=EditorSceneManager.GetActiveScene();if(scene.isDirty&&!EditorSceneManager.SaveScene(scene))throw new IOException("Cannot save scene.");
        if(scene.path!="Assets/Scenes/GameScane.unity")scene=EditorSceneManager.OpenScene("Assets/Scenes/GameScane.unity");
        var view=UnityEngine.Object.FindObjectOfType<WavePreviewUI>(true);if(view==null)throw new InvalidOperationException("Wave preview is missing.");
        Import("Panel");Piece("Frame",new Rect(0,0,1024,572),new Vector4(26,80,26,96));Piece("Row",new Rect(60,102,914,128),Vector4.zero);Piece("Body",new Rect(37,65,16,20),Vector4.zero);
        var skin=view.GetComponent<WavePreviewSkin>();if(skin==null)skin=view.gameObject.AddComponent<WavePreviewSkin>();
        var waves=UnityEngine.Object.FindObjectOfType<WaveManager>(true);
        var prefabs=waves.chapters.Where(c=>c!=null).SelectMany(c=>c.waves).Where(w=>w!=null).SelectMany(w=>w.GetGroups()).Where(g=>g!=null&&g.enemyPrefab!=null).Select(g=>g.enemyPrefab).Distinct().ToArray();
        skin.portraits.Clear();foreach(var prefab in prefabs)skin.portraits.Add(new WavePreviewSkin.Portrait{prefab=prefab,sprite=Portrait(prefab)});
        skin.Configure(view);var first=waves.chapters[0].waves;skin.Render(first[0],0,first.Count);view.panel.SetActive(false);
        foreach(var component in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Component>(true)))if(component!=null)EditorUtility.SetDirty(component);
        AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
    }
}
