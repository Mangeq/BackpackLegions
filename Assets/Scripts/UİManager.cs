using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UİManager : MonoBehaviour
{
    //public Transform gridVisualsParent; // Arkadaki 25 siyah karenin klasörü
    //private UnityEngine.UI.Image[,] cellVisuals;

    //public Color defaultColor = Color.white;
    //public Color validColor = new Color(0, 1, 0, 0.4f);
    //public Color invalidColor = new Color(1, 0, 0, 0.4f);

    

    public static GameObject TowerShopUI;
    public static Text WaveText;
    public static Transform selectedTowerPlace;
    public static TowerSlot selectedTowerSlot;
    public static string selectedTowerType;
    public static Tower activeTower;
    private Button TowerShopBuyBTN;
    private Text UnitTowerGold;
    private Text MegicTowerGold;
    private Text ArcherTowerGold;
    private static Text GoldText;
    // Kule fiyatlarının tek kaynağı her prefab'ın kendi TowerData.cost'u.
    private int UnitTowerCost => UnitTowerPrefab.GetComponent<Tower>().data.cost;
    private int MegicTowerCost => MegicTowerPrefab.GetComponent<Tower>().data.cost;
    private int ArcherTowerCost => ArcherTowerPrefab.GetComponent<Tower>().data.cost;

    public GameObject UnitTowerPrefab;
    public GameObject MegicTowerPrefab;
    public GameObject ArcherTowerPrefab;

    internal GameObject Heart1;
    internal GameObject Heart2;
    internal GameObject Heart3;

    // Start is called before the first frame update
    void Awake()
    {
        TowerShopUI = GameObject.Find("TowerShopUI");
        TowerShopBuyBTN = GameObject.Find("TowerShopBuyBTN").GetComponent<Button>();
        UnitTowerGold = GameObject.Find("UnitTowerGold").GetComponent<Text>();
        MegicTowerGold = GameObject.Find("MegicTowerGold").GetComponent<Text>();
        ArcherTowerGold = GameObject.Find("ArcherTowerGold").GetComponent<Text>();
        GoldText = GameObject.Find("GoldText").GetComponent<Text>();
        Heart1 = GameObject.Find("HealBar/Heart1");
        Heart2 = GameObject.Find("HealBar/Heart2");
        Heart3 = GameObject.Find("HealBar/Heart3");
        WaveText= GameObject.Find("WaveBar/WaveText").GetComponent<Text>();

        //cellVisuals = new UnityEngine.UI.Image[5, 5]; // Çanta 5x5
        //if (gridVisualsParent != null)
        //{
        //    int childIndex = 0;
        //    for (int y = 0; y < 5; y++)
        //    {
        //        for (int x = 0; x < 5; x++)
        //        {
        //            if (childIndex < gridVisualsParent.childCount)
        //            {
        //                cellVisuals[x, y] = gridVisualsParent.GetChild(childIndex).GetComponent<UnityEngine.UI.Image>();
        //            }
        //            childIndex++;
        //        }
        //    }
        //}
    }
    void Start()
    {
        TowerShopBuyBTN.enabled = false;
        UnitTowerGold.text = UnitTowerCost.ToString();
        MegicTowerGold.text = MegicTowerCost.ToString();
        ArcherTowerGold.text = ArcherTowerCost.ToString();
        UpdateGoldText();
        TowerShopUI.SetActive(false);
    }
    public void TogglePanel(GameObject panel)
    {
        bool willBeActive = !panel.activeSelf;
        panel.SetActive(willBeActive);

        if (panel == TowerShopUI && !willBeActive)
        {
            // Kule sat�n al�nmadan panel kapat�ld�, se�ili slotu tekrar t�klanabilir yap
            if (selectedTowerSlot != null)
            {
                selectedTowerSlot.ReleaseSelection();
                selectedTowerSlot = null;
            }
            selectedTowerPlace = null;
            selectedTowerType = null;
        }
    }
   


    public static void UpdateWaveText(int currentWave, int totalWaves)
    {
        WaveText.text = "WAVE: " + currentWave + "/" + totalWaves;
    }

    public void UnitTowerBTN()
    {
        selectedTowerType = "UnitTower";
        TowerShopBuyBTN.enabled = true;
    }
    public void MegicTowerBTN()
    {
        selectedTowerType = "MegicTower";
        TowerShopBuyBTN.enabled = true;
    }
    public void ArcherTowerBTN()
    {
        selectedTowerType = "ArcherTower";
        TowerShopBuyBTN.enabled = true;
    }
    public static void UpdateGoldText()
    {
        GoldText.text = EconomyManager.Gold.ToString();
    }
    public void BuyTower() //Bu fonksiyonun içindeki ifler kalkacak towermanager adında bir sınıf oluşturulacak ve o sınıfta kule satın alma işlemi yapılacak. Bu sınıfın içinde kule türüne göre fiyat kontrolü yapılacak ve kule satın alınacak.
    {
        if(selectedTowerType == null || selectedTowerPlace == null)
        {
            Debug.Log("Lütfen bir kule türü ve yer seçin!");
            return;
        }
        if(selectedTowerType == "UnitTower" && EconomyManager.Gold >= UnitTowerCost)
        {
            EconomyManager.Gold -= UnitTowerCost;
            GameObject tower = Instantiate(UnitTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            tower.GetComponent<UnitTower>().directionUp = selectedTowerPlace.GetComponent<TowerSlot>().directionUp;
            UpdateGoldText();
        }
        else if(selectedTowerType == "MegicTower" && EconomyManager.Gold >= MegicTowerCost)
        {
            EconomyManager.Gold -= MegicTowerCost;
            Instantiate(MegicTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            UpdateGoldText();
        }
        else if(selectedTowerType == "ArcherTower" && EconomyManager.Gold >= ArcherTowerCost)
        {
            EconomyManager.Gold -= ArcherTowerCost;
            Instantiate(ArcherTowerPrefab, selectedTowerPlace.position, Quaternion.identity);
            UpdateGoldText();
        }
        else
        {
            Debug.Log("Yeterli altın yok!");
            return;
        }
       
        Debug.Log("Kule satın alındı!");
        selectedTowerSlot.MarkOccupied();
        selectedTowerSlot = null;
        selectedTowerPlace = null;
        selectedTowerType = null;
        TowerShopUI.SetActive(false);
    }

    //public void ResetAllHighlights()
    //{
    //    if (cellVisuals == null) return;
    //    for (int x = 0; x < 5; x++)
    //    {
    //        for (int y = 0; y < 5; y++)
    //        {
    //            if (cellVisuals[x, y] != null)
    //                cellVisuals[x, y].color = defaultColor;
    //        }
    //    }
    //}

    //public void HighlightCells(ItemData item, int startX, int startY)
    //{
    //    ResetAllHighlights();
    //    if (activeBackpack == null || cellVisuals == null) return;

    //    // Eşya sığıyor mu diye kuledeki çantaya (backend) soruyoruz
    //    bool canPlace = activeBackpack.CanPlaceItem(item, startX, startY);

    //    for (int x = 0; x < item.width; x++)
    //    {
    //        for (int y = 0; y < item.height; y++)
    //        {
    //            int targetX = startX + x;
    //            int targetY = startY + y;

    //            if (targetX >= 0 && targetX < 5 && targetY >= 0 && targetY < 5)
    //            {
    //                if (cellVisuals[targetX, targetY] != null)
    //                {
    //                    cellVisuals[targetX, targetY].color = canPlace ? validColor : invalidColor;
    //                }
    //            }
    //        }
    //    }
    //}
}
