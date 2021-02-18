using System;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(menuName = "Create Enemy", fileName = "Enemy", order = 0)]
    [Serializable]
    public class Enemy : ScriptableObject
    {
        public string enemyName;
        public float health;
        public float attackPower;
        public int level;
        public Color32 enemyColor;
    }
}