using System;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UnityEngine;

namespace Modules.Bombs
{
    /// <summary>
    /// Extends the Module class to create a BombBullet upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "BombsT0", menuName = "Modules/Bombs")]
    public class BombsModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner), typeof(Smasher), typeof(Lancer)};
        
        [Header("Shooter, Gunner & Lancer")]
        [Tooltip("What percentage to modify the explosion radius of bullets by")]
        [SerializeField]
        private float explosionRadiusChange;
        [Tooltip("What percentage to modify the damage of the turret by")]
        [SerializeField]
        private float damagePercentageChange;
        [Tooltip("What percentage to modify the fire rate of the turret by")]
        [SerializeField]
        private float fireRatePercentageChange;
        [Tooltip("What percentage to modify the range of the turret by")]
        [SerializeField]
        private float rangePercentageChange;
        [Tooltip("What percentage to modify the speed of the bullet by")]
        [SerializeField]
        private float speedPercentageChange;
        
        [Header("Lancer Only")]
        [Tooltip("The percentage to modify the knockback of the bullet")]
        [SerializeField]
        private float knockbackPercentageChange;

        [Header("Smasher")]
        [SerializeField]
        [Tooltip("The percentage to modify the damage of smasher by")]
        private float smasherDamageChange;
        [SerializeField]
        [Tooltip("The percentage to modify the range of smasher by")]
        private float smasherRangeChange;
        
        /// <summary>
        /// Changes the turret's stats when added
        /// </summary>
        /// <param name="damager">The turret to change stats for</param>
        public override void AddModule(Damager damager)
        {
            damager.OnShoot += OnShoot;
            switch (damager)
            {
                case Smasher smasher:
                    smasher.damage.AddModifier(smasherDamageChange);
                    smasher.range.AddModifier(smasherRangeChange);
                    break;
                case Turret turret:
                    turret.damage.AddModifier(damagePercentageChange);
                    turret.fireRate.AddModifier(fireRatePercentageChange);
                    turret.range.AddModifier(rangePercentageChange);
                    break;
            }
        }
        
        /// <summary>
        /// Removes stat modifications for a turret
        /// </summary>
        /// <param name="damager">The turret to revert stat changes for</param>
        public override void RemoveModule(Damager damager)
        {
            damager.OnShoot -= OnShoot;
            switch (damager)
            {
                case Smasher smasher:
                    smasher.damage.TakeModifier(smasherDamageChange);
                    smasher.range.TakeModifier(smasherRangeChange);
                    break;
                case Turret turret:
                    turret.damage.TakeModifier(damagePercentageChange);
                    turret.fireRate.TakeModifier(fireRatePercentageChange);
                    turret.range.TakeModifier(rangePercentageChange);
                    break;
            }
        }
        
        /// <summary>
        /// Modifies the stats of a bullet when fired
        /// </summary>
        /// <param name="bullet">The bullet to add stats for</param>
        private void OnShoot(Bullet bullet)
        {
            bullet.explosionRadius.AddModifier(explosionRadiusChange);
            bullet.speed.AddModifier(speedPercentageChange);
            bullet.knockbackAmount.AddModifier(knockbackPercentageChange);
            if (bullet.useLocation) return;
            
            bullet.useLocation = true;
            bullet.targetLocation = bullet.target.position;
        }
    }
}