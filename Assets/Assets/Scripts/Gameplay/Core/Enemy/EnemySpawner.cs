using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        #region Private Variables
        [SerializeField] private List<Enemy> _enemyPrefabs;
        [SerializeField] private float _spawnInterval = 3f;
        #endregion

        #region Monobehaviour Functions
        private void Start()
        {
            StartCoroutine(SpawnEnemies());
        }
        #endregion

        #region Private Functions
        private IEnumerator SpawnEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnInterval);

                int randomX = GridManager.Instance.GetRandomPointIndex();
                Vector3 spawnPosition = GridManager.Instance.GetCellCenter(randomX, 0);
                spawnPosition.y = transform.position.y;
                Enemy enemy = PoolManager.Instance.Spawn<Enemy>(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Count)].gameObject, spawnPosition);
            }
        }
        #endregion
    }
}
