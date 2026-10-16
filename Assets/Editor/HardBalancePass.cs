using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// One-time rebalance. Inspector assets, not this script, remain the runtime source.
public static class HardBalancePass
{
    [Serializable] class Snapshot { public List<Entry> entries = new List<Entry>(); }
    [Serializable] class Entry { public string path, json; }
    public static string Apply()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first.");
        const string backup="Docs/Validation/hard-balance-original-assets-2026-10-13.json";
        if(File.Exists(backup))throw new Exception("Already applied or interrupted: inspect snapshot before repeating.");
        var chapters=Resources.LoadAll<ChapterData>("Data/Chapters").OrderBy(c=>c.chapterNumber).ToArray();
        var enemies=Resources.LoadAll<EnemyData>("Data/EnemyData/Campaign");
        var towers=Resources.LoadAll<TowerData>("Data/TowerData");
        var items=Resources.LoadAll<ItemData>("Data/Items");
        if(chapters.Length!=5||enemies.Length!=30||towers.Length!=4||items.Length!=240)throw new Exception("Unexpected catalogue; no changes made.");
        var snapshot=new Snapshot();
        foreach(var asset in chapters.Cast<UnityEngine.Object>().Concat(enemies).Concat(towers).Concat(items))
            snapshot.entries.Add(new Entry{path=AssetDatabase.GetAssetPath(asset),json=EditorJsonUtility.ToJson(asset)});
        File.WriteAllText(backup,JsonUtility.ToJson(snapshot,true));
        foreach(var tower in towers)
        {
            tower.levelDamageGrowth=.05f;tower.levelAttackSpeedGrowth=.015f;tower.levelCriticalGrowth=.003f;tower.maximumCriticalChance=.6f;
            tower.firstLevelExperience=24;tower.experienceIncreasePerLevel=12;tower.criticalChance=.05f;
            switch(tower.type)
            {
                case TowerType.ArrowTower: tower.damage=13;tower.attackSpeed=1.1f;tower.range=4.7f;tower.cost=250;break;
                case TowerType.MagicTower: tower.damage=29;tower.attackSpeed=1.9f;tower.range=3.4f;tower.cost=325;break;
                case TowerType.CannonTower: tower.damage=46;tower.attackSpeed=3.4f;tower.range=4.8f;tower.cost=375;tower.explosionRadius=1.25f;break;
                case TowerType.UnitTower: tower.damage=4;tower.attackSpeed=1.35f;tower.range=3;tower.cost=225;break;
            }
            EditorUtility.SetDirty(tower);
        }
        foreach(var item in items)
        {
            bool normal=item.element==ElementType.Normal;
            // Adjacency bonuses and recipes stay intact: careful placement retains value.
            item.damageBonus=(float)Math.Round(item.damageBonus*(normal?.8f:.9f),3);
            item.attackSpeedBonus=(float)Math.Round(item.attackSpeedBonus*(normal?.75f:.85f),3);
            item.criticalChanceBonus=(float)Math.Round(item.criticalChanceBonus*.6f,3);
            if(item.cost>0)item.cost=5*Mathf.CeilToInt(item.cost*1.15f/5f);
            EditorUtility.SetDirty(item);
        }
        string[] roles={"Scout","Raider","Guard","Elite","SkyWisp","Warlord"};
        float[] health={45,90,170,250,95,3000},healthStep={14,30,55,80,35,1800};
        int[] damage={9,14,22,30,14,55},gold={12,16,20,26,18,150},leak={5,8,12,16,8,100};
        float[] speed={3.1f,4.2f,2.4f,3,3.5f,1.4f},xp={16,20,28,36,24,120};
        for(int c=0;c<5;c++)
        {
            string key="Chapter"+(c+1).ToString("00");var prefabs=new GameObject[6];
            for(int r=0;r<6;r++)
            {
                var data=AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Resources/Data/EnemyData/Campaign/"+key+"/"+roles[r]+".asset");
                data.health=health[r]+healthStep[r]*c;data.damage=Mathf.RoundToInt(damage[r]*(1+.18f*c));
                data.goldReward=gold[r]+(r==5?30*c:c);data.experienceReward=xp[r];data.speed=speed[r];
                data.attackInterval=r==5?.9f:1.2f;data.isBoss=r==5;data.baseDamageOverride=leak[r];
                data.weaknessMultiplier=2;data.resistanceMultiplier=.3f;data.normalAttackMultiplier=r==5?.45f:r==0?.9f:.75f;
                EditorUtility.SetDirty(data);
                prefabs[r]=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CampaignEnemies/"+key+"/"+roles[r]+".prefab");
            }
            var chapter=chapters[c];chapter.startingGold=700+150*c;
            int rounds=5+2*c,total=180+80*c,first=20+3*c,remaining=total-first;
            float weights=0;for(int w=1;w<rounds;w++)weights+=1+.18f*w;
            chapter.waves.Clear();
            for(int w=0;w<rounds;w++)
            {
                bool boss=w==rounds-1;int count=w==0?first:boss?remaining:Mathf.RoundToInt((total-first)*(1+.18f*w)/weights);
                if(w>0)remaining-=count;
                int[] kinds=boss?new[]{5,2,1}:w==0?new[]{0}:w==1?new[]{0,1}:w==2?new[]{4}:w%2==1?new[]{1,2,3}:new[]{0,4};
                var round=new WaveRound();int left=count;
                float aggregateInterval=Mathf.Max(.75f,2.15f-.1f*c-.1f*w);
                for(int g=0;g<kinds.Length;g++)
                {
                    int amount=boss&&g==0?1:left/(kinds.Length-g);left-=amount;
                    round.groups.Add(new WaveEnemyGroup{enemyPrefab=prefabs[kinds[g]],enemyCount=amount,
                        spawnInterval=aggregateInterval*(boss?2:kinds.Length),startDelay=boss&&g==0?8:0});
                }
                chapter.waves.Add(round);
            }
            EditorUtility.SetDirty(chapter);
        }
        AssetDatabase.SaveAssets();
        return "Hard balance applied: 180/260/340/420/500 enemies, 700/850/1000/1150/1300 starting gold. Snapshot preserved.";
    }
}
