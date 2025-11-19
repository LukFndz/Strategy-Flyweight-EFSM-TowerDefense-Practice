using Assets.Scripts.Gameplay.Core.Towers;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Card
{
    /// <summary>
    /// Singleton class that manages the local card database, loading data from a JSON file and triggering events when cards are loaded.
    /// </summary>
    [System.Serializable]
    public class CardsDatabase : SingletonMonoBehaviour<CardsDatabase>
    {
        #region Private Variables

        [Tooltip("List of all the card data loaded from the JSON file.")]
        [SerializeField] private List<CardUIData> _cards;

        #endregion

        #region Public Properties
        public bool IsLoaded { get; private set; } = false;
        public List<CardUIData> Cards => _cards;
        #endregion

        #region Monobehaviour Methods

        private void Start()
        {
            LoadCardData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads card data from a JSON file and initializes the database.
        /// </summary>
        private async void LoadCardData()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("LocalDB/Cards");

            if (jsonFile == null)
            {
                Debug.LogWarning("Cards JSON file not found in Resources/LocalDB.");
                return;
            }

            CardDatabaseWrapper cardDatabaseWrapper = JsonUtility.FromJson<CardDatabaseWrapper>(jsonFile.ToString());

            if (cardDatabaseWrapper == null || cardDatabaseWrapper.cards == null)
            {
                Debug.LogWarning("Error loading cards or empty database.");
                return;
            }

            _cards = cardDatabaseWrapper.cards;
            IsLoaded = true;

            await TowerFlyweightFactory.PreloadFlyweights(_cards);
            EventManager.Instance.Trigger(EventName.CardsLoaded);
        }

        #endregion
    }

    /// <summary>
    /// Wrapper class used for deserializing the JSON data into a list of card data.
    /// </summary>
    [System.Serializable]
    public class CardDatabaseWrapper
    {
        [Tooltip("List of all card data from the JSON.")]
        public List<CardUIData> cards;
    }
}