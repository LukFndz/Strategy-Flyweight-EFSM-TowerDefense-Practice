using Assets.Scripts.Gameplay.Core.EFSM;
using Assets.Scripts.Gameplay.Core.Interface;
using Assets.Scripts.Gameplay.Core.Strategy;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Enemies
{
    public class Enemy : StateMachine<EnemyStates>, IDamageable
    {
        #region Private Variables
        [SerializeField, Tooltip("Speed at which the enemy moves downward.")]
        private int _initialHp = 10;
        private int _currentHp;

        [SerializeField, Tooltip("Speed at which the enemy moves downward.")]
        private float _speed = 2f;

        [SerializeField, Tooltip("Detection radius for attacking a tower.")]
        private int _damage = 1;

        [SerializeField, Tooltip("Detection radius for attacking a tower.")]
        private float _attackRange = 1.5f;

        [SerializeField, Tooltip("Stop distance to attack the target")]
        private float _stoppingDistance = 1.5f;

        [SerializeField, Tooltip("Stop distance to attack the target")]
        private float _attackCooldown = 1.5f;
        private float _lastAttackTime = 0f;

        [SerializeField, Tooltip("Layer mask used to detect towers.")]
        private LayerMask _towerLayer;

        private Transform _transform;

        private IDamageable _target;
        private Vector3 _targetPosition;

        [SerializeField] private Strategy<Enemy> _walkingStrategy;
        [SerializeField] private Strategy<Enemy> _attackStrategy;

        #endregion

        #region Properties

        public float Speed { get => _speed; set => _speed = value; }
        public int Damage { get => _damage; set => _damage = value; }
        public float StoppingDistance { get => _stoppingDistance; set => _stoppingDistance = value; }
        public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
        public float LastAttackTime { get => _lastAttackTime; set => _lastAttackTime = value; }
        public IDamageable Target { get => _target; set => _target = value; }
        public Vector3 TargetPosition { get => _targetPosition; set => _targetPosition = value; }

        #endregion

        #region MonoBehaviour Functions

        private void OnEnable()
        {
            _transform = transform;
            _currentHp = _initialHp;
            ConfigureFSM();
        }

        #endregion

        #region Private Functions


        private void OnTriggerEnter2D(Collider2D collision) // Turn off on mainbase touch, just for testing
        {
            if (collision.gameObject.CompareTag("MainBase"))
            {
                PoolManager.Instance.Despawn(this);
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Configures the FSM states and transitions.
        /// </summary>
        public override void ConfigureFSM()
        {
            var walking = new State<EnemyStates>("WalkingState");
            var attacking = new State<EnemyStates>("AttackState");

            StateConfigurer.Create(walking)
                .SetTransition(EnemyStates.Attack, attacking)
                .Done();

            StateConfigurer.Create(attacking)
                .SetTransition(EnemyStates.Walking, walking)
                .Done();

            walking.OnUpdate += () => _walkingStrategy.OnUpdate(this);

            attacking.OnEnter += x => _attackStrategy.OnEnter(this);
            attacking.OnUpdate += () => _attackStrategy.OnUpdate(this);
            attacking.OnExit += x => _attackStrategy.OnExit(this);

            _fsm = new EventFSM<EnemyStates>(walking);
        }

        /// <summary>
        /// Checks if there is a tower within attack range.
        /// </summary>
        public bool IsTowerInRange()
        {
            var hit = Physics2D.OverlapCircle(_transform.position, _attackRange, _towerLayer);
            if (hit != null)
            {
                _target = hit.GetComponent<IDamageable>();
                _targetPosition = hit.transform.position;
            }
            else
            {
                _target = null;
            }

            return _target != null;
        }
        public void TakeDamage(int damage)
        {
            _currentHp -= damage;

            if (_currentHp <= 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }

        #endregion

        #region Gizmos Methods 
        /// <summary>
        /// Draws the attack range in the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
        #endregion
    }
}