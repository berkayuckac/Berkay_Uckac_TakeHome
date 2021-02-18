using System;
using System.IO;
using Game;
using Hero;
using UnityEngine;

namespace Services
{
    public class SaveService : MonoBehaviour
    {
        private string gameDataFilePath;
        private string heroFilePath;

        private void Start()
        {
            SetHeroFilePath();
            SetGameDataFilePath();
            CheckForHeroFile();
        }

        #region HELPER

        //Had to use a helper as the built-in JsonUtility won't serialize array of ScriptableObjects to JSON
        private struct JsonHelper
        {
            internal static string ToJson<T>(T[] array, bool prettyPrint)
            {
                var wrapper = new Wrapper<T> {Entries = array};
                return JsonUtility.ToJson(wrapper, prettyPrint);
            }

            internal static T[] FromJson<T>(string json)
            {
                var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
                return wrapper.Entries;
            }
        }

        [Serializable]
        private struct Wrapper<T>
        {
            public T[] Entries;
        }

        [Serializable]
        private struct TempHero
        {
            public string heroName;
            public float health;
            public float attackPower;
            public int experience;
            public int level;
            public Color32 heroColor;

            public TempHero(string _heroName, float _health, float _attackPower, int _experience, int _level,
                Color32 _heroColor)
            {
                heroName = _heroName;
                health = _health;
                attackPower = _attackPower;
                experience = _experience;
                level = _level;
                heroColor = _heroColor;
            }
        }

        #endregion

        #region HERO_SAVE_LOAD

        private void CheckForHeroFile()
        {
            if (!File.Exists(heroFilePath))
            {
                File.Create(gameDataFilePath).Dispose();
                SaveGameData();
                File.Create(heroFilePath).Dispose();
                SaveHeroData();
            }
        }

        private void SetHeroFilePath()
        {
            #if UNITY_EDITOR
            heroFilePath = Path.Combine(Application.streamingAssetsPath, "heroData.json");
            #elif UNITY_ANDROID
            heroFilePath = Path.Combine(Application.persistentDataPath, "heroData.json");
            #endif
        }

        private void SetGameDataFilePath()
        {
            #if UNITY_EDITOR
            gameDataFilePath = Path.Combine(Application.streamingAssetsPath, "gameData.json");
            #elif UNITY_ANDROID
            gameDataFilePath = Path.Combine(Application.persistentDataPath, "gameData.json");
            #endif
        }

        private Objects.Hero[] GetArrayOfHeroes()
        {
            return Locator.instance.GetService<Heroes>().arrayOfHeroes;
        }

        public void SaveHeroData()
        {
            var json = JsonHelper.ToJson(CreateArrayOfConvertedHeroes(), true);
            File.WriteAllText(heroFilePath, "");
            File.AppendAllText(heroFilePath, json);
        }

        private TempHero[] CreateArrayOfConvertedHeroes()
        {
            var arrayOfHeroes = GetArrayOfHeroes();
            var arrayOfConvertedHeroes = new TempHero[arrayOfHeroes.Length];

            for (var i = 0; i < arrayOfHeroes.Length; i++)
                arrayOfConvertedHeroes[i] = ConvertHeroType(arrayOfHeroes[i]);

            return arrayOfConvertedHeroes;
        }

        private TempHero ConvertHeroType(Objects.Hero _hero)
        {
            return new TempHero(_hero.heroName, _hero.health, _hero.attackPower, _hero.experience, _hero.level,
                _hero.heroColor);
        }

        public void LoadHeroData()
        {
            var jsonString = File.ReadAllText(heroFilePath);
            var arrayOfTempHeroes = JsonHelper.FromJson<TempHero>(jsonString);
            SetHeroDataToScriptableObjects(arrayOfTempHeroes);
        }

        private void SetHeroDataToScriptableObjects(TempHero[] loadedHeroArray)
        {
            var arrayOfHeroes = GetArrayOfHeroes();

            for (var i = 0; i < arrayOfHeroes.Length; i++)
            {
                arrayOfHeroes[i].heroName = loadedHeroArray[i].heroName;
                arrayOfHeroes[i].health = loadedHeroArray[i].health;
                arrayOfHeroes[i].attackPower = loadedHeroArray[i].attackPower;
                arrayOfHeroes[i].experience = loadedHeroArray[i].experience;
                arrayOfHeroes[i].level = loadedHeroArray[i].level;
                arrayOfHeroes[i].heroColor = loadedHeroArray[i].heroColor;
            }
        }

        #endregion

        #region GAMEDATA

        public void SaveGameData()
        {
            var json = JsonUtility.ToJson(GetCurrentGameData(), true);
            File.WriteAllText(gameDataFilePath, "");
            File.AppendAllText(gameDataFilePath, json);
        }

        private GameData GetCurrentGameData()
        {
            return Locator.instance.GetService<GameData>();
        }

        public void LoadGameData()
        {
            var jsonString = File.ReadAllText(gameDataFilePath);
            SetGameDataToCurrent(jsonString);
        }

        private void SetGameDataToCurrent(string jsonString)
        {
            JsonUtility.FromJsonOverwrite(jsonString, Locator.instance.GetService<GameData>());
        }

        #endregion
    }
}