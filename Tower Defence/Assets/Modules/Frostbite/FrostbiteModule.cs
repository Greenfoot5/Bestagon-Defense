using System;
using Abstract;
using Enemies;
using Turrets;
using Turrets.Choker;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using UnityEngine;

namespace Modules.Frostbite
{
    /// <summary>
    /// Extends the Module class to create a Damage upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "FrostbiteT0", menuName = "Modules/Frostbite")]
    public class FrostbiteModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Choker), typeof(Gunner), typeof(Lancer), typeof(Shooter) };
        
        [Tooltip("Multiplicative modifier to damage based on seconds left of slow")]
        [SerializeField]
        private float damageMultiplier;
        [Tooltip("Multiplicative percentage modifier to bullet explosion radius")]
        [SerializeField]
        private float bulletExplosionRadius;
        [Tooltip("Slow effect of any level (used to get effect key)")]
        [SerializeField]
        private EnemyEffect enemyEffect;
        
        /// <summary>
        /// Changes the turret's stats when added
        /// </summary>
        /// <param name="damager">The turret to change stats for</param>
        public override void AddModule(Damager damager)
        {
            damager.OnShoot += OnShoot;
            damager.OnHit += OnHit;
        }
        
        /// <summary>
        /// Removes stat modifications for a turret
        /// </summary>
        /// <param name="damager">The turret to revert stat changes for</param>
        public override void RemoveModule(Damager damager)
        {
            damager.OnShoot -= OnShoot;
            damager.OnHit -= OnHit;
        }
        
        /// <summary>
        /// Modifies the stats of a bullet when fired
        /// </summary>
        /// <param name="bullet">The bullet to add stats for</param>
        private void OnShoot(Bullet bullet)
        {
            bullet.explosionRadius.MultiplyModifier(bulletExplosionRadius);
        }

        /// <summary>
        /// Remove the slow effect and deal damage
        /// </summary>
        private void OnHit(Enemy enemy, Damager damager, Bullet bullet)
        {
            if (!enemy.ActiveEffects.TryGetValue(enemyEffect.effectType, out EnemyEffect effect)) return;
            
            bullet.damage.MultiplyModifier(damageMultiplier * effect.ticksLeft * effect.tickDuration);
            effect.isCancelled = true;
        }
    }
}