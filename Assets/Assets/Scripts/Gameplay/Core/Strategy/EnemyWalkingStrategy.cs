using Assets.Scripts.Gameplay.Core;
using  Assets.Scripts.Gameplay.Core.Enemies;
using Assets.Scripts.Gameplay.Core.EFSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Strategy
{
    [CreateAssetMenu(menuName = "Enemy Strategies/Walking/Enemy")]
    public class EnemyWalkingStrategy : Strategy<Enemy>
    {
        public override void OnUpdate(Enemy enemy)
        {
            enemy.transform.position += Vector3.down * enemy.Speed * Time.deltaTime;

            if (enemy.IsTowerInRange())
            {
                enemy.SendInputToFSM(EnemyStates.Attack);
            }
        }
    }
}