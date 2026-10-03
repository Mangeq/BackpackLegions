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
    public ItemData defaultWeapon;
    public TowerType type;
    public int damage;
    [Tooltip("Temel kritik olasılığı: 0.1 = %10.")]
    [Range(0f, 1f)] public float criticalChance = 0.1f;
    public float range;
    [Tooltip("Saldırılar arası bekleme süresi (saniye). Küçük değer daha hızlı saldırı demektir.")]
    [Min(0.1f)]
    public float attackSpeed;
    public int cost;
    public int gridWidth=5;
    public int gridHeight=5;
}
