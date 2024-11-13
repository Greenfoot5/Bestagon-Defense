using System;
using Turrets;
using Turrets.Choker;
using Turrets.Gunner;
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
        // Choker - Fewer shots, are explosive
        // Gunner - Small explosive bullets
        // Lancer - No change
        // Laser - No effect
        // Shooter - Targets location, significant explosion, slight slow of bullet speed
        // Smasher - Larger range & damage
        protected override Type[] ValidTypes => new[] { typeof(Choker), typeof(Gunner), typeof(Shooter), typeof(Smasher)};

        [Header("Choker")]
        [Tooltip("Additive percentage modifier to part explosion radius")]
        [SerializeField]
        private float chokerExplosionRadiusChange;
        [Tooltip("Additive percentage modifier to part count")]
        [SerializeField]
        private float chokerBulletCountChange;
        [Tooltip("Additive percentage modifier to choker damage")]
        [SerializeField]
        private float chokerDamageChange;

        [Header("Gunner")]
        [Tooltip("Additive percentage modifier to pellet explosion radius")]
        [SerializeField]
        private float gunnerExplosionRadiusChange;
        [Tooltip("Additive percentage modifier to gunner damage")]
        [SerializeField]
        private float gunnerDamageChange;

        [Header("Shooter")]
        [Tooltip("Additive percentage modifier to bullet explosion radius")]
        [SerializeField]
        private float shooterExplosionRadiusChange;
        [Tooltip("Additive percentage modifier to bullet speed")]
        [SerializeField]
        private float shooterBulletSpeedChange;
        [Tooltip("Additive percentage modifier to shooter damage")]
        [SerializeField]
        private float shooterDamageChange;
        
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
                case Choker choker:
                    choker.partCount.AddModifier(chokerBulletCountChange);
                    choker.damage.AddModifier(chokerDamageChange);
                    break;
                case Gunner gunner:
                    gunner.damage.AddModifier(gunnerDamageChange);
                    break;
                case Shooter shooter:
                    shooter.damage.AddModifier(shooterDamageChange);
                    break;
                case Smasher smasher:
                    smasher.damage.AddModifier(smasherDamageChange);
                    smasher.range.AddModifier(smasherRangeChange);
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
                case Choker choker:
                    choker.partCount.TakeModifier(chokerBulletCountChange);
                    choker.damage.TakeModifier(chokerDamageChange);
                    break;
                case Gunner gunner:
                    gunner.damage.TakeModifier(gunnerDamageChange);
                    break;
                case Shooter shooter:
                    shooter.damage.TakeModifier(shooterDamageChange);
                    break;
                case Smasher smasher:
                    smasher.damage.TakeModifier(smasherDamageChange);
                    smasher.range.TakeModifier(smasherRangeChange);
                    break;
            }
        }
        
        /// <summary>
        /// Modifies the stats of a bullet when fired
        /// </summary>
        /// <param name="bullet">The bullet to add stats for</param>
        private void OnShoot(Bullet bullet)
        {
            switch (bullet.source)
            {
                case Choker:
                    bullet.explosionRadius.AddModifier(chokerExplosionRadiusChange);
                    break;
                case Gunner:
                    bullet.explosionRadius.AddModifier(gunnerExplosionRadiusChange);
                    break;
                case Shooter:
                    bullet.explosionRadius.TakeModifier(shooterExplosionRadiusChange);
                    bullet.speed.AddModifier(shooterBulletSpeedChange);
                    bullet.useLocation = true;
                    bullet.targetLocation = bullet.target.position;
                    break;
            }
        }
    }
}