using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet Data", menuName = "Bullet/BulletData")]
public class BulletData : ScriptableObject
{
    public float Speed;
    public float critMultiplier;
    public ElementType bulletType;
}