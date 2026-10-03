using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    internal bool directionUp = false; // Kule yerle�tirilecek noktan�n y�n� (yukar� veya a�a��)
    private Transform Place; // Kule yerle�tirilecek nokta
    public bool isOccupied = false; // Kule yerle�tirilip yerle�tirilmedi�ini kontrol eder
    private Collider slotCollider;
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
        if (!isOccupied)
        {
            UİManager.selectedTowerPlace = Place; // Se�ilen kule yerini UI Manager'a g�nder
            UİManager.selectedTowerSlot = this; // Se�ilen slotu UI Manager'a g�nder (sat�n alma iptal edilirse geri a�mak i�in)
            UİManager.TowerShopUI.SetActive(true); // Kule sat�n alma UI's�n� a�
            slotCollider.enabled = false; // Panel a��kken slot tekrar t�klanamas�n
        }
        else
        {
            Debug.Log("Bu slot zaten dolu!");
        }
    }

    // Kule ger�ekten sat�n al�nd���nda �a�r�l�r, slot kal�c� olarak dolu say�l�r
    public void MarkOccupied()
    {
        isOccupied = true;
    }

    // Panel kule al�nmadan kapat�ld���nda �a�r�l�r, slot tekrar t�klanabilir olur
    public void ReleaseSelection()
    {
        if (!isOccupied)
        {
            slotCollider.enabled = true;
        }
    }
}
