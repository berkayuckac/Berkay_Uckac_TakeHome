using UnityEngine;

namespace Hero
{
    public class HeroData : MonoBehaviour
    {
        [SerializeField] private Objects.Hero heroObject = null;

        public Objects.Hero GetHeroData()
        {
            return heroObject;
        }

        public void SetHeroData(Objects.Hero _hero)
        {
            heroObject = _hero;
        }
    }
}