using System;
using UnityEngine;

namespace Services
{
    public class EventService : MonoBehaviour
    {
        public event Action<float> onAttackEnemyEvent;

        public void OnAttackEnemy(float damageAmount)
        {
            onAttackEnemyEvent?.Invoke(damageAmount);
        }

        public event Action<Vector3> onAttackHeroEvent;

        public void OnAttackHero(Vector3 heroPosition)
        {
            onAttackHeroEvent?.Invoke(heroPosition);
        }

        public event Action<bool> onBattleEndEvent;

        public void OnBattleEnd(bool isBattleWon)
        {
            onBattleEndEvent?.Invoke(isBattleWon);
        }
    }
}