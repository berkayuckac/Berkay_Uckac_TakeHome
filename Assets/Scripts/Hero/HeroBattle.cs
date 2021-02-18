using System;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hero
{
    public class HeroBattle : MonoBehaviour
    {
        [SerializeField] private GameObject specsUI = null;
        [SerializeField] private Image healthBar = null;
        [SerializeField] private TMP_Text popupText = null;
        private float heldTime;
        private Objects.Hero heroData;

        private bool isHolding;
        private float maxHealth;

        private void Start()
        {
            GetHeroData();
            DisableSpecUI();
            SetMaxHealth();
            SetSpecUIData();
            SetDamageTextScaleToZero();

            Locator.instance.GetService<EventService>().onBattleEndEvent += EndOfBattleSequence;
        }

        private void Update()
        {
            CalculateHoldTime();
        }

        private void OnDestroy()
        {
            Locator.instance.GetService<EventService>().onBattleEndEvent -= EndOfBattleSequence;
        }

        private void GetHeroData()
        {
            heroData = gameObject.GetComponent<HeroData>().GetHeroData();
        }

        private void SetSpecUIData()
        {
            GetTextComponent(specsUI.transform.GetChild(0).gameObject).text = heroData.heroName;
            GetTextComponent(specsUI.transform.GetChild(2).gameObject).text = heroData.level.ToString();
            GetTextComponent(specsUI.transform.GetChild(4).gameObject).text = heroData.attackPower.ToString();
            GetTextComponent(specsUI.transform.GetChild(6).gameObject).text = heroData.experience.ToString();
        }

        private void EnableSpecUI()
        {
            specsUI.SetActive(true);
        }

        private void DisableSpecUI()
        {
            specsUI.SetActive(false);
        }

        private void SetMaxHealth()
        {
            maxHealth = heroData.health;
        }

        private void UpdateHealthBar()
        {
            healthBar.fillAmount = heroData.health / maxHealth;
        }


        private TMP_Text GetTextComponent(GameObject textGO)
        {
            return textGO.GetComponent<TMP_Text>();
        }

        private void ScaleHeroUp()
        {
            gameObject.transform.DOScale(1.2f, 0.2f);
        }

        private void ScaleHeroDown()
        {
            gameObject.transform.DOScale(1f, 0.2f);
        }

        private void AttackEnemy()
        {
            //order an attack if its hero turn or health > 0 
            if (heroData.health > 0 && CanHeroAttack())
            {
                Locator.instance.GetService<BattlefieldService>().OrderAttackOnEnemy(heroData.attackPower);
                AttackSequence();
                ShowPopupText(heroData.attackPower.ToString(), TextType.DamageGiven);
            }
        }

        private bool CanHeroAttack()
        {
            return Locator.instance.GetService<BattlefieldService>().GetCurrentTurn() == BattlefieldService.Turn.Player;
        }

        private void AttackSequence()
        {
            var hero = gameObject;
            var originalPosition = hero.transform.position;

            //move forward and back
            hero.transform.DOMove(Locator.instance.GetService<BattlefieldService>().GetEnemyLocation(), 0.3f)
                .OnComplete(delegate { hero.transform.DOMove(originalPosition, 0.3f); });
        }

        public void TakeDamage(float damageAmount)
        {
            ShowPopupText(Math.Round(damageAmount).ToString(), TextType.DamageTaken);
            TakeDamageColorEffect();
            TakeDamageScaleEffect();
            ReduceHealth(damageAmount);
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
            gameObject.transform.DOScale(1.5f, 0.3f).OnComplete(delegate { gameObject.transform.DOScale(1, 0.3f); });
        }

        private void ReduceHealth(float damageAmount)
        {
            heroData.health -= damageAmount;
            UpdateHealthBar();

            if (heroData.health <= 0) Locator.instance.GetService<BattlefieldService>().KillHero(gameObject);
        }

        private void ShowPopupText(string text, TextType textType)
        {
            if (textType != TextType.AttributeIncrease)
                SetDamageText(text.Length > 3 ? text.Substring(0, 4) : text);
            else
                SetDamageText(text);

            SetPopupTextColor(textType);
            PopupTextFadeInAndOut();
        }

        private void SetDamageText(string text)
        {
            popupText.text = text;
        }

        private void SetPopupTextColor(TextType textType)
        {
            popupText.color = textType switch
            {
                TextType.DamageGiven => Color.green,
                TextType.DamageTaken => Color.red,
                _ => new Color(1f, 0.91f, 0f)
            };
        }

        private void PopupTextFadeInAndOut()
        {
            popupText.transform.DOScale(1, 0.3f).OnComplete(() =>
                popupText.transform.DOScale(0, 0.3f).SetDelay(0.5f));
        }

        private void SetDamageTextScaleToZero()
        {
            popupText.transform.localScale = Vector3.zero;
        }

        private void EndOfBattleSequence(bool isBattleWon)
        {
            if (isBattleWon)
            {
                HealthReset();

                if (Locator.instance.GetService<BattlefieldService>().DidThisHeroDie(gameObject)) return;
                IncreaseExperience();
                CheckForAttributeIncrease();
            }
            else
            {
                HealthReset();
            }
        }

        private void IncreaseExperience()
        {
            heroData.experience += 1;
        }

        private void CheckForAttributeIncrease()
        {
            if (IsReadyForLevelUp()) LevelUp();
        }

        private void HealthReset()
        {
            heroData.health = maxHealth;
        }

        private bool IsReadyForLevelUp()
        {
            return heroData.experience == 5;
        }

        private void LevelUp()
        {
            heroData.level += 1;
            
            var increasedHealthAmount = heroData.health + heroData.health * 0.1;
            heroData.health = (float) increasedHealthAmount;

            var increasedAttackPowerAmount = heroData.health + heroData.health * 0.1;
            heroData.attackPower = (float) increasedAttackPowerAmount;

            heroData.experience = 0;

            var popupText = "+" + (heroData.health * 0.1).ToString().Substring(0, 4) + " Health" +
                            "+" + (heroData.health * 0.1).ToString().Substring(0, 4) + " AP";
            ShowPopupText(popupText, TextType.AttributeIncrease);
        }

        private enum TextType
        {
            DamageTaken,
            DamageGiven,
            AttributeIncrease
        }

        #region POINTER_INPUT

        public void StartHolding()
        {
            ScaleHeroUp();
            heldTime = 0f;
            isHolding = true;
        }

        public void StopHolding()
        {
            ScaleHeroDown();
            if (heldTime < 3)
            {
                isHolding = false;
                AttackEnemy();
            }
            else if (heldTime >= 3)
            {
                isHolding = false;
                DisableSpecUI();
            }
        }

        private void CalculateHoldTime()
        {
            if (!isHolding) return;
            heldTime += Time.deltaTime;
            if (heldTime >= 3f) EnableSpecUI();
        }

        #endregion
    }
}