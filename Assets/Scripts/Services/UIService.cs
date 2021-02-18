using System;
using UnityEngine;

namespace Services
{
    public class UIService : MonoBehaviour
    {
        public enum Screens
        {
            None,
            Won,
            Lost
        }

        [SerializeField] private GameObject wonScreen = null;
        [SerializeField] private GameObject lostScreen = null;

        private Screens activeScreen = Screens.None;

        public void OpenScreen(Screens screen)
        {
            GetScreenObject(screen).SetActive(true);
        }

        private void CloseScreen(Screens screen)
        {
            GetScreenObject(screen).SetActive(false);
        }

        public void BackToHeroSelection()
        {
            CloseScreen(activeScreen);
            SetActiveScreen(Screens.None);
            Locator.instance.GetService<BattlefieldService>().ResetBattlefield();
            Locator.instance.GetService<HeroSelectionUIService>().OpenHeroSelectionUI();
        }

        private GameObject GetScreenObject(Screens screen)
        {
            SetActiveScreen(screen);

            return screen switch
            {
                Screens.Won => wonScreen,
                Screens.Lost => lostScreen,
                _ => throw new ArgumentOutOfRangeException(nameof(screen), screen, "Could not find the UI")
            };
        }

        private void SetActiveScreen(Screens screen)
        {
            activeScreen = screen;
        }
    }
}