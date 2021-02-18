using System.Collections.Generic;
using DG.Tweening;
using Enemy;
using Game;
using Hero;
using UnityEngine;

namespace Services
{
    public class BattlefieldService : MonoBehaviour
    {
        public enum Turn
        {
            Player,
            Enemy
        }

        [SerializeField] private GameObject enemy = null;
        [SerializeField] private GameObject battlefieldCamera = null;
        [SerializeField] private Transform cameraHeroPosition = null;
        [SerializeField] private Transform cameraEnemyPosition = null;
        private Turn currentTurn = Turn.Player;
        private readonly List<GameObject> deadHeroesOnBattlefield = new List<GameObject>();

        private List<GameObject> heroesOnBattlefield = new List<GameObject>();

        public void SetHeroesToBattlefield(List<GameObject> _heroesOnBattlefield)
        {
            heroesOnBattlefield = _heroesOnBattlefield;
            ChangeCameraPosition();
        }

        public Turn GetCurrentTurn()
        {
            return currentTurn;
        }

        private void ToggleTurn()
        {
            //check if any of the end rules has occured between turns
            if (CheckIsBattleFinished()) return;
            currentTurn = currentTurn == Turn.Player ? Turn.Enemy : Turn.Player;
            ChangeCameraPosition();
        }

        private void ChangeCameraPosition()
        {
            battlefieldCamera.transform.DOMove(
                currentTurn == Turn.Player ? cameraHeroPosition.position : cameraEnemyPosition.position, 0.3f);
        }

        public void OrderAttackOnEnemy(float damageAmount)
        {
            Locator.instance.GetService<EventService>().OnAttackEnemy(damageAmount);
            ToggleTurn();
        }

        public Vector3 GetEnemyLocation()
        {
            return enemy.transform.position;
        }

        public void OrderAttackOnHero(int heroNumber, float damageAmount)
        {
            var attackedHero = heroesOnBattlefield[heroNumber];

            GetHeroBattle(attackedHero).TakeDamage(damageAmount);
            Locator.instance.GetService<EventService>().OnAttackHero(GetHeroPosition(attackedHero));

            ToggleTurn();
        }

        private HeroBattle GetHeroBattle(GameObject heroGO)
        {
            return heroGO.GetComponent<HeroBattle>();
        }

        private Vector3 GetHeroPosition(GameObject heroGO)
        {
            return heroGO.transform.position;
        }

        public void KillHero(GameObject killedHeroGO)
        {
            deadHeroesOnBattlefield.Add(killedHeroGO);
            heroesOnBattlefield.Remove(killedHeroGO);
        }

        public int GetBattlefieldSize()
        {
            return heroesOnBattlefield.Count;
        }

        private bool CheckIsBattleFinished()
        {
            if (heroesOnBattlefield.Count == 0)
            {
                //heroes lost
                Locator.instance.GetService<EventService>().OnBattleEnd(false);
                Locator.instance.GetService<UIService>().OpenScreen(UIService.Screens.Lost);
                return true;
            }

            if (GetEnemyHealth() <= 0)
            {
                //heroes won
                Locator.instance.GetService<EventService>().OnBattleEnd(true);
                Locator.instance.GetService<UIService>().OpenScreen(UIService.Screens.Won);
                return true;
            }

            return false;
        }

        private float GetEnemyHealth()
        {
            return enemy.GetComponent<EnemyBattle>().GetEnemyData().health;
        }

        public bool DidThisHeroDie(GameObject heroGO)
        {
            return deadHeroesOnBattlefield.Contains(heroGO);
        }

        public void ResetBattlefield()
        {
            //save hero and game data
            Locator.instance.GetService<GameData>().IncreasePlayCount();
            Locator.instance.GetService<SaveService>().SaveGameData();
            Locator.instance.GetService<SaveService>().SaveHeroData();

            //refresh hero selection
            StartCoroutine(Locator.instance.GetService<HeroSelectionUIService>().StartChores());

            //delete spawned heroes and clear their lists
            ClearBattlefield();
        }

        private void ClearBattlefield()
        {
            foreach (var hero in heroesOnBattlefield) Destroy(hero.gameObject);

            foreach (var hero in deadHeroesOnBattlefield) Destroy(hero.gameObject);

            heroesOnBattlefield.Clear();
            deadHeroesOnBattlefield.Clear();
        }
    }
}