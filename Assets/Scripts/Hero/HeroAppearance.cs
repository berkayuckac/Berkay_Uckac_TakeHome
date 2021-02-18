using UnityEngine;

namespace Hero
{
    public class HeroAppearance : MonoBehaviour
    {
        public void SetHeroAppearance()
        {
            SetHeroColor();
        }

        private Objects.Hero GetHeroObject()
        {
            return gameObject.GetComponent<HeroData>().GetHeroData();
        }

        private Material GetHeroMaterial()
        {
            return gameObject.GetComponent<MeshRenderer>().material;
        }

        private void SetHeroColor()
        {
            SetMaterialColor();
        }

        private void SetMaterialColor()
        {
            GetHeroMaterial().color = GetHeroObject().heroColor;
        }
    }
}