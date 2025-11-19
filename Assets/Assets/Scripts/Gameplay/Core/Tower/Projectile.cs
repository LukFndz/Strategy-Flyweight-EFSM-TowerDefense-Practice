using Assets.Scripts.Gameplay.Core.Interface;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Core.Towers
{
    public class Projectile : MonoBehaviour
    {
        #region Private Variables

        [Tooltip("Speed of the projectile")]
        [SerializeField] private float _speed = 5f;
        [Tooltip("Sprite Renderer of the projectile")]
        [SerializeField] private SpriteRenderer _spriteRender;
        private int _damage = 1;
        private Transform _target;
        #endregion

        #region MonoBehaviour Functions

        private void Update()
        {
            if (_target != null)
            {
                // Calcular la dirección hacia el enemigo
                Vector3 direction = (_target.position - transform.position).normalized;

                // Rotar el proyectil para que mire hacia el enemigo
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                // Mover el proyectil hacia el enemigo
                transform.position += direction * _speed * Time.deltaTime;

                // Si llega al enemigo, lo dañamos y destruimos el proyectil
                if (Vector3.Distance(transform.position, _target.position) < 0.1f)
                {
                    if (_target.TryGetComponent<IDamageable>(out IDamageable damageable))
                    {
                        damageable.TakeDamage(_damage);
                    }
                    PoolManager.Instance.Despawn(this);
                }
            }
            else
            {
                PoolManager.Instance.Despawn(this);
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Initialize the projectile with the target and damage.
        /// </summary>
        /// <param name="target"> The enemy</param>
        /// <param name="damage"> Damage to do at enemy collision</param>
        /// <param name="projectileSprite"> The projectile sprite</param>
        public void Initialize(Transform target, int damage, Sprite projectileSprite)
        {
            _target = target;
            _damage = damage;
            _spriteRender.sprite = projectileSprite;
        }

        #endregion

    }
}