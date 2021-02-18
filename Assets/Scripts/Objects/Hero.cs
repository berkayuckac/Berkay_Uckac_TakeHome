using System;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(menuName = "Create Hero", fileName = "Hero", order = 0)]
    [Serializable]
    public class Hero : ScriptableObject
    {
        public string heroName;
        public float health;
        public float attackPower;
        public int experience;
        public int level;
        public Color32 heroColor;
    }
}