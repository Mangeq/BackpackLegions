using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public enum MovementType { Ground = 0, Air = 1 }
    [Tooltip("Hava düşmanı aynı yolu izler; birliklerle çatışmaz. Uçuş prefabında visualRoot bağlanmalıdır.")]
    public MovementType movementType = MovementType.Ground;
    [Min(0f)] public float flightHeight = 1f;
    public string enemyName;
    [Tooltip("Düşmanın element kimliği. Kara/hava hareket türünden bağımsızdır.")]
    public ElementType element = ElementType.Normal;
    public float health;
    public float speed;
    public int damage;
    [Tooltip("Birliğe yapılan saldırılar arasındaki süre (saniye).")]
    [Min(0.1f)] public float attackInterval = 1.2f;
    public int goldReward;
    [Tooltip("Ölümde, gerçek hasar katkısına göre kulelere paylaştırılan XP.")]
    [Min(0f)] public float experienceReward = 20f;

    [Tooltip("Normal kimlikli eski düşmanlar için özel zayıflıklar. Elementli düşmanda aşağıdaki döngü kullanılır.")]
    public List<ElementType> weaknesses;
    [Tooltip("Normal kimlikli eski düşmanlar için özel dirençler.")]
    public List<ElementType> resistances;

    // Onaylanan döngü: Su > Ateş > Hava > Toprak > Su.
    public ElementType WeakTo => element == ElementType.Fire ? ElementType.Water
        : element == ElementType.Air ? ElementType.Fire
        : element == ElementType.Earth ? ElementType.Air
        : element == ElementType.Water ? ElementType.Earth : ElementType.Normal;
    public ElementType ResistantTo => element == ElementType.Fire ? ElementType.Air
        : element == ElementType.Air ? ElementType.Earth
        : element == ElementType.Earth ? ElementType.Water
        : element == ElementType.Water ? ElementType.Fire : ElementType.Normal;

    public float GetDamageMultiplier(ElementType attack)
    {
        if (element != ElementType.Normal)
            return attack == WeakTo ? 1.5f : attack == ResistantTo ? 0.5f : 1f;
        // Eski nötr düşmanların özel verilerini sessizce değiştirme.
        return weaknesses != null && weaknesses.Contains(attack) ? 1.5f
            : resistances != null && resistances.Contains(attack) ? 0.5f : 1f;
    }
}
