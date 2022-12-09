using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [CreateAssetMenu(fileName = "ExecuteT0", menuName = "ModuleTiers/Execute")]
    public class ExecuteModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner), typeof(Lancer) };
        
        [Tooltip("The maximum percentage of health the enemy can have before they get executed")]
        [SerializeField]
        private float percentageHealthRemaining;
        
        /// <summary>
        /// Check if the module can be applied to the turret
        /// The turret must be a valid type
        /// The turret cannot already have the execute module applied
        /// </summary>
        /// <param name="turret">The turret the module might be applied to</param>
        /// <returns>If the module can be applied</returns>
        public override bool ValidModule(Turret turret)
        {
            return turret.moduleHandlers.All(module => module.GetType() != typeof(ExecuteModule))
                   && ((IList)ValidTypes).Contains(turret.GetType());
        }
        
        /// <summary>
        /// Adds the EnemyAbility to some target(s)
        /// </summary>
        /// <param name="targets">The target(s) to apply the ability to</param>
        /// <param name="turret">The turret that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        public override void OnHit(IEnumerable<Enemy> targets, Turret turret, Bullet bullet = null)
        {
            foreach (Enemy target in targets)
            {
                if ((target.health / target.maxHealth) <= percentageHealthRemaining)
                {
                    target.TakeDamage(target.maxHealth, null);
                }
            }
        }
    }
}