using System;
using Enemies;
using Turrets;
using Turrets.Choker;
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
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner), typeof(Lancer), typeof(Choker), typeof(Smasher) };

        [SerializeField]
        [Tooltip("The percentage the slow the enemy's movement speed")]
        private SlowEnemyEffect effect;

        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to damage")]
        private float damageChange;

        /// <summary>
        /// Modifies the stats of the turret when applied
        /// </summary>
        /// <param name="damager">The turret to modify the stats for</param>
        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
            damager.damage.MultiplyModifier(damageChange);
        }
        
        /// <summary>
        /// Removes stats modifications of the turret
        /// </summary>
        /// <param name="damager">The turrets to remove the modifications of</param>
        public override void RemoveModule(Damager damager)
        {
            damager.OnHit -= OnHit;
            damager.damage.DivideModifier(damageChange);
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
            SlowEnemyEffect newEffect = Instantiate(effect);
            newEffect.Apply(target);
        }
    }
}
