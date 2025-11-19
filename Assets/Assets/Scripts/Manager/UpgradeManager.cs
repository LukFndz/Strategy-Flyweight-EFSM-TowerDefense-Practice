using Assets.Scripts.Gameplay.Core;
using Assets.Scripts.Gameplay.Core.Towers;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class UpgradeManager : SingletonMonoBehaviour<UpgradeManager>
    {
        [SerializeField] private int _newDamageValue;
        [ContextMenu("Add the new value to Archer Tower Damage")] // This have testing purposes, just to see how the towers reacts to ugprades.
        public void MakeDamageUpgrade()
        {
            TowerFlyweightFactory.ModifyFlyweight(1, "Damage", _newDamageValue);
        }

        [SerializeField] private int _newManaCostValue;
        [ContextMenu("Add the new value to Archer Card Mana")] // This have testing purposes, just to see how the cards reacts to ugprades.
        public void MakeManaUpgrade()
        {
            TowerFlyweightFactory.ModifyFlyweight(1, "ManaCost", _newManaCostValue);
        }
    }
}