using UnityEngine;


namespace Assets.Scripts.Gameplay.Core.Card
{
    public class CardController : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private CardData _cardData;

        public delegate void Component();
        public Component _awake, _start, _update, _onDestroy;

        public CardData CardData { get => _cardData; }

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _cardData.SetContext(this);
            _awake();
        }

        void Start() => _start();

        void Update() => _update();

        void OnDestroy() => _onDestroy();
        #endregion
    }
}
