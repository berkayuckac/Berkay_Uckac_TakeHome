using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HeroButton : MonoBehaviour
    {
        [SerializeField] private Image heroColorImage = null;
        private float heldTime;
        private Objects.Hero hero;
        private bool isHolding;
        private bool isSelected;

        private void Update()
        {
            CalculateHoldTime();
        }

        public Objects.Hero GetButtonData()
        {
            return hero;
        }

        public void SetButtonData(Objects.Hero _hero)
        {
            SetHeroData(_hero);
            SetHeroColor(hero.heroColor);
        }

        private void SetHeroData(Objects.Hero _hero)
        {
            hero = _hero;
        }

        private void SetHeroColor(Color32 heroColor)
        {
            heroColorImage.color = heroColor;
        }

        public void EnableHero()
        {
            gameObject.GetComponent<Button>().interactable = true;
            heroColorImage.gameObject.SetActive(true);
        }

        private void ToggleSelection()
        {
            if (isSelected)
                DeselectButton();
            else
                SelectButton();
        }

        public void ResetButton()
        {
            isSelected = false;
            ToggleBackgroundColor();
        }

        private void DeselectButton()
        {
            isSelected = false;
            Locator.instance.GetService<HeroSelectionUIService>().RemoveFromSelectedButtons(this);
            ToggleBackgroundColor();
        }

        private void SelectButton()
        {
            if (!CheckWithinSelectLimit() || !IsButtonInteractable()) return;
            Locator.instance.GetService<HeroSelectionUIService>().AddToSelectedButtons(this);
            isSelected = true;
            ToggleBackgroundColor();
        }

        private bool CheckWithinSelectLimit()
        {
            return Locator.instance.GetService<HeroSelectionUIService>().GetSelectedButtonsCount() < 3;
        }

        private void ToggleBackgroundColor()
        {
            gameObject.GetComponent<Image>().color = isSelected ? new Color(0.58f, 1f, 0.49f) : Color.white;
        }

        private bool IsButtonInteractable()
        {
            return gameObject.GetComponent<Button>().interactable;
        }

        #region POINTER_INPUT

        public void StartHolding()
        {
            if (IsButtonInteractable())
                isHolding = true;
        }

        public void StopHolding()
        {
            if (heldTime < 3)
            {
                ToggleSelection();
                isHolding = false;
            }
            else if (heldTime >= 3)
            {
                isHolding = false;
            }

            isHolding = false;
            heldTime = 0f;
            Locator.instance.GetService<HeroInfo>().ClosePanel();
        }

        private void CalculateHoldTime()
        {
            if (!isHolding) return;
            heldTime += Time.deltaTime;
            if (heldTime >= 3f)
            {
                Locator.instance.GetService<HeroInfo>().SetPanelData(hero);
                Locator.instance.GetService<HeroInfo>().OpenPanel(transform.position,
                    gameObject.GetComponent<RectTransform>().rect.width);
            }
        }

        #endregion
    }
}