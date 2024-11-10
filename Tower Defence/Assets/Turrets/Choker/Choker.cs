using Abstract.Data;
using UnityEngine;
using UnityEngine.VFX;

namespace Turrets.Choker
{
    /// <summary>
    /// Extends DynamicTurret to add Shooting functionality.
    /// </summary>
    public class Choker : DynamicTurret
    {
        // Bullets
        [Tooltip("The bullet prefab to spawn each attack")]
        [SerializeField]
        private GameObject bulletPrefab;
        [Tooltip("The effect to fire when the bullet is shot")]
        [SerializeField]
        private VisualEffect attackEffect;
        
        [Tooltip("The range at which to spread out the bullets")]
        public UpgradableStat partSpread = new(0.5f);
        [Tooltip("How many parts should be fired")]
        public UpgradableStat partCount = new(10);

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
            if (Target is null)
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
            //attackEffect?.Play();

            // Any decimal count is a chance for an additional part
            int count = ((int)partCount.GetStat()) + (Random.value < partCount.GetStat() % 10 ? 1 : 0);
            
            // float oneSegment = spreadSize / (spreadAmount - 1);
            for (var i = 0; i < count; i++)
            {
                Transform bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).transform;
                bullet.name = "_" + bullet.name;
                
                float bulletAngle = firePoint.eulerAngles.z + Random.Range(partSpread.GetStat() / -2, partSpread.GetStat() / 2);
                Debug.Log(bulletAngle);
                bullet.eulerAngles = new Vector3(0, 0, bulletAngle);

                float radian = (bulletAngle + 90) * Mathf.Deg2Rad;
                var bulletDirection = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                //bullet.position = 
                bullet.GetComponent<Bullet>().Seek((Vector2)firePoint.position + bulletDirection * range.GetStat(), this);
            }
            
            base.Attack(this);
        }
    }
}
