using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

// Editor-only authoring convenience: the list shown under the UI edits the shared tree asset.
[CustomEditor(typeof(SkillTreePanelUI))]
public class SkillTreePanelUIEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        var root=new VisualElement();
        root.Add(new HelpBox("SkillData'yı ilgili ağacın Nodes > Skill alanına sürükle. Position düğüm merkezidir; bağlantılar ön koşullardan otomatik çizilir. Bu alanlar ortak SkillTreeData assetini düzenler.",HelpBoxMessageType.Info));
        root.Add(new PropertyField(serializedObject.FindProperty("trees")));
        var lists=new VisualElement();root.Add(lists);
        System.Action rebuild=()=>{
            lists.Clear();var ui=(SkillTreePanelUI)target;if(ui.trees==null)return;
            foreach(var tree in ui.trees){if(tree==null)continue;
                var fold=new Foldout{text=tree.title+" — SkillData sürükle",value=false};
                var so=new SerializedObject(tree);
                var field=new PropertyField(so.FindProperty("nodes"),"Düğümler");field.Bind(so);fold.Add(field);
                var error=tree.ConfigurationError();if(error!=null)fold.Add(new HelpBox(error,HelpBoxMessageType.Warning));lists.Add(fold);
            }
        };
        root.Add(new Button(rebuild){text="Ağaç Listelerini Yenile"});rebuild();
        var advanced=new Foldout{text="UI referansları",value=false};InspectorElement.FillDefaultInspector(advanced,serializedObject,this);root.Add(advanced);
        return root;
    }
}
