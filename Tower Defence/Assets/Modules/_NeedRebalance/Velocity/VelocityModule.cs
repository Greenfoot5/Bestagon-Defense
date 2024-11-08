using System;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UnityEngine;

namespace Modules.Velocity
{
    /// <summary>
    /// Increases the speed of bullets
    /// </summary>
    [CreateAssetMenu(fileName = "VelocityT0", menuName = "Modules/Velocity")]
    public class VelocityModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner), typeof(Smasher), typeof(Lancer) };

        [Header("Gunner & Shooter & Lancer")]
        [Tooltip("The percentage to modify the speed of the bullet by")]
        [SerializeField]
        private float bulletSpeedChange;
        [Tooltip("The percentage to modify the fire rate of the turret by")]
        [SerializeField]
        private float fireRateChange;
        
        [Header("Gunner & Shooter")]
        [Tooltip("The percentage to modify the range of the bullet by")]
        [SerializeField]
        private float rangeChange;
        
        
        [Header("Lancer")]
        [SerializeField]
        [Tooltip("The percentage to modify the range of the Lancer's bullet")]
        private float lancerBulletRangeChange;

        [Header("Smasher")]
        [SerializeField]
        [Tooltip("The percentage to modify the damage of the smasher by")]
        private float smasherDamageChange;
        [SerializeField]
        [Tooltip("The percentage to modify the fire rate of the smasher by")]
        private float smasherFireRateChange;
        [SerializeField]
        [Tooltip("The percentage to modify the range of the smasher by")]
        private float smasherRangeChange;

        /// <summary>
        /// Increases the bullet speed of a turret
        /// </summary>
        /// <param name="damager">The turret to apply the modifications to</param>
        public override void AddModule(Damager damager)
        {
            switch (damager)
            {
                case Smasher smasher:
                    smasher.damage.AddModifier(smasherDamageChange);
                    smasher.range.AddModifier(smasherRangeChange);
                    smasher.fireRate.AddModifier(smasherFireRateChange);
                    break;
                case Lancer lancer:
                    lancer.fireRate.AddModifier(fireRateChange);
                    lancer.bulletRange.AddModifier(lancerBulletRangeChange);
                    break;
                case Turret turret:
                    turret.fireRate.AddModifier(fireRateChange);
                    turret.range.AddModifier(rangeChange);
                    break;
            }

            damager.OnShoot += OnShoot;
        }
        
        /// <summary>
        /// Decreases the bullet speed
        /// </summary>
        /// <param name="damager"></param>
        public override void RemoveModule(Damager damager)
        {
            switch (damager)
            {
                case Smasher smasher:
                    smasher.damage.TakeModifier(smasherDamageChange);
                    smasher.range.TakeModifier(smasherRangeChange);
                    smasher.fireRate.TakeModifier(smasherFireRateChange);
                    break;
                case Lancer lancer:
                    lancer.fireRate.TakeModifier(fireRateChange);
                    lancer.bulletRange.TakeModifier(lancerBulletRangeChange);
                    break;
                case Turret turret:
                    turret.fireRate.TakeModifier(fireRateChange);
                    turret.range.TakeModifier(rangeChange);
                    break;
            }

            damager.OnShoot -= OnShoot;
        }
        
        /// <summary>
        /// Increases the speed of the bullet once fired
        /// </summary>
        /// <param name="bullet">The bullet to accelerate</param>
        private void OnShoot(Bullet bullet)
        {
            bullet.speed.AddModifier(bulletSpeedChange);
        }
    }
}