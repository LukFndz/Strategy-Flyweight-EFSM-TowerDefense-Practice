using Assets.Scripts.Gameplay.Core.EFSM;
using Assets.Scripts.Gameplay.Core.Towers;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Strategy
{
    [CreateAssetMenu(menuName = "Tower Strategies/Attack/Arrow")]
    public class ArrowAttackStrategy : Strategy<Tower>
    {
        public override void OnExit(Tower tower)
        {
            tower.Target = null;
        }

        public override void OnUpdate(Tower tower)
        {
            if (tower.Target != null && tower.Target.gameObject.activeSelf)
            {
                float horizontalRange = tower.TowerData.TowerFlyweight.HorizontalCellsRange * GridManager.Instance.CellSize;
                float verticalRange = tower.TowerData.TowerFlyweight.VerticalCellsRange * GridManager.Instance.CellSize;

                float rangeSquared = horizontalRange * horizontalRange + verticalRange * verticalRange;
                float distanceToEnemySquared = (tower.transform.position - tower.Target.transform.position).sqrMagnitude;

                if (distanceToEnemySquared > rangeSquared)
                {
                    tower.ChangeState(TowerStates.Idle);
                }
                else if (Time.time >= tower.LastAttackTime + tower.TowerData.TowerFlyweight.FireCooldown)
                {
                    Projectile projectile = PoolManager.Instance.Spawn<Projectile>(tower.ProjectilePrefab.gameObject, tower.transform.position);
                    projectile.Initialize(tower.Target.transform, tower.TowerData.TowerFlyweight.Damage, tower.TowerData.TowerFlyweight.ProjectileSprite);
                    tower.LastAttackTime = Time.time;
                }
            }
            else
            {
                tower.ChangeState(TowerStates.Idle);
            }
        }
    }
}