using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Backpack/Skills/Skill Tree Data",fileName="New Skill Tree")]
public class SkillTreeData : ScriptableObject
{
    [Serializable] public class Node
    {
        public SkillData skill;
        [Tooltip("Orta alanın sol üstüne göre düğüm merkezi. X: 80–720, Y: 50–510.")]
        public Vector2 position;
    }
    public TowerType towerType;
    public string title;
    public string subtitle;
    public Color accent = new Color(.25f,.39f,.29f);
    public List<Node> nodes = new List<Node>();
    public string ConfigurationError()
    {
        var ids=new HashSet<string>();
        foreach(var node in nodes)
        {
            if(node==null||node.skill==null)return "Boş düğüme SkillData sürükle.";
            if(node.skill.towerType!=towerType)return "Düğümün kule türü ağaçla uyuşmuyor.";
            if(!ids.Add(node.skill.id))return "Ağaçta yinelenen yetenek kimliği var.";
            var error=node.skill.ConfigurationError();if(error!=null)return error;
        }
        foreach(var node in nodes)if(node.skill.prerequisites!=null)foreach(var parent in node.skill.prerequisites)
            if(!nodes.Exists(n=>n!=null&&n.skill==parent))return "Ön koşulu da bu ağaca ekle.";
        return null;
    }
}
