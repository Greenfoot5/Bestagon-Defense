using System;
using Turrets;
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

        [Header("Shooter Turret")]
        [Tooltip("The percentage to modify the range of the Shooter by")]
        [SerializeField]
        private float shooterRangePercentageChange;
        [Tooltip("The percentage to modify the damage of the Shooter by")]
        [SerializeField]
        private float shooterDamagePercentageChange;
        [Tooltip("The percentage to modify the fire rate of the Shooter by")]
        [SerializeField]
        private float shooterFireRatePercentageChange;
        [Tooltip("The percentage to modify the rotation speed of the shooter by")]
        [SerializeField]
        private float shooterRotationSpeedPercentageChange;
        
        [Header("Gunner Turret")]
        [Tooltip("The percentage to modify the range of the Gunner by")]
        [SerializeField]
        private float gunnerRangePercentageChange;
        [Tooltip("The percentage to modify the damage of the Gunner by")]
        [SerializeField]
        private float gunnerDamagePercentageChange;
        [Tooltip("The percentage to modify the fire rate of the Gunner by")]
        [SerializeField]
        private float gunnerFireRatePercentageChange;
        [Tooltip("The percentage to modify the rotation speed of the Gunner by")]
        [SerializeField]
        private float gunnerSpinUpPercentageChange;
        [Tooltip("The percentage to modify the fire rate of the Gunner by")]
        [SerializeField]
        private float gunnerSpinDownPercentageChange;
        [Tooltip("The percentage to modify the rotation speed of the Gunner by")]
        [SerializeField]
        private float gunnerMaxFireRatePercentageChange;

        [Header("Laser Turret")]
        [Tooltip("The percentage to modify the range of the Laser by")]
        [SerializeField]
        private float laserRangePercentageChange;
        [Tooltip("The percentage to modify the damage of the Laser by")]
        [SerializeField]
        private float laserDamagePercentageChange;
        [Tooltip("The percentage to modify the rotation speed of the Laser by")]
        [SerializeField]
        private float laserRotationSpeedPercentageChange;
        [Tooltip("The percentage to modify the Laser cooldown by")]
        [SerializeField]
        private float laserLaserCooldown;
        [Tooltip("The percentage to modify the Laser duration by")]
        [SerializeField]
        private float laserLaserDuration;

        [Header("Lancer Turret")]
        [Tooltip("The percentage to modify the range of the Lancer by")]
        [SerializeField]
        private float lancerRangePercentageChange;
        [Tooltip("The percentage to modify the damage of the Lancer by")]
        [SerializeField]
        private float lancerDamagePercentageChange;
        [Tooltip("The percentage to modify the fire rate of the Lancer by")]
        [SerializeField]
        private float lancerFireRatePercentageChange;
        [Tooltip("The percentage to modify the Lancer shot distance by")]
        [SerializeField]
        private float lancerShotDistanceChange;
        [Tooltip("The percentage to modify the Lancer shot knockback by")]
        [SerializeField]
        private float lancerShotKnockbackChange;
        
        /// <summary>
        /// Modifies a turret's stats
        /// </summary>
        /// <param name="damager">The turret's stats to modify</param>
        public override void AddModule(Damager damager)
        {
            switch (damager)
            {
                // Modify the Shooter stats
                case Shooter shooter:
                    damager.damage.AddModifier(shooterDamagePercentageChange);
                    shooter.range.AddModifier(shooterRangePercentageChange);
                    shooter.fireRate.AddModifier(shooterFireRatePercentageChange);
                    shooter.rotationSpeed.AddModifier(shooterRotationSpeedPercentageChange);
                    break;
                // Modify the Smasher's stats
                case Gunner gunner:
                    damager.damage.AddModifier(gunnerDamagePercentageChange);
                    gunner.range.AddModifier(gunnerRangePercentageChange);
                    gunner.fireRate.AddModifier(gunnerFireRatePercentageChange);
                    gunner.spinMultiplier.AddModifier(gunnerSpinUpPercentageChange);
                    gunner.spinCooldown.AddModifier(gunnerSpinDownPercentageChange);
                    gunner.maxFireRate.AddModifier(gunnerMaxFireRatePercentageChange);
                    break;
                // Modify the Laser's stats
                case Laser laser:
                    damager.damage.AddModifier(laserDamagePercentageChange);
                    laser.range.AddModifier(laserRangePercentageChange);
                    laser.rotationSpeed.AddModifier(laserRotationSpeedPercentageChange);
                    laser.laserCooldown.AddModifier(laserLaserCooldown);
                    laser.laserDuration.AddModifier((laserLaserDuration));
                    break;
                // Modify the Lancer's stats
                case Lancer lancer:
                    damager.damage.AddModifier(lancerDamagePercentageChange);
                    lancer.range.AddModifier(lancerRangePercentageChange);
                    lancer.fireRate.AddModifier(lancerFireRatePercentageChange);
                    lancer.bulletRange.AddModifier(lancerShotDistanceChange);
                    damager.OnShoot += OnShoot;
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
                // Modify the Shooter stats
                case Shooter shooter:
                    damager.damage.AddModifier(shooterDamagePercentageChange);
                    shooter.range.AddModifier(shooterRangePercentageChange);
                    shooter.fireRate.AddModifier(shooterFireRatePercentageChange);
                    shooter.rotationSpeed.AddModifier(shooterRotationSpeedPercentageChange);
                    break;
                // Modify the Smasher's stats
                case Gunner gunner:
                    damager.damage.AddModifier(gunnerDamagePercentageChange);
                    gunner.range.AddModifier(gunnerRangePercentageChange);
                    gunner.fireRate.AddModifier(gunnerFireRatePercentageChange);
                    gunner.spinMultiplier.AddModifier(gunnerSpinUpPercentageChange);
                    gunner.spinCooldown.AddModifier(gunnerSpinDownPercentageChange);
                    gunner.maxFireRate.AddModifier(gunnerMaxFireRatePercentageChange);
                    break;
                // Modify the Laser's stats
                case Laser laser:
                    damager.damage.AddModifier(laserDamagePercentageChange);
                    laser.range.AddModifier(laserRangePercentageChange);
                    laser.rotationSpeed.AddModifier(laserRotationSpeedPercentageChange);
                    laser.laserCooldown.AddModifier(laserLaserCooldown);
                    laser.laserDuration.AddModifier((laserLaserDuration));
                    break;
                // Modify the Lancer's stats
                case Lancer lancer:
                    damager.damage.AddModifier(lancerDamagePercentageChange);
                    lancer.range.AddModifier(lancerRangePercentageChange);
                    lancer.fireRate.AddModifier(lancerFireRatePercentageChange);
                    lancer.bulletRange.AddModifier(lancerShotDistanceChange);
                    damager.OnShoot -= OnShoot;
                    break;
            }
        }

        /// <summary>
        /// Applies stat modifications when the bullet when fired
        /// </summary>
        /// <param name="bullet">The bullet to modify</param>
        private void OnShoot(Bullet bullet)
        {
            bullet.knockbackAmount.AddModifier(lancerShotKnockbackChange);
        }
    }
}