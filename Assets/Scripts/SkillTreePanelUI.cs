using UnityEngine;

// Presentation delegates unlocks and refunds to permanent progress.
public class SkillTreePanelUI : MonoBehaviour
{
    [System.Serializable] public class Branch
    {
        public string title, subtitle;
        public Color accent;
        public string[] names, descriptions;
        public Sprite[] icons;
    }
    [Header("Ağaçlar — aşağıdaki Inspector listelerine SkillData sürükle")]
    public SkillTreeData[] trees;
    private RectTransform connectionRoot;
    public GameObject modal;
    [HideInInspector] public Branch[] branches;
    public UnityEngine.UI.Image[] tabs, nodes, nodeIcons;
    public TMPro.TMP_Text[] nodeLabels;
    public TMPro.TMP_Text branchTitle, branchSubtitle, detailTitle, detailBody, detailIndex;
    public UnityEngine.UI.Image detailIcon;
    public TMPro.TMP_Text pointsLabel;
    public UnityEngine.UI.Button unlockButton, resetButton;
    public TMPro.TMP_Text unlockLabel, rulesLabel;
    public GameObject archerConnections, previewConnections;
    [HideInInspector] public Vector2[] archerPositions, previewPositions;
    private int selectedNode;
    private int selectedBranch;
    private static SkillTreePanelUI openPanel;
    private static int dismissedFrame = -1;
    public static bool IsOpen => openPanel != null && openPanel.modal.activeInHierarchy;
    public static bool BlocksWorldInput => IsOpen || dismissedFrame == Time.frameCount;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() { openPanel = null; dismissedFrame = -1; }

    public void Open()
    {
        if(SceneLoadingUI.IsLoading)return;
        var game=FindObjectOfType<GameManager>();
        if(game!=null&&!game.IsPaused&&!game.HasEnded)return;
        SettingsPanelUI.TryCloseOpen();
        modal.SetActive(true);openPanel=this;SelectBranch(selectedBranch);
        if(pointsLabel!=null)pointsLabel.text="YETENEK PUANI  " + new SkillTreeProgress().AvailablePoints;
    }
    public void SelectBranch(int index)
    {
        if(trees==null||index<0||index>=trees.Length||trees[index]==null)return;
        selectedBranch=index;var tree=trees[index];
        branchTitle.text=tree.title;branchSubtitle.text=tree.subtitle;
        for(int i=0;i<tabs.Length;i++)tabs[i].color=i==index?tree.accent:new Color(.14f,.20f,.17f);
        if(archerConnections!=null)archerConnections.SetActive(false);
        if(previewConnections!=null)previewConnections.SetActive(false);
        EnsureViews(tree.nodes.Count);
        int first=-1;
        for(int i=0;i<nodes.Length;i++){
            var slot=i<tree.nodes.Count?tree.nodes[i]:null;var skill=slot!=null?slot.skill:null;
            nodes[i].gameObject.SetActive(skill!=null);
            if(skill==null)continue;
            if(first<0)first=i;
            nodes[i].rectTransform.anchoredPosition=new Vector2(300+slot.position.x-73,-280-slot.position.y+40);
            nodeLabels[i].text=skill.displayName;nodeIcons[i].sprite=skill.icon;
            var button=nodes[i].GetComponent<UnityEngine.UI.Button>();button.onClick=new UnityEngine.UI.Button.ButtonClickedEvent();int nodeIndex=i;button.onClick.AddListener(()=>SelectNode(nodeIndex));
        }
        var reference=GetComponent<SkillTreeReferenceSkin>();
        if(reference!=null)reference.LayoutBranch(tree,index);else DrawConnections(tree);
        if(resetButton!=null)resetButton.gameObject.SetActive(true);
        if(first>=0)SelectNode(first);
        else {selectedNode=-1;detailTitle.text="Ağaç boş";detailBody.text="Inspector'da bu ağaca SkillData ekle.";detailIndex.text="";detailIcon.sprite=null;unlockButton.interactable=false;unlockLabel.text="YETENEK YOK";rulesLabel.text="";}
    }
    private void EnsureViews(int count)
    {
        int previous=nodes.Length;if(count<=previous)return;
        System.Array.Resize(ref nodes,count);System.Array.Resize(ref nodeIcons,count);System.Array.Resize(ref nodeLabels,count);
        for(int i=previous;i<count;i++){
            var go=Instantiate(nodes[0].gameObject,nodes[0].transform.parent);go.name="DataNode"+i;
            nodes[i]=go.GetComponent<UnityEngine.UI.Image>();nodeIcons[i]=go.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();nodeLabels[i]=go.transform.Find("Name").GetComponent<TMPro.TMP_Text>();
        }
    }
    public void SelectNode(int index)
    {
        var tree=trees[selectedBranch];if(index<0||index>=tree.nodes.Count||tree.nodes[index]==null||tree.nodes[index].skill==null)return;
        selectedNode=index;var skill=tree.nodes[index].skill;var progress=new SkillTreeProgress();
        detailTitle.text=skill.displayName;detailBody.text=skill.description;detailIcon.sprite=skill.icon;
        bool unlocked=progress.IsUnlocked(skill.id);string error=progress.UnlockError(tree,skill);
        detailIndex.text="DÜĞÜM "+(index+1).ToString("00")+(!skill.available?" / TASLAK":unlocked?" / AÇIK":" / "+skill.pointCost+" PUAN");
        for(int i=0;i<nodes.Length;i++){
            var entry=i<tree.nodes.Count?tree.nodes[i]:null;bool owned=entry!=null&&entry.skill!=null&&progress.IsUnlocked(entry.skill.id);
            nodes[i].color=i==index?tree.accent:owned?new Color(.33f,.37f,.22f):new Color(.18f,.25f,.21f);
        }
        if(pointsLabel!=null)pointsLabel.text="YETENEK PUANI  "+progress.AvailablePoints;
        if(unlockButton!=null)unlockButton.interactable=error==null;
        if(unlockLabel!=null)unlockLabel.text=error==null?"AÇ · "+skill.pointCost+" PUAN":error=="TASLAK"?"HENÜZ ETKİN DEĞİL":error=="AÇILDI"||error=="YETERSİZ PUAN"||error=="ÖNCE ÖN KOŞULU AÇ"?error:"DATA AYARINI KONTROL ET";
        if(rulesLabel!=null){var names=new System.Collections.Generic.List<string>();foreach(var parent in skill.prerequisites)if(parent!=null)names.Add(parent.displayName);rulesLabel.text=tree.ConfigurationError()??(names.Count==0?"Başlangıç yeteneği":"Ön koşul: "+string.Join(", ",names));}
        var reference=GetComponent<SkillTreeReferenceSkin>();if(reference!=null)reference.RefreshSelection(tree,index);
    }
    public void UnlockSelected()
    {
        if(!IsOpen||selectedNode<0)return;var tree=trees[selectedBranch];
        if(selectedNode>=tree.nodes.Count)return;
        new SkillTreeProgress().TryUnlock(tree,tree.nodes[selectedNode].skill);SelectNode(selectedNode);
    }
    public void ResetSelectedTree()
    {
        if(!IsOpen)return;new SkillTreeProgress().ResetTree(trees[selectedBranch].towerType);
        if(selectedNode>=0)SelectNode(selectedNode);
        if(pointsLabel!=null)pointsLabel.text="YETENEK PUANI  "+new SkillTreeProgress().AvailablePoints;
    }
    private void DrawConnections(SkillTreeData tree)
    {
        if(connectionRoot==null){var go=new GameObject("DataConnections",typeof(RectTransform));connectionRoot=go.GetComponent<RectTransform>();connectionRoot.SetParent(nodes[0].transform.parent,false);connectionRoot.anchorMin=connectionRoot.anchorMax=connectionRoot.pivot=new Vector2(0,1);connectionRoot.anchoredPosition=Vector2.zero;connectionRoot.SetSiblingIndex(nodes[0].transform.GetSiblingIndex());}
        foreach(Transform child in connectionRoot){child.gameObject.SetActive(false);Destroy(child.gameObject);}
        foreach(var slot in tree.nodes){if(slot==null||slot.skill==null)continue;foreach(var parent in slot.skill.prerequisites){var parentSlot=tree.nodes.Find(n=>n!=null&&n.skill==parent);if(parentSlot==null||parent==null)continue;
            Vector2 a=parentSlot.position+new Vector2(300,280),d=slot.position+new Vector2(300,280),b=a+new Vector2(0,-70),c=d+new Vector2(0,65),previous=a;
            for(int j=1;j<=16;j++){float t=j/16f,u=1-t;Vector2 next=u*u*u*a+3*u*u*t*b+3*u*t*t*c+t*t*t*d;
                var go=new GameObject("Branch",typeof(RectTransform),typeof(UnityEngine.UI.Image));var r=go.GetComponent<RectTransform>();r.SetParent(connectionRoot,false);r.anchorMin=r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,.5f);r.anchoredPosition=new Vector2(previous.x,-previous.y);r.sizeDelta=new Vector2(Vector2.Distance(previous,next)+1,3);r.localRotation=Quaternion.Euler(0,0,-Mathf.Atan2(next.y-previous.y,next.x-previous.x)*Mathf.Rad2Deg);var image=go.GetComponent<UnityEngine.UI.Image>();image.color=new Color(.58f,.49f,.32f,.85f);image.raycastTarget=false;previous=next;
            }
        }}
    }
    public void Close() { modal.SetActive(false);if(openPanel==this)openPanel=null;dismissedFrame=Time.frameCount; }
    public static bool TryCloseOpen() { if(!IsOpen)return false;openPanel.Close();return true; }
    private void OnDestroy() { if(openPanel==this)openPanel=null; }
}
