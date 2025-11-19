using Assets.Scripts.Gameplay.Core.EFSM;
using  Assets.Scripts.Gameplay.Core.Enemies;
using Assets.Scripts.Gameplay.Core.Towers;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Strategy
{
    [CreateAssetMenu(menuName = "Tower Strategies/Idle/Arrow")]
    public class ArrowIdleStrategy : Strategy<Tower>
    {
        public override void OnUpdate(Tower tower)
        {
            float cellSize = GridManager.Instance.CellSize;
            Vector2 size = new Vector2(
                (tower.TowerData.TowerFlyweight.HorizontalCellsRange * 2 + 1) * cellSize,
                (tower.TowerData.TowerFlyweight.VerticalCellsRange * 2 + 1) * cellSize
            );

            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(tower.transform.position, size, 0, tower.EnemyLayer);

            if (hitEnemies.Length > 0)
            {
                tower.Target = hitEnemies[0].GetComponent<Enemy>();
                tower.ChangeState(TowerStates.Attack);
            }
        }
    }
}