using System;
using Enemies;
using Turrets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Critical
{
    /// <summary>
    /// Chance to deal double damage
    /// </summary>
    [CreateAssetMenu(fileName = "CriticalT0", menuName = "Modules/Critical")]
    public class CriticalModule : Module
    {
        protected override Type[] ValidTypes => null;
        
        [Tooltip("Percentage chance to deal double damage")]
        [SerializeField]
        private float criticalChance;

        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
        }

        public override void RemoveModule(Damager damager)
        {
            damager.OnHit -= OnHit;
        }

        /// <summary>
        /// Attempts to deal double damage on all enemies hit
        /// </summary>
        /// <param name="target">The targets to attempt to critically strike</param>
        /// <param name="damager">The damager that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        public void OnHit(Enemy target, Damager damager, Bullet bullet = null)
        {
            if (damager is not Turret turret) return;
            if (!(Random.value < criticalChance / turret.fireRate.GetStat())) return;

            target.TakeDamageWithoutAbilities(bullet == null
                ? turret.damage.GetStat()
                : damager.damage.GetStat());
        }
    }
}