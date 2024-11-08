using UnityEngine;
using UnityEngine.VFX;

namespace Turrets.Shooter
{
    /// <summary>
    /// Extends DynamicTurret to add Shooting functionality.
    /// </summary>
    public class Shooter : DynamicTurret
    {
        // Bullets
        [Tooltip("The bullet prefab to spawn each attack")]
        [SerializeField]
        private GameObject bulletPrefab;

        [Tooltip("The effect to fire when the bullet is shot")]
        [SerializeField]
        private VisualEffect attackEffect;

        /// <summary>
        /// Rotates towards the target if the turret have one.
        /// Shoots if the turret is looking towards the target
        /// </summary>
        private void Update()
        {
            // If there's no fire rate, the turret shouldn't do anything
            if (fireRate.GetStat() == 0)
            {
                return;
            }
            
            // Don't do anything if the turret doesn't have a target
            if (target == null)
            {
                fireCountdown -= Time.deltaTime;
                return;
            }
        
            // Rotates the turret each frame
            LookAtTarget();

            if (!IsLookingAtTarget())
            {
                fireCountdown -= Time.deltaTime;
                return;
            }
            
            
            if (fireCountdown <= 0)
            {
                fireCountdown = 1 / fireRate.GetStat();
                Attack();
            }
            
            fireCountdown -= Time.deltaTime;
        }

        /// <summary>
        /// Create the bullet and give it a target
        /// </summary>
        protected override void Attack()
        {
            attackEffect.SetFloat("zRotation", -firePoint.rotation.eulerAngles.z);
            attackEffect.Play();
            // Creates the bullet
            GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bulletGo.name = "_" + bulletGo.name;
            var bullet = bulletGo.GetComponent<Bullet>();
            bullet.Seek(target, this);

            base.Attack(this);
        }
    }
}
