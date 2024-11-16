using System;
using System.Collections;
using System.Linq;
using Abstract;
using Turrets;
using Turrets.Choker;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UI.Inventory;
using UnityEngine;

namespace Modules.Surge
{
    /// <summary>
    /// Grants a temporary fire rate increase to a turret
    /// </summary>
    [CreateAssetMenu(fileName = "SurgeT0", menuName = "Modules/Surge")]
    public class SurgeModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Lancer), typeof(Choker) };
        
        [Header("Effect Details")]
        
        [SerializeField]
        [Tooltip("How many ticks to burn the enemy for")]
        private int duration;
        [SerializeField]
        [Tooltip("How long each tick is in seconds")]
        private float cooldown;
        
        [SerializeField]
        [Tooltip("The VFX to spawn when the turret surges")]
        private GameObject surgeEffect;
        [SerializeField]
        [Tooltip("The VFX to spawn when the ends it's turret surge")]
        private GameObject surgeEndEffect;
        
        [Header("Shooter Surging")]
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to shooter's fire rate when surging")]
        private float surgeShooterFireRateChange;
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to shooter's damage when surging")]
        private float surgeShooterDamageChange;
        
        [Header("Smasher Surging")]
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to smasher's fire rate when surging")]
        private float surgeSmasherFireRateChange;
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to smasher's range when surging")]
        private float surgeSmasherRangeChange;
        
        [Header("Lancer Surging")]
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to lancer's fire rate when surging")]
        private float surgeLancerFireRateChange;
        // TODO - Get this to work
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to lancer's arrow knockback when surging")]
        private float surgeLancerKnockbackChange;
        
        [Header("Choker Surging")]
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to choker's fire rate when surging")]
        private float surgeChokerFireRateChange;
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to choker's part count when surging")]
        private float surgeChokerPartCountChange;
        
        
        [Header("Cooldown effect")]
        
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to part fire rate")]
        private float fireRateChange;
        [SerializeField]
        [Tooltip("Multiplicative percentage modifier to part damage")]
        private float damageChange;

        /// <summary>
        /// Begins the surge effect on the turret
        /// </summary>
        /// <param name="damager">The turret to start the surge loop on</param>
        public override void AddModule(Damager damager)
        {
            if (damager is not Turret turret) return;
            // LINQ to get the turret tier
            int tier = damager.moduleHandlers.Where(handler => handler.GetModule().GetType() == typeof(SurgeModule)).Select(handler => handler.GetTier()).FirstOrDefault();
            
            turret.fireRate.MultiplyModifier(fireRateChange);
            turret.damage.MultiplyModifier(damageChange);
            
            Runner.Run(Surge(turret, tier));
        }

        public override void RemoveModule(Damager damager)
        {
            if (damager is not Turret turret) return;
            turret.fireRate.DivideModifier(fireRateChange);
            turret.damage.DivideModifier(damageChange);
        }

        /// <summary>
        /// Handles the surge effect
        /// </summary>
        /// <param name="turret">The turret to increase the fire rate for</param>
        /// <param name="tier">The tier of the module</param>
        private IEnumerator Surge(Turret turret, int tier)
        {
            // Wait the cooldown
            yield return new WaitForSeconds(cooldown);
            
            while (turret != null && turret.moduleHandlers.Any(module => module.GetModule().GetType() == typeof(SurgeModule) && module.GetTier() == tier))
            {
                // SURGE!
                turret.fireRate.DivideModifier(fireRateChange);
                turret.damage.DivideModifier(damageChange);
                switch (turret)
                {
                    case Choker choker:
                        choker.fireRate.MultiplyModifier(surgeChokerFireRateChange);
                        choker.partCount.MultiplyModifier(surgeChokerPartCountChange);
                        break;
                    case Lancer lancer:
                        lancer.fireRate.MultiplyModifier(surgeLancerFireRateChange);
                        break;
                    case Shooter shooter:
                        shooter.fireRate.MultiplyModifier(surgeShooterFireRateChange);
                        shooter.damage.MultiplyModifier(surgeShooterDamageChange);
                        break;
                    case Smasher smasher:
                        smasher.fireRate.MultiplyModifier(surgeSmasherFireRateChange);
                        smasher.range.MultiplyModifier(surgeSmasherRangeChange);
                        turret.UpdateRange();
                        break;
                }
                TurretInfo.instance.UpdateStats();
                Vector3 position = turret.transform.position;
                GameObject effect = Instantiate(surgeEffect, position, Quaternion.identity);
                effect.name = "_" + effect.name;
                Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                

                yield return new WaitForSeconds(duration);
                
                turret.fireRate.MultiplyModifier(fireRateChange);
                turret.damage.MultiplyModifier(damageChange);
                switch (turret)
                {
                    case Choker choker:
                        choker.fireRate.DivideModifier(surgeChokerFireRateChange);
                        choker.partCount.DivideModifier(surgeChokerPartCountChange);
                        break;
                    case Lancer lancer:
                        lancer.fireRate.DivideModifier(surgeLancerFireRateChange);
                        break;
                    case Shooter shooter:
                        shooter.fireRate.DivideModifier(surgeShooterFireRateChange);
                        shooter.damage.DivideModifier(surgeShooterDamageChange);
                        break;
                    case Smasher smasher:
                        smasher.fireRate.DivideModifier(surgeSmasherFireRateChange);
                        smasher.range.DivideModifier(surgeSmasherRangeChange);
                        turret.UpdateRange();
                        break;
                }
                TurretInfo.instance.UpdateStats();
                GameObject endEffect = Instantiate(surgeEndEffect, position, Quaternion.identity);
                endEffect.name = "_" + endEffect.name;
                Destroy(endEffect, endEffect.GetComponent<ParticleSystem>().main.duration);
                
                // Wait the cooldown
                yield return new WaitForSeconds(cooldown);
            }
        }
    }
}