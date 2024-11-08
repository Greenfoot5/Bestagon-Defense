using System;
using Turrets;
using Turrets.Lancer;
using UnityEngine;

namespace Modules.Range
{
    /// <summary>
    /// Increases the range of a turret
    /// </summary>
    [CreateAssetMenu(fileName = "RangeModule", menuName = "Modules/Range")]
    public class RangeModule : Module
    {
        protected override Type[] ValidTypes => new [] {typeof(Turret)};  // any
        
        [Tooltip("The percentage to modify the range of the turret by")]
        [SerializeField]
        private float percentageChange;
        
        /// <summary>
        /// Increases the range of a turret
        /// </summary>
        /// <param name="damager">The turret to increase range for</param>
        public override void AddModule(Damager damager)
        {
            switch (damager)
            {
                case Lancer lancer:
                    lancer.bulletRange.AddModifier(percentageChange);
                    lancer.range.AddModifier(percentageChange);
                    break;
                case Turret turret:
                    turret.range.AddModifier(percentageChange);
                    break;
            }
        }
        
        /// <summary>
        /// Removes the range increase from a turret
        /// </summary>
        /// <param name="damager">The turret to decrease the range for</param>
        public override void RemoveModule(Damager damager)
        {
            switch (damager)
            {
                case Lancer lancer:
                    lancer.bulletRange.TakeModifier(percentageChange);
                    lancer.range.TakeModifier(percentageChange);
                    break;
                case Turret turret:
                    turret.range.TakeModifier(percentageChange);
                    break;
            }
        }
    }
}
