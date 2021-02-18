using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Hero;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class HeroSelectionUIService : MonoBehaviour
    {
        [SerializeField] private GameObject heroSelectionUI = null;
        [SerializeField] private List<HeroButton> listOfHeroButtons = new List<HeroButton>();
        [SerializeField] private Button battleButton = null;

        private readonly List<HeroButton> selectedButtons = new List<HeroButton>();

        private void Start()
        {
            StartCoroutine(StartChores());
        }

        public IEnumerator StartChores()
        {
            yield return new WaitForSeconds(0.1f);
            RefreshGameData();
            RefreshHeroData();
            SetHeroData();
            CheckForAvailableHeroCount();
            StopCoroutine(StartChores());
        }

        private static void RefreshHeroData()
        {
            Locator.instance.GetService<SaveService>().LoadHeroData();
        }

        private static void RefreshGameData()
        {
            Locator.instance.GetService<SaveService>().LoadGameData();
        }

        private Objects.Hero[] GetHeroData()
        {
            return Locator.instance.GetService<Heroes>().arrayOfHeroes;
        }

        private void SetHeroData()
        {
            var heroes = GetHeroData();

            for (var i = 0; i < heroes.Length; i++) listOfHeroButtons[i].SetButtonData(heroes[i]);
        }

        private void CheckForAvailableHeroCount()
        {
            var playCount = Locator.instance.GetService<GameData>().playCount;
            EnableButtonAndColor(CalculateHeroCount(playCount));
        }

        private int CalculateHeroCount(int playCount)
        {
            var heroCount = 3 + Convert.ToInt32(Math.Floor(playCount / 5f));
            return playCount < 5 ? 3 : heroCount;
        }

        private void EnableButtonAndColor(int toEnableCount)
        {
            for (var i = 0; i < toEnableCount; i++) listOfHeroButtons[i].EnableHero();
        }

        public int GetSelectedButtonsCount()
        {
            return selectedButtons.Count;
        }

        public void AddToSelectedButtons(HeroButton hero)
        {
            selectedButtons.Add(hero);
            CheckReadyForBattle();
        }

        public void RemoveFromSelectedButtons(HeroButton hero)
        {
            selectedButtons.Remove(hero);
            CheckReadyForBattle();
        }

        private void CheckReadyForBattle()
        {
            battleButton.interactable = GetSelectedButtonsCount() == 3;
        }

        private void CloseHeroSelectionUI()
        {
            heroSelectionUI.transform.DOScale(0, 0.2f).SetEase(Ease.OutQuart).OnComplete(delegate
            {
                heroSelectionUI.SetActive(false);
            });
        }

        public void OpenHeroSelectionUI()
        {
            heroSelectionUI.transform.DOScale(1, 0.2f).SetEase(Ease.OutQuart).OnPlay(delegate
            {
                heroSelectionUI.SetActive(true);
            });
        }

        public void GoToBattle()
        {
            Locator.instance.GetService<BattleSpawnService>().StartBattle(selectedButtons);
            CloseHeroSelectionUI();
            ResetHeroSelectionButtons();
        }

        private void ResetHeroSelectionButtons()
        {
            foreach (var button in selectedButtons) button.ResetButton();

            selectedButtons.Clear();
            CheckReadyForBattle();
        }
    }
}