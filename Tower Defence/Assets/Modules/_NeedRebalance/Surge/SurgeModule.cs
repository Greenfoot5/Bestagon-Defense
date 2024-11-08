using System;
using System.Collections;
using System.Linq;
using Abstract;
using Turrets;
using Turrets.Gunner;
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
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };
        
        [SerializeField]
        [Tooltip("The damage to deal to an enemy every tick")]
        private float fireRateChange;
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

        /// <summary>
        /// Begins the surge effect on the turret
        /// </summary>
        /// <param name="damager">The turret to start the surge loop on</param>
        public override void AddModule(Damager damager)
        {
            if (damager is not Turret turret) return;
            // LINQ to get the turret tier
            int tier = damager.moduleHandlers.Where(handler => handler.GetModule().GetType() == typeof(SurgeModule)).Select(handler => handler.GetTier()).FirstOrDefault();
            Runner.Run(Surge(turret, tier));
        }

        public override void RemoveModule(Damager damager)
        {
            
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
                turret.fireRate.AddModifier(fireRateChange);
                TurretInfo.instance.UpdateStats();
                Vector3 position = turret.transform.position;
                GameObject effect = Instantiate(surgeEffect, position, Quaternion.identity);
                effect.name = "_" + effect.name;
                Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
                

                yield return new WaitForSeconds(duration);
                
                turret.fireRate.TakeModifier(fireRateChange);
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