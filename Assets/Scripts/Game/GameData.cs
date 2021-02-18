using System;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class GameData : MonoBehaviour
    {
        public int playCount = 1;

        public void IncreasePlayCount()
        {
            playCount += 1;
        }
    }
}