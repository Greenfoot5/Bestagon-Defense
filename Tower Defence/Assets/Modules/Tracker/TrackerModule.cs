using System;
using Turrets;
using Turrets.Gunner;
using Turrets.Laser;
using Turrets.Shooter;
using UnityEngine;

namespace Modules.Tracker
{
    /// <summary>
    /// Increases the rotation speed of a dynamic turret
    /// </summary>
    [CreateAssetMenu(fileName = "TrackerModuleT0", menuName = "Modules/Tracker")]
    public class TrackerModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Laser), typeof(Gunner) };

        [SerializeField]   
        [Tooltip("The percentage to modify the rotation speed of the turret by")]
        private float rotationSpeedPercentageChange;
        [SerializeField]
        [Tooltip("The percentage to modify the damage of the turret by")]
        private float damagePercentageChange;
        
        /// <summary>
        /// Increases the rotation speed of a turret
        /// </summary>
        /// <param name="damager">The turret to affect</param>
        public override void AddModule(Damager damager)
        {
            damager.damage.AddModifier(damagePercentageChange);
            if (damager is DynamicTurret turret)
                turret.rotationSpeed.AddModifier(rotationSpeedPercentageChange);
        }
        
        /// <summary>
        /// Removes the rotation speed increase of a turret
        /// </summary>
        /// <param name="damager">The turret to affect</param>
        public override void RemoveModule(Damager damager)
        {
            damager.damage.TakeModifier(damagePercentageChange);
            if (damager is DynamicTurret turret)
                turret.rotationSpeed.TakeModifier(rotationSpeedPercentageChange);
        }
    }
}
