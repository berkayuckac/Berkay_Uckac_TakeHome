using UnityEngine;

namespace Enemy
{
    public class EnemyAppearance : MonoBehaviour
    {
        public void SetEnemyAppearance()
        {
            SetEnemyColor();
        }

        private Objects.Enemy GetEnemyObject()
        {
            return gameObject.GetComponent<EnemyBattle>().GetEnemyData();
        }

        private Material GetEnemyMaterial()
        {
            return gameObject.GetComponent<MeshRenderer>().material;
        }

        private void SetEnemyColor()
        {
            SetMaterialColor();
        }

        private void SetMaterialColor()
        {
            GetEnemyMaterial().color = GetEnemyObject().enemyColor;
        }
    }
}