using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Towers
{
    /// <summary>
    /// The tower flyweight data, it saves all the data for a type of tower
    /// </summary>
    [System.Serializable]
    public class TowerFlyweight
    {
        private int _cardID;
        private Sprite _towerSprite;
        private Sprite _cardIcon;
        private int _manaCost;
        private int _stars;
        private Sprite _typeIcon;
        private Sprite _projectileSprite;
        private int _horizontalCellsRange;
        private int _verticalCellsRange;
        private int _damage;
        private int _fireCooldown;
        private int _initialHp;

        public TowerFlyweight(int cardID, Sprite towerSprite, Sprite cardIcon,
            int manaCost, int stars, Sprite typeIcon,
            int initialHp, int damage, int fireCooldown, 
            int verticalCellsRange, int horizontalCellsRange, Sprite projectileSprite)
        {
            _cardID = cardID;
            _towerSprite = towerSprite;
            _cardIcon = cardIcon;
            _manaCost = manaCost;
            _stars = stars;
            _typeIcon = typeIcon;
            _initialHp = initialHp;
            _damage = damage;
            _fireCooldown = fireCooldown;
            _verticalCellsRange = verticalCellsRange;
            _horizontalCellsRange = horizontalCellsRange;
            _projectileSprite = projectileSprite;
        }
        public Sprite TowerSprite { get => _towerSprite; }
        public int CardID { get => _cardID; set => _cardID = value; }
        public Sprite CardIcon { get => _cardIcon; set => _cardIcon = value; }
        public int ManaCost { get => _manaCost; set => _manaCost = value; }
        public int Stars { get => _stars; set => _stars = value; }
        public Sprite TypeIcon { get => _typeIcon; set => _typeIcon = value; }
        public int HorizontalCellsRange { get => _horizontalCellsRange; set => _horizontalCellsRange = value; }
        public int VerticalCellsRange { get => _verticalCellsRange; set => _verticalCellsRange = value; }
        public int Damage { get => _damage; set => _damage = value; }
        public int FireCooldown { get => _fireCooldown; set => _fireCooldown = value; }
        public int InitialHp { get => _initialHp; set => _initialHp = value; }
        public Sprite ProjectileSprite { get => _projectileSprite; set => _projectileSprite = value; }
    }
}