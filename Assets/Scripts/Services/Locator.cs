using System;
using UnityEngine;

namespace Services
{
    public class Locator : MonoBehaviour
    {
        [SerializeField] private SaveService saveService = null;
        [SerializeField] private Hero.Heroes heroes = null;
        [SerializeField] private Game.GameData gameData = null;
        [SerializeField] private HeroSelectionUIService heroSelectionUIService = null;
        [SerializeField] private UI.HeroInfo heroInfoUIService = null;
        [SerializeField] private BattleSpawnService battleService = null;
        [SerializeField] private BattlefieldService battlefieldService = null;
        [SerializeField] private EventService eventService = null;
        [SerializeField] private UIService uiService = null;
        public static Locator instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public T GetService<T>()
        {
            if (typeof(T) == typeof(SaveService))
            {
                return (T) Convert.ChangeType(saveService, typeof(T));
            }
            else if (typeof(T) == typeof(Hero.Heroes))
            {
                return (T) Convert.ChangeType(heroes, typeof(T)); 
            }            
            else if (typeof(T) == typeof(Game.GameData))
            {
                return (T) Convert.ChangeType(gameData, typeof(T)); 
            }            
            else if (typeof(T) == typeof(HeroSelectionUIService))
            {
                return (T) Convert.ChangeType(heroSelectionUIService, typeof(T)); 
            }           
            else if (typeof(T) == typeof(UI.HeroInfo))
            {
                return (T) Convert.ChangeType(heroInfoUIService, typeof(T)); 
            }            
            else if (typeof(T) == typeof(BattleSpawnService))
            {
                return (T) Convert.ChangeType(battleService, typeof(T)); 
            }            
            else if (typeof(T) == typeof(BattlefieldService))
            {
                return (T) Convert.ChangeType(battlefieldService, typeof(T)); 
            }          
            else if (typeof(T) == typeof(EventService))
            {
                return (T) Convert.ChangeType(eventService, typeof(T)); 
            }            
            else if (typeof(T) == typeof(UIService))
            {
                return (T) Convert.ChangeType(uiService, typeof(T)); 
            }

            return (T) Convert.ChangeType(this, typeof(T));
        }
    }
}