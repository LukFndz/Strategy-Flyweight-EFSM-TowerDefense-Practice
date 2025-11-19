using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Gameplay.Core.Card;

namespace Assets.Scripts.Managers
{
    public class HandManager : SingletonMonoBehaviour<HandManager>
    {
        #region Private Variables

        [SerializeField, Tooltip("Normal spacing between the cards when there is enough space.")]
        private float _baseSpacing = 20f;

        [SerializeField, Tooltip("Space between the cards and the edges of the hand area.")]
        private float _borderPadding = 30f;

        [SerializeField, Tooltip("Maximum overlap percentage between the cards in case the cards exceed the screen width.")]
        private float _maxOverlapFactor = 1f;

        [SerializeField, Tooltip("Reference to the hand RectTransform.")]
        private RectTransform _handRect;

        [SerializeField, Tooltip("Prefab for the CardController.")]
        private CardController _cardControllerPrefab;

        private List<RectTransform> _cards = new List<RectTransform>();

        #endregion

        #region Monobehaviour Functions

        private void Start()
        {
            EventManager.Instance.Subscribe(EventName.CardsLoaded, GenerateCards);
            //if (CardsDatabase.Instance != null && CardsDatabase.Instance.IsLoaded)
            //{
            //    GenerateCards();
            //}
        }

        private void OnDestroy()
        {
            EventManager.Instance.Unsubscribe(EventName.CardsLoaded, GenerateCards);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Creates a CardController for each card type, this has testing purposes.
        /// </summary>
        private void GenerateCards(params object[] parameters)
        {
            if (CardsDatabase.Instance == null)
            {
                Debug.Log("Database is not loaded");
                return;
            }

            foreach (var item in CardsDatabase.Instance.Cards)
            {
                var card = Instantiate(_cardControllerPrefab);
                RectTransform cardTransform = card.transform as RectTransform;
                card.CardData.UpdateCardUI(item);
                AddCard(cardTransform);
            }

            UpdateHand();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Updates the layout of the cards within the hand area.
        /// </summary>
        public void UpdateHand()
        {
            if (_cards.Count == 0) return;

            float handWidth = _handRect.rect.width;
            float cardWidth = _cards[0].rect.width;
            float availableWidth = handWidth - (2 * _borderPadding);
            float totalWidth = (_cards.Count * cardWidth) + ((_cards.Count - 1) * _baseSpacing);

            float spacing = _baseSpacing;
            if (totalWidth > availableWidth)
            {
                spacing = (availableWidth - (_cards.Count * cardWidth)) / (_cards.Count - 1);
                float maxOverlap = -cardWidth * _maxOverlapFactor;
                spacing = Mathf.Max(spacing, maxOverlap);
            }

            float totalHandWidth = (_cards.Count * cardWidth) + ((_cards.Count - 1) * spacing);
            float startX = -totalHandWidth / 2f + cardWidth / 2f;

            for (int i = 0; i < _cards.Count; i++)
            {
                float posX = startX + i * (cardWidth + spacing);
                _cards[i].anchoredPosition = new Vector2(posX, 0);
            }

            EventManager.Instance.Trigger(EventName.HandUpdate);
        }

        /// <summary>
        /// Adds a new card to the hand and updates it.
        /// </summary>
        public void AddCard(RectTransform card)
        {
            card.SetParent(_handRect, false);
            _cards.Add(card);
            UpdateHand();
        }

        /// <summary>
        /// Removes a card from the hand and updates it.
        /// </summary>
        public void RemoveCard(RectTransform card)
        {
            _cards.Remove(card);
            Destroy(card.gameObject);
            UpdateHand();
        }

        #endregion

        #region Debug Methods

        /// <summary>
        /// Adds a new random card to the hand. Button for testing.
        /// </summary>
        [ContextMenu("Add random card")]
        private void AddRandomCard()
        {
            var card = Instantiate(_cardControllerPrefab, transform);
            RectTransform cardTransform = card.transform as RectTransform;
            card.CardData.UpdateCardUI(CardsDatabase.Instance.Cards[Random.Range(0, CardsDatabase.Instance.Cards.Count - 1)]);
            AddCard(cardTransform);
        }

        /// <summary>
        /// Removes the last card from the hand. Button for testing.
        /// </summary>
        [ContextMenu("Remove last card")]
        private void RemoveLastCard()
        {
            if (_cards.Count > 0)
            {
                RemoveCard(_cards[_cards.Count - 1]);
            }
        }

        #endregion
    }
}
