using System;
using Enemies;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Modules.Instakill
{
    /// <summary>
    /// Chance to instakill an enemy
    /// </summary>
    [CreateAssetMenu(fileName = "InstakillT0", menuName = "Modules/Instakill")]
    public class InstakillModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };
        
        [FormerlySerializedAs("percentageChange")]
        [Tooltip("The percentage chance to kill the enemy")]
        [SerializeField]
        private float instakillChance;
        
        [Tooltip("The effect to play on kill")]
        [SerializeField]
        private GameObject instakillEffect;

        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
        }

        public override void RemoveModule(Damager damager)
        {
            damager.OnHit -= OnHit;
        }

        /// <summary>
        /// Attempt to instakill all hit enemies
        /// </summary>
        /// <param name="target">The targets to attempt to instakill</param>
        /// <param name="damager">The turret that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        private void OnHit(Enemy target, Damager damager, Bullet bullet = null)
        {
            if (damager is not Turret turret) return;
            if (!(Random.value < (instakillChance / turret.fireRate.GetStat())) || !(target.health > 0) ||
                target.isBoss) return;
            
            GameObject effect = Instantiate(instakillEffect);
            effect.transform.position = target.transform.position;
            Destroy(effect, 1f);
            target.TakeDamage(target.maxHealth, null);
        }
    }
}