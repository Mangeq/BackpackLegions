using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Backpack/Skills/Skill Data",fileName="New Skill")]
public class SkillData : ScriptableObject
{
    public enum EffectType { DamagePercent, AttackSpeedPercent, RangePercent, CriticalChancePoints, UnitHealthPercent, SameTargetHitSpeed, OuterRangeDamage, LowHealthDamage, EveryNthAttackCritical }
    [Serializable] public class Effect
    {
        public EffectType type;
        [Tooltip("5 = %5. Kritik şansında 3 = +3 yüzde puan. Kesin kritikte kullanılmaz.")]
        [Min(0)] public float value = 5;
        [Tooltip("İsabet serisinin en fazla kaç kez birikeceği veya kaçıncı saldırının kritik olacağı.")]
        [Min(1)] public int count = 5;
        [Tooltip("Mesafe/can koşulu. 50 = menzilin dış yarısı, 30 = %30 can altı.")]
        [Range(0,100)] public float threshold = 50;
    }
    [Tooltip("Kalıcı kayıt kimliği. Oluşturulduktan sonra değiştirme. Kopya asset için yeni kimlik ver.")]
    public string id;
    public TowerType towerType;
    public string displayName;
    [TextArea(3,6)] public string description;
    public Sprite icon;
    [Min(0)] public int pointCost = 2;
    [Tooltip("Kapalıysa düğüm taslaktır: satın alınamaz ve etki vermez.")]
    public bool available = true;
    [Tooltip("Listedeki bütün yetenekler açılmış olmalı. Kök için boş bırak.")]
    public List<SkillData> prerequisites = new List<SkillData>();
    public List<Effect> effects = new List<Effect>();
    private void OnValidate() { if(string.IsNullOrWhiteSpace(id)) id=Guid.NewGuid().ToString("N"); }
    public string ConfigurationError()
    {
        if(string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(displayName))return "Kimlik ve isim gerekli.";
        if(pointCost<0)return "Puan maliyeti negatif olamaz.";
        if(effects==null)return "Etki listesi eksik.";
        foreach(var effect in effects)
        {
            if(effect==null||float.IsNaN(effect.value)||float.IsInfinity(effect.value)||effect.value<0||effect.count<1||float.IsNaN(effect.threshold)||effect.threshold<0||effect.threshold>100)return "Geçersiz etki değeri.";
            if(effect.type==EffectType.RangePercent&&towerType==TowerType.UnitTower)return "Birlik kulesi için menzil etkisi desteklenmiyor.";
            if(effect.type==EffectType.UnitHealthPercent&&towerType!=TowerType.UnitTower)return "Birlik canı yalnız birlik kulesinde kullanılır.";
            if(effect.type>=EffectType.SameTargetHitSpeed&&towerType!=TowerType.ArrowTower)return "Koşullu etkiler şu an yalnız okçuda destekleniyor.";
        }
        return ValidateParents(this,new HashSet<SkillData>(),new HashSet<SkillData>());
    }
    private static string ValidateParents(SkillData node,HashSet<SkillData> path,HashSet<SkillData> done)
    {
        if(done.Contains(node))return null;
        if(!path.Add(node))return "Ön koşullarda döngü var.";
        if(node.prerequisites!=null)foreach(var parent in node.prerequisites)
        {
            if(parent==null||parent.towerType!=node.towerType)return "Ön koşul eksik veya başka kule türüne ait.";
            var error=ValidateParents(parent,path,done);if(error!=null)return error;
        }
        path.Remove(node);done.Add(node);return null;
    }
}
