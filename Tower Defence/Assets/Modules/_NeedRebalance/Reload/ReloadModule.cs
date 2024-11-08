using System;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules._NeedRebalace.Reload
{
    /// <summary>
    /// Chance to attack again
    /// </summary>
    [CreateAssetMenu(fileName = "ReloadT0", menuName = "Modules/Reload")]
    public class ReloadModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };
        
        [Tooltip("Percentage chance to deal attack again")]
        [SerializeField]
        private float reloadChance;

        public override void AddModule(Damager damager)
        {
            damager.OnAttack += OnAttack;
        }

        public override void RemoveModule(Damager damager)
        {
            damager.OnAttack -= OnAttack;
        }

        /// <summary>
        /// When attacking, checks to see if the turret should attack again
        /// </summary>
        /// <param name="damager">The turret that attacked</param>
        private void OnAttack(Damager damager)
        {
            if (damager is not Turret turret) return;
            
            if (Random.value < (reloadChance / turret.fireRate.GetStat()))
            {
                // We don't want to instantly fire again, we want a slight delay to make it clear the turret has attacked again
                turret.fireCountdown *= 0.1f;
            }
        }
    }
}