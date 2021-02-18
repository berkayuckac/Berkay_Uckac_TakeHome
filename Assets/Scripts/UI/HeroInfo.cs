using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HeroInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameValue = null;
        [SerializeField] private TMP_Text levelValue = null;
        [SerializeField] private TMP_Text attackPowerValue = null;
        [SerializeField] private TMP_Text experienceValue = null;

        private bool isOpen;

        private void Start()
        {
            RuntimeTransformReset();
        }

        public void SetPanelData(Objects.Hero hero)
        {
            nameValue.text = hero.heroName;
            levelValue.text = hero.level.ToString();
            attackPowerValue.text = hero.attackPower.ToString();
            experienceValue.text = hero.experience.ToString();
        }

        public bool IsInfoPanelOpen()
        {
            return isOpen;
        }

        public void OpenPanel(Vector3 position, float heroButtonSizeWidth)
        {
            if (isOpen) return;
            isOpen = true;
            SetPosition(position, heroButtonSizeWidth);
            TweenScaleUp();
        }

        private void SetPosition(Vector3 pos, float heroButtonSizeWidth)
        {
            transform.position = new Vector3(pos.x + heroButtonSizeWidth / 2, pos.y - heroButtonSizeWidth / 2, pos.z);
        }

        public void ClosePanel()
        {
            isOpen = false;
            TweenScaleDown();
        }

        private void TweenScaleUp()
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuart).OnPlay(TweenScaleUpOnPlay);
        }

        private void TweenScaleUpOnPlay()
        {
            gameObject.SetActive(true);
            isOpen = true;
        }

        private void TweenScaleDown()
        {
            transform.DOScale(0, 0.2f).SetEase(Ease.OutQuart).OnComplete(TweenScaleDownOnComplete);
        }

        private void TweenScaleDownOnComplete()
        {
            gameObject.SetActive(false);
            isOpen = false;
        }

        private void RuntimeTransformReset()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}