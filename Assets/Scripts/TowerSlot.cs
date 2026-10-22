using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    internal bool directionUp = false; // Kule yerle�tirilecek noktan�n y�n� (yukar� veya a�a��)
    private Transform Place; // Kule yerle�tirilecek nokta
    public bool isOccupied = false; // Kule yerle�tirilip yerle�tirilmedi�ini kontrol eder
    private Collider slotCollider;
    [Header("Platform visuals")]
    public Renderer[] platformRenderers;
    public GameObject selectionHighlight;

    private void LateUpdate()
    {
        if (selectionHighlight == null) return;
        bool selected = !isOccupied && UİManager.selectedTowerSlot == this
            && UİManager.TowerShopUI != null && UİManager.TowerShopUI.activeInHierarchy;
        if (selectionHighlight.activeSelf != selected) selectionHighlight.SetActive(selected);
    }

    private void OnDisable()
    {
        if (selectionHighlight != null) selectionHighlight.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        Place = gameObject.transform; // Kule yerle�tirilecek nokta olarak kendi transformunu kullan,
        slotCollider = GetComponent<Collider>();
        if(gameObject.CompareTag("Up"))
        {
            directionUp = true; // E�er tag "UpSlot" ise, y�n� yukar� olarak ayarla
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        // UI tıklaması arkadaki dünya slotunun seçimini değiştirmemeli.
        if (IsPointerOverUI(Input.mousePosition)) return;
        if (UİManager.ConsumeWorldPanelClick()) return;
        if (!GameManager.CanBuyTowers) return;
        if (!isOccupied)
        {
            // Önceki yerin kilidini, referansını değiştirmeden önce kaldır.
            if (UİManager.selectedTowerSlot != null && UİManager.selectedTowerSlot != this)
                UİManager.selectedTowerSlot.ReleaseSelection();
            UİManager.selectedTowerType = null;
            UİManager.selectedTowerPlace = Place; // Se�ilen kule yerini UI Manager'a g�nder
            UİManager.selectedTowerSlot = this; // Se�ilen slotu UI Manager'a g�nder (sat�n alma iptal edilirse geri a�mak i�in)
            UİManager.TowerShopUI.SetActive(true); // Kule sat�n alma UI's�n� a�
            FindObjectOfType<UİManager>().PreparePurchasePanel();
            slotCollider.enabled = false; // Panel a��kken slot tekrar t�klanamas�n
        }
        else
        {
            Debug.Log("Bu slot zaten dolu!");
        }
    }

    internal static bool IsPointerOverUI(Vector2 screenPosition)
    {
        var events = UnityEngine.EventSystems.EventSystem.current;
        if (events == null) return false;
        // OnMouseDown, EventSystem'in pointer kaydından önce çalışabilir.
        // Önceki kare sonucunu değil, bu tıklamanın konumunu sorgula.
        var pointer = new UnityEngine.EventSystems.PointerEventData(events) { position = screenPosition };
        var hits = new List<UnityEngine.EventSystems.RaycastResult>();
        events.RaycastAll(pointer, hits);
        foreach (var hit in hits)
            if (hit.module is UnityEngine.UI.GraphicRaycaster) return true;
        return false;
    }

    // Kule ger�ekten sat�n al�nd���nda �a�r�l�r, slot kal�c� olarak dolu say�l�r
    public void MarkOccupied()
    {
        isOccupied = true;
        if (slotCollider != null) slotCollider.enabled = false;
        if (selectionHighlight != null) selectionHighlight.SetActive(false);
        if (platformRenderers != null)
            foreach (var renderer in platformRenderers)
                if (renderer != null) renderer.enabled = false;
    }

    // Panel kule al�nmadan kapat�ld���nda �a�r�l�r, slot tekrar t�klanabilir olur
    public void ReleaseSelection()
    {
        if (selectionHighlight != null) selectionHighlight.SetActive(false);
        if (!isOccupied)
        {
            slotCollider.enabled = true;
        }
    }
}
