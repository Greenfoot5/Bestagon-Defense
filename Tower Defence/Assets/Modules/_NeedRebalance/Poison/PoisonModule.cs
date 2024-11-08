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

namespace Modules.Poison
{
    /// <summary>
    /// Extends the Module class to create a DebuffEnemy upgrade,
    /// Used to add effects to enemies
    /// </summary>
    [CreateAssetMenu(fileName = "PoisonT0", menuName = "Modules/Poison")]
    public class PoisonModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };
        
        [SerializeField]
        [Tooltip("The damage to deal to an enemy every tick")]
        private float poisonDamage;
        [SerializeField]
        [Tooltip("How many ticks to burn the enemy for")]
        private int tickCount;
        [SerializeField]
        [Tooltip("How long each tick is in seconds")]
        private float tickDuration;
        
        [SerializeField]
        [Tooltip("The VFX to spawn each time a tick passes")]
        // ReSharper disable once NotAccessedField.Local
        private GameObject tickEffect;

        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
        }

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
            Runner.Run(PoisonEnemy(target));
        }
        
        /// <summary>
        /// Handles the poison effect
        /// </summary>
        /// <param name="target">The enemy to poison</param>
        private IEnumerator PoisonEnemy(Enemy target)
        {
            // Check the enemy is immune
            if (target.uniqueEffects.Contains("Poison"))
            {
                yield break;
            }
            
            // Loop until we've gone through every tick
            int ticksLeft = tickCount;
            while (ticksLeft > 0)
            {
                // Wait 1 tick
                yield return new WaitForSeconds(tickDuration);
                
                // Check we still have a target
                if (target == null)
                    yield break;
                
                // Deal Damage
                target.TakeDamageWithoutAbilities(poisonDamage);
                ticksLeft--;
                
                // Spawn ability effect
                // GameObject effect = Instantiate(tickEffect, target.transform.position, Quaternion.identity);
                // effect.name = "_" + effect.name;
                // Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
            }
        }
    }
}