using System.Collections.Generic;
using System.Linq;
using Abstract.Data;
using Enemies;
using UnityEngine;

namespace Turrets.Laser
{
    /// <summary>
    /// Extends DynamicTurret to add Laser functionality
    /// </summary>
    public class Laser : DynamicTurret
    {
        // Lasers
        [Tooltip("The line renderer that displays the laser")]
        [SerializeField]
        private LineRenderer lineRenderer;
        [SerializeField]
        [Tooltip("The particle effect that's spawned at the end of the laser's line")]
        private ParticleSystem impactEffect;

        // Laser Duration
        [Tooltip("How long the laser lasts before it goes on cooldown")]
        public UpgradableStat laserDuration = new(1f);
        /// <summary> How long left until the next attack </summary>
        public float durationCountdown;
        [Tooltip("How long the laser should go on cooldown for")]
        public UpgradableStat laserCooldown = new(1f);
        /// <summary> How long left until the next attack </summary>
        public float cooldownCountdown;

        /// <summary>
        /// Fires the laser if the turret have a target and are looking at them.
        /// Otherwise rotate to target if there is one.
        /// </summary>
        private void Update()
        {
            durationCountdown -= Time.deltaTime;
            
            if (cooldownCountdown >= 0 && durationCountdown <= 0)
            {
                cooldownCountdown -= Time.deltaTime;

                if (target != null)
                    LookAtTarget();
                
                if (!lineRenderer.enabled) return;
                
                lineRenderer.enabled = false;
                impactEffect.Stop();
                return;
            }
            
            // Don't do anything if the turret doesn't have a target
            // or fire rate is <= 0
            if (target == null || laserDuration.GetStat() < 0)
            {
                return;
            }
        
            // Rotates the turret each frame
            LookAtTarget();
            
            // One of the two laser timers expired
            if (durationCountdown <= 0)
            {
                if (IsLookingAtTarget())
                {
                    durationCountdown = laserDuration.GetStat();
                    cooldownCountdown = laserCooldown.GetStat();
                }

                return;
            }
            
            Attack();
        }
        
        /// <summary>
        /// Fires the laser towards the enemy and deals damage
        /// </summary>
        // TODO - Animate the laser slightly (make it pulse)
        protected override void Attack()
        {
            Vector3 triangleCentre = transform.position;
            
            // Get the end point of the line renderer
            Vector3 direction = (firePoint.up * range.GetStat());
            Vector3 endPosition = (direction + triangleCentre);
            
            // Get all enemies the laser hits
            var results = new List<Collider2D>();
            Physics2D.OverlapCapsule(direction/2 + triangleCentre, new Vector2(lineRenderer.endWidth, range.GetStat()), 
                CapsuleDirection2D.Vertical, transform.rotation.y, new ContactFilter2D().NoFilter(), results);
            List<Enemy> enemies = results.Select(result => result.transform.GetComponent<Enemy>()).ToList();
            enemies.RemoveAll(x => x == null);
            
            base.Attack(this);
            HitMany(enemies, this);
            
            // Deal damage to every enemy hit
            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamage(damage.GetStat() * Time.deltaTime, gameObject);
            }

            // Enable visuals
            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                impactEffect.Play();
            }
            
            // Set Laser positions
            Vector3 firePointPosition = transform.position;
            lineRenderer.SetPosition(0, firePointPosition);
            lineRenderer.SetPosition(1, endPosition);
            
            // Set impact effect rotation
            // Transform impactEffectTransform = impactEffect.transform;
            // var aimDir = (Vector3)((Vector2)firePointPosition - (Vector2)impactEffectTransform.position).normalized;
            // impactEffectTransform.rotation = Quaternion.LookRotation(aimDir);
            
            // Set impact effect position
            //impactEffectTransform.position = endPosition + aimDir * 0.2f;
        }
    }
}
