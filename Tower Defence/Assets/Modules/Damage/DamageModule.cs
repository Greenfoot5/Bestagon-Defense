using System;
using Turrets;
using UnityEngine;

namespace Modules.Damage
{
    /// <summary>
    /// Extends the Module class to create a Damage upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "DamageT0", menuName = "Modules/Damage")]
    public class DamageModule : Module
    {
        protected override Type[] ValidTypes => null;  // any
        
        [Tooltip("What percentage to modify the damage by")]
        [SerializeField]
        private float percentageChange;
        
        /// <summary>
        /// Increases the damage for a turret
        /// </summary>
        /// <param name="damager">The turret to increase damage for</param>
        public override void AddModule(Damager damager)
        {
            damager.damage.AddModifier(percentageChange);
        }
        
        /// <summary>
        /// Removes a damage upgrade for a turret
        /// </summary>
        /// <param name="damager">The turret to remove a damage upgrade for</param>
        public override void RemoveModule(Damager damager)
        {
            damager.damage.TakeModifier(percentageChange);
        }
    }
}