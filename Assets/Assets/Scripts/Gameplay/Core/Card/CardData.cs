using Assets.Scripts.Gameplay.Core.Strategy;
using Assets.Scripts.Gameplay.Core.Towers;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay.Core.Card
{
    [System.Serializable]
    public struct CardUIData
    {
        public string CardID;
        public string CardIcon;
        public int Level;
        public int ManaCost;
        public string Type;
        public int HorizontalCellsRange;
        public int VerticalCellsRange;
        public int Damage;
        public int FireCooldown;
        public string ProjectileSprite;
        public int InitialHp;
    }

    [System.Serializable]
    public class CardData : CardComponent
    {
        #region Private Variables

        [Header("Background Elements")]
        [Tooltip("Background image.")]
        [SerializeField] private Image _background;
        private Sprite _backgroundSprite;

        [Header("Foreground Elements")]
        [Tooltip("Individual star objects representing the card's rating.")]
        [SerializeField] private GameObject[] _stars;
        private int _starsCount;

        [Header("Icons")]
        [Tooltip("Main card image.")]
        [SerializeField] private Image _cardIcon;
        private Sprite _cardIconSprite;

        [Header("Text Elements")]
        [Tooltip("TextMeshPro component for displaying card information.")]
        [SerializeField] private TextMeshProUGUI _manaCost;
        private int _manaCostValue;

        [Header("Tower Prefab")]
        [Tooltip("The base tower prefab")]
        [SerializeField] private Tower _towerPrefab;
        private Sprite _towerSprite;
        private Sprite _projectileSprite;

        private int _cardId;
        private int _horizontalCellsRange;
        private int _verticalCellsRange;
        private int _damage;
        private int _fireCooldown;
        private int _initialHp;

        private Strategy<Tower> _attackStrategy;
        private Strategy<Tower> _idleStrategy;

        #endregion

        #region Monobehaviour Functions

        public override void ManualStart()
        {
            EventManager.Instance.Subscribe(EventName.CardEndDrag, OnCardEndDrag);
            EventManager.Instance.Subscribe(EventName.CardUpdate, OnCardUpdate);
        }

        public override void ManualOnDestroy()
        {
            EventManager.Instance.Unsubscribe(EventName.CardEndDrag, OnCardEndDrag);
            EventManager.Instance.Unsubscribe(EventName.CardUpdate, OnCardUpdate);
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// The card drop event
        /// </summary>
        /// <param name="parameters[0]">The pointereventdata from the card drag handler</param>
        /// <param name="parameters[1]">The dropped card</param>
        private async void OnCardEndDrag(params object[] parameters)
        {
            if ((GameObject)parameters[1] != _card.gameObject) return;

            var eventData = (PointerEventData)parameters[0];
            var pos = GridManager.Instance.GetGridCellUnderMouse();

            if (pos == Vector3.zero || GridManager.Instance.CheckCellBusy(pos)) return;

            TowerFlyweight flyweight = await TowerFlyweightFactory.GetFlyweight(new CardUIData
            {
                CardID = _cardId.ToString(),
                CardIcon = _cardIconSprite.name,
                Level = _starsCount,
                ManaCost = _manaCostValue,
                HorizontalCellsRange = _horizontalCellsRange,
                VerticalCellsRange = _verticalCellsRange,
                Damage = _damage,
                FireCooldown = _fireCooldown,
                InitialHp = _initialHp,
                ProjectileSprite = _projectileSprite.name
            });

            Tower tower = PoolManager.Instance.Spawn<Tower>(_towerPrefab.gameObject, pos);
            tower.InitializeTower(flyweight, _idleStrategy, _attackStrategy);
            GridManager.Instance.SetCellBusy(pos, true);
        }

        /// <summary>
        /// The card data has been updated event
        /// </summary>
        /// <param name="parameters[0]">The card data updated</param>
        private void OnCardUpdate(params object[] parameters)
        {
            var param = (CardUIData)parameters[0];
            if (param.CardID != _cardId.ToString()) return;

            UpdateCardUI(param);
            var index = CardsDatabase.Instance.Cards.FindIndex(x => x.CardID == param.CardID);

            if (index >= 0)
            {
                CardsDatabase.Instance.Cards[index] = param;
            }
        }

        /// <summary>
        /// Update the card UI stars
        /// </summary>
        /// <param name="level">The stars count</param>
        private void SetStars(int level)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].SetActive(i < level);
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Update the card UI with new parameters
        /// </summary>
        /// <param name="card">The card new data</param>
        public void UpdateCardUI(CardUIData card)
        {
            Addressables.LoadAssetAsync<Sprite>("Assets/Assets/Sprites/UI/icons/" + card.CardIcon + ".png").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _cardIcon.sprite = handle.Result;
                    _cardIconSprite = handle.Result;
                }
                else
                {
                    Debug.LogError($"Error al cargar sprite: {card.CardIcon}");
                }
            };

            Addressables.LoadAssetAsync<Sprite>("Assets/Assets/Sprites/Projectiles/" + card.ProjectileSprite + ".png").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _projectileSprite = handle.Result;
                }
                else
                {
                    Debug.LogError($"Error al cargar sprite: {card.ProjectileSprite}");
                }
            };

            Addressables.LoadAssetAsync<Sprite>("Assets/Assets/Sprites/Towers/Tower_" + card.CardID + ".png").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _towerSprite = handle.Result;
                    _cardId = int.Parse(card.CardID);
                }
                else
                {
                    Debug.LogError($"Error al cargar sprite: {card.CardID}");
                }
            };

            Addressables.LoadAssetAsync<Sprite>("Assets/Assets/Sprites/UI/" + card.Type + ".png").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _background.sprite = handle.Result;
                }
                else
                {
                    Debug.LogError($"Error al cargar background: {card.Type}");
                }
            };

            Addressables.LoadAssetAsync<Strategy<Tower>>("Assets/Assets/ScriptableObjects/Attack_" + card.CardID + ".asset").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _attackStrategy = handle.Result;
                }
                else
                {
                    Debug.LogError($"Error al cargar sprite: {card.CardID}");
                }
            };

            Addressables.LoadAssetAsync<Strategy<Tower>>("Assets/Assets/ScriptableObjects/Idle_" + card.CardID + ".asset").Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _idleStrategy = handle.Result;
                }
                else
                {
                    Debug.LogError($"Error al cargar sprite: {card.CardID}");
                }
            };


            SetStars(card.Level);
            _manaCost.text = card.ManaCost.ToString();

            _starsCount = card.Level;
            _manaCostValue = card.ManaCost;
            _backgroundSprite = _background.sprite;
            _cardIconSprite = _cardIcon.sprite;
            _horizontalCellsRange = card.HorizontalCellsRange;
            _verticalCellsRange = card.VerticalCellsRange;
            _damage = card.Damage;
            _fireCooldown = card.FireCooldown;
            _initialHp = card.InitialHp;
        }
        #endregion
    }
}