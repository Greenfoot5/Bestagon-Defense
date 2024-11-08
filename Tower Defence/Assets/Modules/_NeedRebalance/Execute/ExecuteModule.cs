using System;
using System.Collections.Generic;
using Enemies;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using UnityEngine;

namespace Modules.Execute
{
    /// <summary>
    /// Extends the module class to create the execute module
    /// </summary>
    [CreateAssetMenu(fileName = "ExecuteT0", menuName = "Modules/Execute")]
    public class ExecuteModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner), typeof(Lancer) };
        
        [Tooltip("The maximum percentage of health the enemy can have before they get executed")]
        [SerializeField]
        private float percentageHealthRemaining;

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
            if ((target.health / target.maxHealth) <= percentageHealthRemaining)
            {
                target.TakeDamage(target.maxHealth, null);
            }
        }
    }
}