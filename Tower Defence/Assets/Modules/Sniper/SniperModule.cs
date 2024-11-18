using System;
using Turrets;
using Turrets.Choker;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Laser;
using Turrets.Shooter;
using UnityEngine;

namespace Modules.Sniper
{
    /// <summary>
    /// Extends the Module class to create a Sniper upgrade
    /// </summary>
    [CreateAssetMenu(fileName = "SniperT0", menuName = "Modules/Sniper")]
    public class SniperModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Laser), typeof(Gunner), typeof(Lancer) };

        [Header("Shooter")]
        [Tooltip("Additive percentage modifier to shooter's range")]
        [SerializeField]
        private float shooterRangeChange;
        [Tooltip("Additive percentage modifier to shooter's damage")]
        [SerializeField]
        private float shooterDamageChange;
        [Tooltip("Additive percentage modifier to shooter's fire rate")]
        [SerializeField]
        private float shooterFireRateChange;
        [Tooltip("Additive percentage modifier to bullet speed")]
        [SerializeField]
        private float shooterBulletSpeedChange;
        [Tooltip("Additive percentage modifier to bullet knockback")]
        [SerializeField]
        private float shooterBulletKnockbackChange;
        
        [Header("Lancer")]
        [Tooltip("Additive percentage modifier to lancer's range")]
        [SerializeField]
        private float lancerRangeChange;
        [Tooltip("Additive percentage modifier to lancer's damage")]
        [SerializeField]
        private float lancerDamageChange;
        [Tooltip("Additive percentage modifier to lancer's fire rate")]
        [SerializeField]
        private float lancerFireRateChange;
        [Tooltip("Additive percentage modifier to lancer's arrow range")]
        [SerializeField]
        private float lancerArrowRangeChange;
        [Tooltip("Additive percentage modifier to lancer's arrow speed")]
        [SerializeField]
        private float lancerArrowSpeedChange;
        [Tooltip("Additive percentage modifier to lancer's arrow knockback")]
        [SerializeField]
        private float lancerArrowKnockbackChange;

        [Header("Laser")]
        [Tooltip("Additive percentage modifier to laser's range")]
        [SerializeField]
        private float laserRangeChange;
        [Tooltip("Additive percentage modifier to laser's damage")]
        [SerializeField]
        private float laserDamageChange;
        [Tooltip("Multiplicative percentage modifier to laser's laser duration")]
        [SerializeField]
        private float laserLaserDuration;

        [Header("Choker")]
        [Tooltip("Additive percentage modifier to choker's range")]
        [SerializeField]
        private float chokerRangeChange;
        [Tooltip("Additive percentage modifier to choker's damage")]
        [SerializeField]
        private float chokerDamageChange;
        [Tooltip("Additive percentage modifier to choker's fire rate")]
        [SerializeField]
        private float chokerFireRateChange;
        [Tooltip("Multiplicative percentage modifier to choker's part spread")]
        [SerializeField]
        private float chokerPartSpreadChange;
        
        /// <summary>
        /// Modifies a turret's stats
        /// </summary>
        /// <param name="damager">The turret's stats to modify</param>
        public override void AddModule(Damager damager)
        {
            switch (damager)
            {
                // Modify the Shooter's stats
                case Shooter shooter:
                    damager.damage.AddModifier(shooterDamageChange);
                    shooter.range.AddModifier(shooterRangeChange);
                    shooter.fireRate.AddModifier(shooterFireRateChange);
                    damager.OnShoot += OnShoot;
                    break;
                // Modify the Lancer's stats
                case Lancer lancer:
                    damager.damage.AddModifier(lancerDamageChange);
                    lancer.range.AddModifier(lancerRangeChange);
                    lancer.fireRate.AddModifier(lancerFireRateChange);
                    lancer.bulletRange.AddModifier(lancerArrowRangeChange);
                    damager.OnShoot += OnShoot;
                    break;
                // Modify the Laser's stats
                case Laser laser:
                    damager.damage.AddModifier(laserDamageChange);
                    laser.range.AddModifier(laserRangeChange);
                    laser.laserDuration.MultiplyModifier(laserLaserDuration);
                    break;
                // Modify the Choker's stats
                case Choker choker:
                    damager.damage.AddModifier(chokerDamageChange);
                    choker.range.AddModifier(chokerRangeChange);
                    choker.fireRate.AddModifier(chokerFireRateChange);
                    choker.partSpread.MultiplyModifier(chokerPartSpreadChange);
                    break;
            }
        }
        
        /// <summary>
        /// Removes any stats modifications from the module
        /// </summary>
        /// <param name="damager">The turret to remove the modifications from</param>
        /// <exception cref="ArgumentOutOfRangeException">An invalid turret</exception>
        public override void RemoveModule(Damager damager)
        {
            switch (damager)
            {
                // Modify the Shooter's stats
                case Shooter shooter:
                    damager.damage.TakeModifier(shooterDamageChange);
                    shooter.range.TakeModifier(shooterRangeChange);
                    shooter.fireRate.TakeModifier(shooterFireRateChange);
                    damager.OnShoot -= OnShoot;
                    break;
                // Modify the Lancer's stats
                case Lancer lancer:
                    damager.damage.TakeModifier(lancerDamageChange);
                    lancer.range.TakeModifier(lancerRangeChange);
                    lancer.fireRate.TakeModifier(lancerFireRateChange);
                    lancer.bulletRange.TakeModifier(lancerArrowRangeChange);
                    damager.OnShoot -= OnShoot;
                    break;
                // Modify the Laser's stats
                case Laser laser:
                    damager.damage.TakeModifier(laserDamageChange);
                    laser.range.TakeModifier(laserRangeChange);
                    laser.laserDuration.DivideModifier(laserLaserDuration);
                    break;
                // Modify the Choker's stats
                case Choker choker:
                    damager.damage.TakeModifier(chokerDamageChange);
                    choker.range.TakeModifier(chokerRangeChange);
                    choker.fireRate.TakeModifier(chokerFireRateChange);
                    choker.partSpread.DivideModifier(chokerPartSpreadChange);
                    break;
            }
        }

        /// <summary>
        /// Applies stat modifications when the bullet when fired
        /// </summary>
        /// <param name="bullet">The bullet to modify</param>
        private void OnShoot(Bullet bullet)
        {
            switch (bullet.source)
            {
                case Shooter:
                    bullet.speed.AddModifier(shooterBulletSpeedChange);
                    bullet.knockbackAmount.AddModifier(shooterBulletKnockbackChange);
                    break;
                case Lancer:
                    bullet.speed.AddModifier(lancerArrowSpeedChange);
                    bullet.knockbackAmount.AddModifier(lancerArrowKnockbackChange);
                    break;
            }
        }
    }
}