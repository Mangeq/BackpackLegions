if(!UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Play Mode required");
var rows=new System.Collections.Generic.List<object>();
var ui=UnityEngine.Object.FindObjectOfType<UİManager>();
var gm=UnityEngine.Object.FindObjectOfType<GameManager>();gm.enabled=false;
var buy=ui.purchasePrice.transform.parent.GetComponent<UnityEngine.UI.Button>();
var highlights=new[]{ui.magicSelectionHighlight,ui.cannonSelectionHighlight,ui.archerSelectionHighlight,ui.unitSelectionHighlight};
var tabs=highlights.Select(x=>x.transform.parent.GetComponent<UnityEngine.UI.Button>()).ToArray();
var prefabs=new[]{ui.MegicTowerPrefab,ui.MortarTowerPrefab,ui.ArcherTowerPrefab,ui.UnitTowerPrefab};
var names=new[]{"BÜYÜ KULESİ","HAVAN KULESİ","OKÇU KULESİ","BİRLİK KULESİ"};
EconomyManager.Gold=3000;UİManager.UpdateGoldText();
ui.ClosePurchasePanel();
var slot=UnityEngine.Object.FindObjectsOfType<TowerSlot>().First(x=>!x.isOccupied);slot.SendMessage("OnMouseDown");
for(int i=0;i<4;i++)
{
    tabs[i].onClick.Invoke();ui.GetComponent<TowerPurchaseSkin>().SendMessage("LateUpdate");
    var tower=prefabs[i].GetComponent<Tower>();var d=tower.data;var weapon=d.defaultWeapon;
    float damage=d.damage*(1+(weapon!=null?weapon.damageBonus:0));
    float interval=Mathf.Max(.1f,d.attackSpeed/(1+(weapon!=null?weapon.attackSpeedBonus:0)));
    string[] values=ui.purchaseStatValues.text.Split('\n');
    bool correct=ui.purchaseTitle.text==names[i]&&ui.purchasePrice.text.Contains(d.cost.ToString())&&
        values[0]==damage.ToString("0.##")&&values[1]==interval.ToString("0.##")+" sn"&&values.Last()==d.gridWidth+" × "+d.gridHeight;
    if(i==1)correct&=ui.purchaseStats.text.Contains("PATLAMA YARIÇAPI")&&ui.purchaseDescription.text.Contains("Alan hasarı");
    if(i==3)correct&=ui.purchaseStats.text.Contains("BİRLİK SAYISI")&&ui.purchaseStats.text.Contains("YENİDEN ÜRETİM");
    rows.Add(new{test=names[i]+" uses its own data and the shared reference skin",passed=correct&&ui.purchasePortrait.enabled});
}
rows.Add(new{test="original selection and buy callbacks retained",passed=buy.onClick.GetPersistentEventCount()>0&&tabs.All(x=>x.onClick.GetPersistentEventCount()>0)});
var card=(RectTransform)ui.purchaseDetails.transform;
var parent=(RectTransform)UİManager.TowerShopUI.transform.parent;
var corners=new Vector3[4];card.GetWorldCorners(corners);
rows.Add(new{test="card fits within the screen",passed=corners.All(p=>parent.rect.Contains(parent.InverseTransformPoint(p)))});
EconomyManager.Gold=0;tabs[3].onClick.Invoke();int count=UnityEngine.Object.FindObjectsOfType<Tower>().Length;
buy.onClick.Invoke();
rows.Add(new{test="insufficient gold disables purchase and cannot create a tower",passed=!buy.interactable&&UnityEngine.Object.FindObjectsOfType<Tower>().Length==count&&!slot.isOccupied});
ui.purchaseDetails.transform.Find("CloseCard").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
rows.Add(new{test="reference close button releases the selected slot",passed=!UİManager.TowerShopUI.activeInHierarchy&&UİManager.selectedTowerSlot==null&&slot.GetComponent<Collider>().enabled});
EconomyManager.Gold=3000;UİManager.UpdateGoldText();
for(int i=0;i<4;i++)
{
    slot=UnityEngine.Object.FindObjectsOfType<TowerSlot>().First(x=>!x.isOccupied);
    slot.SendMessage("OnMouseDown");tabs[i].onClick.Invoke();
    int before=EconomyManager.Gold;count=UnityEngine.Object.FindObjectsOfType<Tower>().Length;
    int cost=prefabs[i].GetComponent<Tower>().data.cost;
    buy.onClick.Invoke();
    rows.Add(new{test=names[i]+" button buys exactly one tower at its data price",passed=slot.isOccupied&&EconomyManager.Gold==before-cost&&UnityEngine.Object.FindObjectsOfType<Tower>().Length==count+1});
}
return rows;
