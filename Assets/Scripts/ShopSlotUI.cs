using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler
{
    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData e)
    {
        if(e.button!=UnityEngine.EventSystems.PointerEventData.InputButton.Right||BackpackDraggable.IsDragging)return;
        var shop=FindObjectOfType<ShopUIManager>(true);var tower=UİManager.activeTower;
        if(shop==null||tower==null)return;
        tower.ToggleOfferReservation(System.Array.IndexOf(shop.shopSlots,this));
        shop.UpdateShopUI();
    }
    public void ShowReservation(bool reserved)
    {
        var badge=transform.Find("ReservationBadge");
        if(badge==null)
        {
            var go=new GameObject("ReservationBadge",typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(transform,false);
            var r=(RectTransform)go.transform;r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(10,-172);r.sizeDelta=new Vector2(115,18);
            var t=go.GetComponent<TextMeshProUGUI>();var name=transform.Find("ReferenceName");
            if(name!=null)t.font=name.GetComponent<TMP_Text>().font;
            t.fontSize=13;t.fontStyle=FontStyles.Bold;t.color=new Color(1,.77f,.3f);t.alignment=TextAlignmentOptions.TopLeft;t.raycastTarget=false;t.text="REZERVE";badge=go.transform;
        }
        badge.gameObject.SetActive(reserved);
    }
    public ItemData currentItem;
    public Image image;
    public GameObject synergyGlowVisual;
    public Text itemCost;
    public bool isPurchased = false;
    public Text itemName;
    public Text footprintText;
    public RectTransform footprint;

    public void RefreshCard()
    {
        var item = currentItem;
        if (itemName != null) itemName.text = item != null ? item.Name : "Alındı";
        if (footprintText != null) footprintText.text = item != null
            ? ItemShapeUtility.GetBoundingSize(item.itemShape).x + "×" + ItemShapeUtility.GetBoundingSize(item.itemShape).y + " · " + item.itemShape.Count + " hücre" : "";
        if (footprint == null) return;
        foreach (Transform child in footprint) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
        if (item == null) return;
        var size = ItemShapeUtility.GetBoundingSize(item.itemShape);
        float unit = Mathf.Min(18f, Mathf.Min(footprint.rect.width / size.x, footprint.rect.height / size.y));
        foreach (var cell in item.itemShape)
        {
            var go = new GameObject("Cell", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            go.transform.SetParent(footprint, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = Vector2.one * (unit - 2f);
            rt.anchoredPosition = new Vector2(cell.x * unit, -cell.y * unit);
            var img = go.GetComponent<UnityEngine.UI.Image>();
            if(BackpackUIManager.Instance!=null&&BackpackUIManager.Instance.GetComponent<TowerPanelLayout>()!=null)
            {img.sprite=ReferencePanelAssets.Sprite("FootprintCell");img.color=Color.white;}
            else img.color = new Color(0.52f, 0.61f, 0.68f);
            img.raycastTarget = false;
        }
    }
}
