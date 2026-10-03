using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float health;
    public float speed;
    public int damage;
    [Tooltip("Birliğe yapılan saldırılar arasındaki süre (saniye).")]
    [Min(0.1f)] public float attackInterval = 1.2f;
    public int goldReward;

    public List<ElementType> weaknesses;
    public List<ElementType> resistances;
}
