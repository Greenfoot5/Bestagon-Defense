using System;
using System.Collections;
using Abstract;
using Enemies;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UnityEngine;

namespace Modules.Slow
{
    /// <summary>
    /// Extends the Module class to create a DebuffEnemy upgrade,
    /// Used to add effects to enemies
    /// </summary>
    [CreateAssetMenu(fileName = "SlowT0", menuName = "Modules/Slow")]
    public class SlowModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };
        
        [SerializeField]
        [Tooltip("The percentage the slow the enemy's movement speed")]
        private float slowPercentage;
        
        [SerializeField]
        [Tooltip("How long each slow stack should last")]
        private float duration;

        /// <summary>
        /// Modifies the stats of the turret when applied
        /// </summary>
        /// <param name="damager">The turret to modify the stats for</param>
        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
        }
        
        /// <summary>
        /// Removes stats modifications of the turret
        /// </summary>
        /// <param name="damager">The turrets to remove the modifications of</param>
        public override void RemoveModule(Damager damager)
        {
            damager.OnHit -= OnHit;
        }

        /// <summary>
        /// Adds the EnemyAbility to some target(s)
        /// </summary>
        /// <param name="target">The target(s) to apply the ability to</param>
        /// <param name="damager">The turret that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        private void OnHit(Enemy target, Damager damager, Bullet bullet = null)
        {
            if (damager is not Turret) return;
            Runner.Run(SlowEnemy(target));
        }
        
        /// <summary>
        /// Applies the slow effect for a set duration
        /// </summary>
        /// <param name="target">The enemy to slow</param>
        private IEnumerator SlowEnemy(Enemy target)
        {
            // Check the enemy is immune
            if (target.uniqueEffects.Contains("Slow"))
            {
                yield break;
            }
            
            float slowValue = 1f - slowPercentage;

            if (target.speed.GetModifier() * slowValue <= 0.4f)
            {
                yield break;
            }
            
            target.speed.MultiplyModifier(slowValue);

            yield return new WaitForSeconds(duration);

            target.speed.DivideModifier(slowValue);
        }
    }
}
