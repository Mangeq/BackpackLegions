using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitTower : Tower
{
    public int maxUnitCount = 3;
    public float spawnCooldown = 2f;
    public GameObject UnitPrefab;
    public List<GameObject> ActiveUnits = new List<GameObject>();
    public Transform rallyPointTransform;
    public Transform rallyPointTransformUp;
    internal Transform rallyPoint;
    internal bool directionUp;

    public float detectionInterval = 0.3f;

    private bool[] slotOccupied;
    private Vector3[] slotPositions;
    private Dictionary<GameObject, int> unitSlots = new Dictionary<GameObject, int>();

    protected override void Start()
    {
        base.Start(); // Tower taban sinifi: Backpack olusturur, varsayilan silahi yerlestirir, CalculateStats() ile currentDamage'i hesaplar

        slotOccupied = new bool[maxUnitCount];
        // Her birligin nobet noktasi: kuleye en yakin yol parcasinin ortasinda, yol yonunde yan yana
        slotPositions = CalculateSlotPositions();

        // İlk birlikler kuleden doğar, savaş başlayınca nöbet noktalarına yürür.
        for (int i = 0; i < maxUnitCount; i++)
        {
            SpawnUnitAt(i);
        }

        StartCoroutine(DetectEnemies());
    }

    // UnitTower kendisi ates etmez, birlikleri savasir - Tower'in otomatik
    // mermi atma dongusunu burada devre disi birakiyoruz.
    protected override void Update()
    {
    }

    public override void Attack(GameObject target)
    {
        // Kullanilmiyor: UnitTower dogrudan saldirmaz, birlikleri savasir.
    }

    // Backpack her degistiginde Tower statlari yeniden hesaplar; UnitTower kendisi
    // ates etmedigi icin bu statlari aninda sahadaki tum birliklere aktarir.
    public override void CalculateStats()
    {
        base.CalculateStats();

        foreach (GameObject unitObj in ActiveUnits)
        {
            if (unitObj != null)
                ApplyStatsTo(unitObj.GetComponent<Unit>());
        }
    }

    private void ApplyStatsTo(Unit unit)
    {
        unit.elementType = SelectedElement;
        unit.ApplyStats(currentDamage, currentCriticalChance, currentAttackSpeed, 1f + currentHealthBonus);
    }

    private Vector3 AlignToPath(Vector3 approxPosition)
    {
        if (NavMesh.SamplePosition(approxPosition, out NavMeshHit hit, 100f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return approxPosition;
    }

    private void SpawnUnitAt(int slot)
    {
        GameObject unit = Instantiate(UnitPrefab, transform.position, Quaternion.identity);
        Unit unitComp = unit.GetComponent<Unit>();
        unitComp.myTower = gameObject;
        ApplyStatsTo(unitComp); // Kule backpack'indeki itemlerin statlarini birlige aktar
        unitComp.PlaceAt(slotPositions[slot]);

        slotOccupied[slot] = true;
        unitSlots[unit] = slot;
        ActiveUnits.Add(unit);
    }

    // Belirli araliklarla, bos birliklerin hemen yanindaki henuz kimsenin
    // hedeflemedigi dusmani bulup o birlige atar
    IEnumerator DetectEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(detectionInterval);

            foreach (GameObject unitObj in ActiveUnits)
            {
                if (unitObj == null) continue;

                Unit unit = unitObj.GetComponent<Unit>();
                if (!unit.IsIdle) continue;

                GameObject target = FindClosestUnclaimedEnemy(unitObj.transform.position, unit.engageRange);
                if (target != null)
                {
                    unit.Engage(target);
                }
            }
        }
    }

    private GameObject FindClosestUnclaimedEnemy(Vector3 origin, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, ~0, QueryTriggerInteraction.Collide);
        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.IsAir || enemy.isEngaged) continue;

            float dist = Vector3.Distance(origin, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        return closest;
    }

    // Bir birlik oldugunde cagrilir: listeden cikarir ve tam spawnCooldown sonra
    // ayni slota yeni bir birlik cikarir
    public void ReleaseUnit(GameObject unit)
    {
        ActiveUnits.Remove(unit);
        if (unitSlots.TryGetValue(unit, out int slot))
        {
            slotOccupied[slot] = false;
            unitSlots.Remove(unit);
            StartCoroutine(RespawnAfterDelay(slot));
        }
    }

    // Kule tipi değişirken askerler eski kuleden bağımsız sahnede kalmamalı.
    internal void RemoveUnitsForTypeChange()
    {
        StopAllCoroutines();
        foreach (var obj in ActiveUnits)
        {
            if (obj == null) continue;
            var unit = obj.GetComponent<Unit>();
            if (unit != null)
            {
                unit.StopAllCoroutines();
                var enemy = unit.currentEnemy != null ? unit.currentEnemy.GetComponent<Enemy>() : null;
                if (enemy != null && enemy.currentUnit == obj) enemy.ResumeMoving();
            }
            obj.SetActive(false);
            Destroy(obj);
        }
        ActiveUnits.Clear();
    }

    IEnumerator RespawnAfterDelay(int slot)
    {
        yield return new WaitForSeconds(spawnCooldown);
        if (!slotOccupied[slot])
        {
            SpawnUnitAt(slot);
        }
    }

    // Kuleye en yakin yol parcasini bulur ve birlikleri o parcanin ortasina, yolun
    // uzandigi yonde yan yana dizer. Slot tag'ine / elle konmus rally noktasina
    // ihtiyac duymaz; kule yolun hangi tarafinda olursa olsun ayni sekilde calisir.
    private Vector3[] CalculateSlotPositions()
    {
        const float spacing = 1.2f;
        Vector3[] positions = new Vector3[maxUnitCount];

        // 1) Kuleye en yakin yol (NavMesh) noktasi
        if (!NavMesh.SamplePosition(transform.position, out NavMeshHit nearest, 20f, NavMesh.AllAreas))
        {
            for (int i = 0; i < maxUnitCount; i++)
                positions[i] = transform.position;
            return positions;
        }

        // 2) O noktanin hizasinda yolun ortasi ve yolun ic yonu
        Vector3 center = FindRoadCenter(nearest.position, out Vector3 inward);

        // 3) Yolun uzandigi yon, ic yone diktir. Birlikler bu yonde, ortadakinden esit
        //    araliklarla dizilir. Kivrimda duz giden nokta yoldan cikabilecegi icin her
        //    birligin noktasi ayrica kendi hizasindaki yol ortasina cekilir.
        Vector3 along = Vector3.Cross(Vector3.up, inward).normalized;
        for (int i = 0; i < maxUnitCount; i++)
        {
            float offset = (i - (maxUnitCount - 1) / 2f) * spacing;
            Vector3 onPath = AlignToPath(center + along * offset);
            positions[i] = FindRoadCenter(onPath, out _);
        }
        return positions;
    }

    // Verilen yol noktasinin hizasinda yolun (NavMesh seridinin) tam ortasini bulur.
    // inward: en yakin kenardan yolun icine dogru bakan yon.
    private Vector3 FindRoadCenter(Vector3 pointOnPath, out Vector3 inward)
    {
        NavMesh.FindClosestEdge(pointOnPath, out NavMeshHit edge, NavMesh.AllAreas);
        Vector3 normal = edge.normal;
        normal.y = 0f;
        normal.Normalize();

        // Kenarin iki yonune isin at: yolun ic tarafi, isinin daha uzun gidebildigi yondur
        // (Unity'nin verdigi kenar normali bazen yolun disini gosteriyor). Isinin carptigi
        // yer karsi kenar; iki kenarin ortasi yolun ortasi.
        Vector3 center = pointOnPath;
        inward = normal;
        float bestWidth = -1f;
        foreach (Vector3 dir in new[] { normal, -normal })
        {
            NavMesh.Raycast(edge.position + dir * 0.05f, edge.position + dir * 20f, out NavMeshHit opposite, NavMesh.AllAreas);
            float width = Vector3.Distance(edge.position, opposite.position);
            if (width > bestWidth)
            {
                bestWidth = width;
                inward = dir;
                center = (edge.position + opposite.position) / 2f;
            }
        }
        return center;
    }

}
