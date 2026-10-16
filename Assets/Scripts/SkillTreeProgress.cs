using System;
using System.Collections.Generic;
using UnityEngine;

// Permanent campaign progress. One save contains both rewards and balance.
public sealed class SkillTreeProgress
{
    public const string SaveKey = "BackpackLegions.SkillTree.v1";
    [Serializable] public sealed class ChapterRecord { public int chapterNumber; public int bestPoints; }
    [Serializable] public sealed class Purchase { public string id; public int paid; public TowerType towerType; }
    [Serializable] public sealed class SaveData
    {
        public int version = 1;
        public int availablePoints;
        public List<ChapterRecord> chapters = new List<ChapterRecord>();
        // Stable IDs survive label and layout changes.
        public List<string> unlockedSkills = new List<string>();
        public List<Purchase> purchases = new List<Purchase>();
    }
    private readonly string key;
    private SaveData data;
    public static event Action Changed;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetEvents() { Changed = null; }
    public int AvailablePoints => data.availablePoints;
    public SkillTreeProgress(string saveKey = SaveKey)
    {
        key = saveKey;
        Reload();
    }
    private void Reload()
    {
        data = PlayerPrefs.HasKey(key) ? JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString(key)) : new SaveData();
        if (data == null || data.version != 1) throw new InvalidOperationException("Unsupported skill progress save.");
        if (data.chapters == null) data.chapters = new List<ChapterRecord>();
        if (data.unlockedSkills == null) data.unlockedSkills = new List<string>();
        if(data.purchases==null)data.purchases=new List<Purchase>();
        foreach(var id in LegacyArcherIds)if(data.unlockedSkills.Contains(id)&&!data.purchases.Exists(p=>p.id==id))
            data.purchases.Add(new Purchase{id=id,paid=id=="archer.training"?1:2,towerType=TowerType.ArrowTower});
    }
    public static int PointsForResult(bool victory, int health, int maxHealth)
    {
        if (!victory || health <= 0 || maxHealth <= 0) return 0;
        // Integer thresholds avoid floating point rounding at 50% and 90%.
        long percent = (long)health * 100;
        return percent >= (long)maxHealth * 90 ? 3 : percent >= (long)maxHealth * 50 ? 2 : 1;
    }
    public int BestPoints(int chapterNumber)
    {
        var record = data.chapters.Find(c => c.chapterNumber == chapterNumber);
        return record == null ? 0 : record.bestPoints;
    }
    public int RecordResult(int chapterNumber, bool victory, int health, int maxHealth)
    {
        Reload();
        if (chapterNumber <= 0) return 0;
        int score = PointsForResult(victory, health, maxHealth);
        int gained = Mathf.Max(0, score - BestPoints(chapterNumber));
        if (gained == 0) return 0;
        var record = data.chapters.Find(c => c.chapterNumber == chapterNumber);
        if (record == null) { record = new ChapterRecord { chapterNumber = chapterNumber }; data.chapters.Add(record); }
        record.bestPoints = score;
        data.availablePoints += gained;
        Save();
        return gained;
    }
    public bool IsUnlocked(string skillId) => data.unlockedSkills.Contains(skillId);


    // Only historical receipt migration uses these IDs; gameplay is entirely asset driven.
    private static readonly string[] LegacyArcherIds={"archer.training","archer.light","archer.distance","archer.focus","archer.string","archer.eagle","archer.sharp","archer.sustain","archer.longshot","archer.hunter","archer.perfect"};
    public static SkillData[] Catalog => Resources.LoadAll<SkillData>("Data/Skills");
    public string UnlockError(SkillTreeData tree,SkillData skill)
    {
        if(tree==null||skill==null)return "Yetenek seç.";
        var error=tree.ConfigurationError();if(error!=null)return error;
        if(!tree.nodes.Exists(n=>n.skill==skill)||tree.towerType!=skill.towerType)return "Yetenek bu ağaca ait değil.";
        int count=0;foreach(var entry in Catalog)if(entry.id==skill.id)count++;
        if(count!=1)return "SkillData, Resources/Data/Skills altında tek kimlikle bulunmalı.";
        if(!skill.available)return "TASLAK";
        if(IsUnlocked(skill.id))return "AÇILDI";
        foreach(var parent in skill.prerequisites)if(!IsUnlocked(parent.id))return "ÖNCE ÖN KOŞULU AÇ";
        return AvailablePoints<skill.pointCost?"YETERSİZ PUAN":null;
    }
    public bool TryUnlock(SkillTreeData tree,SkillData skill)
    {
        Reload();if(UnlockError(tree,skill)!=null)return false;
        data.availablePoints-=skill.pointCost;data.unlockedSkills.Add(skill.id);
        data.purchases.Add(new Purchase{id=skill.id,paid=skill.pointCost,towerType=skill.towerType});Save();return true;
    }
    public int ResetTree(TowerType towerType)
    {
        Reload();int refund=0;bool changed=false;
        for(int i=data.purchases.Count-1;i>=0;i--){var purchase=data.purchases[i];if(purchase.towerType!=towerType)continue;
            if(data.unlockedSkills.Remove(purchase.id)){refund+=purchase.paid;changed=true;}data.purchases.RemoveAt(i);
        }
        if(changed){data.availablePoints+=refund;Save();}return refund;
    }
    public List<SkillData.Effect> ActiveEffects(TowerType type)
    {
        var effects=new List<SkillData.Effect>();var catalog=Catalog;var ids=new System.Collections.Generic.Dictionary<string,int>();
        foreach(var skill in catalog){if(string.IsNullOrEmpty(skill.id))continue;if(!ids.ContainsKey(skill.id))ids[skill.id]=0;ids[skill.id]++;}
        foreach(var skill in catalog)if(skill.towerType==type&&skill.available&&IsUnlocked(skill.id)&&ids.ContainsKey(skill.id)&&ids[skill.id]==1&&skill.ConfigurationError()==null)effects.AddRange(skill.effects);
        return effects;
    }
    private void Save()
    {
        PlayerPrefs.SetString(key,JsonUtility.ToJson(data));PlayerPrefs.Save();
        if(key==SaveKey)Changed?.Invoke();
    }
}
