using System.Collections;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class EnemyBattle : MonoBehaviour
    {
        private const int ATTACK_BACK_DELAY_AMOUNT = 2;
        [SerializeField] private Objects.Enemy enemyObject = null;
        [SerializeField] private Image healthBar = null;

        private EnemyAppearance enemyAppearance;
        private float maxHealth;

        private void Start()
        {
            enemyAppearance = gameObject.GetComponent<EnemyAppearance>();

            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            Locator.instance.GetService<EventService>().onAttackEnemyEvent += TakeDamage;
            Locator.instance.GetService<EventService>().onAttackHeroEvent += AttackSequence;
        }

        private void UnsubscribeFromEvents()
        {
            Locator.instance.GetService<EventService>().onAttackEnemyEvent -= TakeDamage;
            Locator.instance.GetService<EventService>().onAttackHeroEvent -= AttackSequence;
        }

        public Objects.Enemy GetEnemyData()
        {
            return enemyObject;
        }

        public void SetEnemySpecs(int newLevel, int newAttackPower, int newHealth)
        {
            enemyObject.level = newLevel;
            enemyObject.attackPower = newAttackPower;
            enemyObject.health = newHealth;

            UpdateAppearance();
            SetMaxHealth();
            UpdateHealthBar();
        }

        private void UpdateAppearance()
        {
            enemyAppearance.SetEnemyAppearance();
        }

        private void SetMaxHealth()
        {
            maxHealth = enemyObject.health;
        }

        private void UpdateHealthBar()
        {
            healthBar.fillAmount = enemyObject.health / maxHealth;
        }

        private void TakeDamage(float damageAmount)
        {
            enemyObject.health -= damageAmount;
            TakeDamageColorEffect();
            TakeDamageScaleEffect();
            UpdateHealthBar();
            CheckNextAttack();
        }

        private void CheckNextAttack()
        {
            if (!IsEnemyAlive()) return;
            AttackToHero();
        }

        private bool IsEnemyAlive()
        {
            return enemyObject.health > 0;
        }

        private void TakeDamageColorEffect()
        {
            var originalMaterialColor = gameObject.GetComponent<MeshRenderer>().material.color;

            gameObject.GetComponent<MeshRenderer>().material.DOColor(Color.red, 0.3f).OnComplete(delegate
            {
                gameObject.GetComponent<MeshRenderer>().material.DOColor(originalMaterialColor, 0.3f);
            });
        }

        private void TakeDamageScaleEffect()
        {
            gameObject.transform.DOScale(2.5f, 0.3f).OnComplete(delegate { gameObject.transform.DOScale(2, 0.3f); });
        }

        private void AttackToHero()
        {
            StartCoroutine(AttackDelayer());
        }

        private IEnumerator AttackDelayer()
        {
            yield return new WaitForSeconds(ATTACK_BACK_DELAY_AMOUNT);
            Locator.instance.GetService<BattlefieldService>()
                .OrderAttackOnHero(SelectRandomHero(), enemyObject.attackPower);
            StopCoroutine(AttackDelayer());
        }

        private int SelectRandomHero()
        {
            return Random.Range(0, Locator.instance.GetService<BattlefieldService>().GetBattlefieldSize());
        }

        private void AttackSequence(Vector3 heroPosition)
        {
            var enemy = gameObject;
            var originalPosition = enemy.transform.position;

            //move forward and back
            enemy.transform.DOMove(heroPosition, 0.3f)
                .OnComplete(delegate { enemy.transform.DOMove(originalPosition, 0.3f); });
        }
    }
}