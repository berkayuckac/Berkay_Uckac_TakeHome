using System.Collections.Generic;
using Enemy;
using Hero;
using UI;
using UnityEngine;

namespace Services
{
    public class BattleSpawnService : MonoBehaviour
    {
        [SerializeField] private List<Transform> listOfHeroSpawnPositions = new List<Transform>();
        [SerializeField] private GameObject heroPrefab = null;
        [SerializeField] private Transform heroParent = null;
        [SerializeField] private EnemyBattle enemy = null;

        private readonly List<GameObject> heroesOnBattlefield = new List<GameObject>();

        public void StartBattle(List<HeroButton> selectedHeroButtons)
        {
            SpawnHeroes(selectedHeroButtons);
            SetEnemySpecs();
            Locator.instance.GetService<BattlefieldService>().SetHeroesToBattlefield(heroesOnBattlefield);
        }

        private void SpawnHeroes(List<HeroButton> selectedHeroButtons)
        {
            foreach (var hero in selectedHeroButtons)
            {
                var _heroData = hero.GetButtonData();
                var _heroGameObject = InstantiateHero();
                SetHeroData(_heroGameObject, _heroData);
                SetHeroPosition(_heroGameObject, selectedHeroButtons.IndexOf(hero));
                SetHeroAppearance(_heroGameObject);
                AddHeroToBattlefieldList(_heroGameObject);
            }
        }

        private GameObject InstantiateHero()
        {
            return Instantiate(heroPrefab, heroParent);
        }

        private void SetHeroData(GameObject heroGO, Objects.Hero hero)
        {
            GetHeroData(heroGO).SetHeroData(hero);
        }

        private HeroData GetHeroData(GameObject heroGO)
        {
            return heroGO.GetComponent<HeroData>();
        }

        private void SetHeroPosition(GameObject heroGO, int positionInList)
        {
            heroGO.transform.position = listOfHeroSpawnPositions[positionInList].position;
        }

        private void SetHeroAppearance(GameObject heroGO)
        {
            GetHeroAppearance(heroGO).SetHeroAppearance();
        }

        private HeroAppearance GetHeroAppearance(GameObject heroGO)
        {
            return heroGO.GetComponent<HeroAppearance>();
        }

        private void AddHeroToBattlefieldList(GameObject hero)
        {
            heroesOnBattlefield.Add(hero);
        }

        private void SetEnemySpecs()
        {
            enemy.SetEnemySpecs(GetAverageHeroLevel(), GetAverageHeroAttackPower(), GetAverageHeroHealth() * 2);
        }

        private int GetAverageHeroLevel()
        {
            var sum = 0f;

            foreach (var hero in heroesOnBattlefield) sum += GetHeroData(hero).GetHeroData().level;

            return (int) sum / heroesOnBattlefield.Count;
        }

        private int GetAverageHeroAttackPower()
        {
            var sum = 0f;

            foreach (var hero in heroesOnBattlefield) sum += GetHeroData(hero).GetHeroData().attackPower;

            return (int) sum / heroesOnBattlefield.Count;
        }

        private int GetAverageHeroHealth()
        {
            var sum = 0f;

            foreach (var hero in heroesOnBattlefield) sum += GetHeroData(hero).GetHeroData().health;

            return (int) sum / heroesOnBattlefield.Count;
        }
    }
}