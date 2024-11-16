using System;
using Turrets;
using Turrets.Gunner;
using Turrets.Shooter;
using UnityEngine;

namespace Modules.Missiles
{
    /// <summary>
    /// Extends the Module class to create a MissileBulletModule upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "MissileBulletT0", menuName = "Modules/Missile Bullet")]
    public class MissilesModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner) };
        
        [SerializeField]
        [Tooltip("What percentage to modify the explosion radius of the bullet by")]
        private float explosionRadiusChange;
        [SerializeField]
        [Tooltip("The percentage to modify the damage of the turret by")]
        private float damagePercentageChange;
        [SerializeField]
        [Tooltip("The percentage to modify the fire rate of the turret by")]
        private float fireRatePercentageChange;
        [SerializeField]
        [Tooltip("The percentage to modify the speed of the bullet by")]
        private float speedPercentageChange;

        [Header("Gunner")]
        [SerializeField]
        [Tooltip("The percentage to modify gunner's fire rate cap by")]
        private float gunnerFireRateCapChange;
        [SerializeField]
        [Tooltip("The percentage to increase gunner's spin up speed by")]
        private float gunnerSpinUpChange;
        
        /// <summary>
        /// Applies the stat changes to the turret
        /// </summary>
        /// <param name="damager">The turret to modifies the stats for</param>
        public override void AddModule(Damager damager)
        {
            damager.OnShoot += OnShoot;
            damager.damage.AddModifier(damagePercentageChange);
            
            if (damager is not Turret turret) return;
            turret.fireRate.AddModifier(fireRatePercentageChange);

            if (turret is not Gunner gunner) return;
            gunner.spinMultiplier.AddModifier(gunnerSpinUpChange);
            gunner.maxFireRate.AddModifier(gunnerFireRateCapChange);
        }
        
        /// <summary>
        /// Removes the stats changes from a turret
        /// </summary>
        /// <param name="damager">The turret to remove the stats changes for</param>
        public override void RemoveModule(Damager damager)
        {
            damager.OnShoot -= OnShoot;
            damager.damage.TakeModifier(damagePercentageChange);
            
            if (damager is not Turret turret) return;
            turret.fireRate.TakeModifier(fireRatePercentageChange);

            if (turret is not Gunner gunner) return;
            gunner.spinMultiplier.TakeModifier(gunnerSpinUpChange);
            gunner.maxFireRate.TakeModifier(gunnerFireRateCapChange);
        }
        
        /// <summary>
        /// Applies stat changes to a bullet
        /// </summary>
        /// <param name="bullet">The bullet to modify</param>
        private void OnShoot(Bullet bullet)
        {
            bullet.explosionRadius.AddModifier(explosionRadiusChange);
            bullet.speed.AddModifier(speedPercentageChange);
        }
    }
}