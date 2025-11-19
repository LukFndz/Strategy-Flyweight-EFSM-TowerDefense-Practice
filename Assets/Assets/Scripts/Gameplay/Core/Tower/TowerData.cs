using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Towers
{
    [RequireComponent(typeof(Tower))]
    public class TowerData : MonoBehaviour // This script saves the tower flyweight data
    {
        [SerializeField] private TowerFlyweight _towerFlyweight;

        public TowerFlyweight TowerFlyweight { get => _towerFlyweight; }

        public void SetTowerFlyweight(TowerFlyweight flyweight)
        {
            _towerFlyweight = flyweight;
        }
    }
}