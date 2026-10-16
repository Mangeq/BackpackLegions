using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    ArrowTower,
    CannonTower,
    UnitTower,
    MagicTower
}

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Tower/TowerData")]
public class TowerData : ScriptableObject
{
    [Min(1)] public int towerLevel = 1;
    [Header("Kule deneyimi")]
    [Min(1f)] public float firstLevelExperience = 20f;
    [Min(0f)] public float experienceIncreasePerLevel = 10f;
    [Header("Seviye başına temel büyüme — eşya bonusundan önce")]
    [Min(0)] public float levelDamageGrowth = 0.05f;
    [Min(0)] public float levelAttackSpeedGrowth = 0.015f;
    [Min(0)] public float levelCriticalGrowth = 0.003f;
    [Range(0, 1)] public float maximumCriticalChance = 0.6f;
    public ItemData defaultWeapon;
    [Header("Ana eşyanın element evrimleri — başlangıç eşyasıyla aynı şekil")]
    public ItemData fireWeapon;
    public ItemData waterWeapon;
    public ItemData earthWeapon;
    public ItemData airWeapon;

    public ItemData GetMainItem(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire: return fireWeapon;
            case ElementType.Water: return waterWeapon;
            case ElementType.Earth: return earthWeapon;
            case ElementType.Air: return airWeapon;
            default: return defaultWeapon;
        }
    }

    public bool IsMainItem(ItemData item) => item != null &&
        (item == defaultWeapon || item == fireWeapon || item == waterWeapon || item == earthWeapon || item == airWeapon);
    public TowerType type;
    public int damage;
    [Tooltip("Temel kritik olasılığı: 0.1 = %10.")]
    [Range(0f, 1f)] public float criticalChance = 0.1f;
    [Tooltip("Saldırı menzilinin dünya birimi cinsinden yarıçapı; model ölçeğinden bağımsızdır.")]
    [Min(0f)] public float range;
    [Tooltip("Saldırılar arası bekleme süresi (saniye). Küçük değer daha hızlı saldırı demektir.")]
    [Min(0.1f)]
    public float attackSpeed;
    public int cost;
    [Header("Havan — yalnız MortarTower kullanır")]
    [Min(0.1f)] public float explosionRadius = 1.5f;
    [Min(0.1f)] public float mortarFlightTime = 0.25f;
    [Min(0f)] public float mortarArcHeight = 0.8f;
    [Min(1f)] public float mortarCriticalMultiplier = 2f;
    [Header("Dönüşüm ücretleri")]
    [Min(0)] public int elementChangeCost = 100;
    [Tooltip("Kule tipi değişiminde eski kule fiyatının iade edilen oranı.")]
    [Range(0f, 1f)] public float typeChangeRefundRate = 0.5f;
    public int gridWidth=5;
    public int gridHeight=5;
}
