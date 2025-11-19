using Assets.Scripts.Gameplay.Core.EFSM;
using Assets.Scripts.Gameplay.Core;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Gameplay.Core.Strategy;
using Assets.Scripts.Gameplay.Core.Interface;
using  Assets.Scripts.Gameplay.Core.Enemies;

    namespace Assets.Scripts.Gameplay.Core.Towers
{
    public class Tower : StateMachine<TowerStates>, IDamageable
    {
        #region Private Variables

        [Header("General settings")]
        [Tooltip("Tower data component")]
        [SerializeField] private TowerData _towerData;
        [Tooltip("The projectile base prefab")]
        [SerializeField] private Projectile _projectilePrefab;
        [Tooltip("The enemies layer")]
        [SerializeField] private LayerMask _enemyLayer;

        private Strategy<Tower> _idleStrategy;
        private Strategy<Tower> _attackStrategy;

        [Header("View")]
        [SerializeField] private SpriteRenderer _characterSprite;

        private int _currentHealth;
        private Enemy _target;
        private float _lastAttackTime;

        #endregion

        #region Properties

        public TowerData TowerData => _towerData;
        public Projectile ProjectilePrefab => _projectilePrefab;
        public LayerMask EnemyLayer => _enemyLayer;
        public Enemy Target { get => _target; set => _target = value; }
        public float LastAttackTime { get => _lastAttackTime; set => _lastAttackTime = value; }

        #endregion

        #region MonoBehaviour Functions
        private void OnEnable()
        {
            ConfigureFSM();
        }

        private void OnDisable()
        {
            GridManager.Instance.SetCellBusy(transform.position, false);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Configures the FSM states and transitions.
        /// </summary>
        public override void ConfigureFSM()
        {
            var idle = new State<TowerStates>("IdleState");
            var attacking = new State<TowerStates>("AttackState");

            StateConfigurer.Create(idle)
                .SetTransition(TowerStates.Attack, attacking)
                .Done();

            StateConfigurer.Create(attacking)
                .SetTransition(TowerStates.Idle, idle)
                .Done();

            idle.OnUpdate += () => _idleStrategy.OnUpdate(this);

            attacking.OnUpdate += () => _attackStrategy.OnUpdate(this);
            attacking.OnExit += x => _attackStrategy.OnExit(this);

            _fsm = new EventFSM<TowerStates>(idle);
        }

        /// <summary>
        /// Initialize the tower params
        /// </summary>
        /// <param name="towerFlyweight"> The flyweight to assign to the tower </param>
        /// <param name="idleStrategy"> The idle strategy of the tower </param>
        /// <param name="attackStrategy"> The attack strategy of the tower </param>
        public void InitializeTower(TowerFlyweight towerFlyweight, Strategy<Tower> idleStrategy, Strategy<Tower> attackStrategy)
        {
            _characterSprite.sprite = towerFlyweight.TowerSprite;
            _towerData.SetTowerFlyweight(towerFlyweight);
            _currentHealth = towerFlyweight.InitialHp;

            _idleStrategy = idleStrategy;
            _attackStrategy = attackStrategy;
        }

        /// <summary>
        /// Changes the tower state
        /// </summary>
        /// <param name="newState">The state to transition</param>
        public void ChangeState(TowerStates newState)
        {
            _fsm.SendInput(newState);
        }

        /// <summary>
        /// The tower damage behaviour
        /// </summary>
        /// <param name="damage">The taken damage</param>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }

        #endregion
    }
}