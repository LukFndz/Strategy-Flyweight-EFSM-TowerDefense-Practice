using  Assets.Scripts.Gameplay.Core.Enemies;
using Assets.Scripts.Gameplay.Core.EFSM;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Strategy
{
    [CreateAssetMenu(menuName = "Enemy Strategies/Attack/Enemy")]
    public class EnemyAttackStrategy : Strategy<Enemy>
    {
        public override void OnEnter(Enemy enemy)
        {
            enemy.LastAttackTime = Time.time + enemy.AttackCooldown;
        }

        public override void OnUpdate(Enemy enemy)
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.TargetPosition);

            if (distance > enemy.StoppingDistance)
            {
                Vector3 direction = (enemy.TargetPosition - enemy.transform.position).normalized;
                enemy.transform.position += direction * enemy.Speed * Time.deltaTime;
            }
            else
            {
                if (Time.time >= enemy.LastAttackTime + enemy.AttackCooldown)
                {
                    if (enemy.Target != null && enemy.IsTowerInRange())
                    {
                        enemy.Target.TakeDamage(enemy.Damage);
                        enemy.LastAttackTime = Time.time;
                    }
                }
            }

            if (!enemy.IsTowerInRange())
            {
                enemy.SendInputToFSM(EnemyStates.Walking);
            }
        }

        public override void OnExit(Enemy enemy)
        {
            enemy.TargetPosition = Vector3.zero;
            enemy.LastAttackTime = 0;
        }
    }
}