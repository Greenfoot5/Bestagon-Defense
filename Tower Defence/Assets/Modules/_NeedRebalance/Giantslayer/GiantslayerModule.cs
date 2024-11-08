using System;
using Enemies;
using Turrets;
using Turrets.Smasher;
using UnityEngine;

namespace Modules.Giantslayer
{
    /// <summary>
    /// Deals extra damage to bosses
    /// </summary>
    [CreateAssetMenu(fileName = "GiantslayerT0", menuName = "Modules/Giantslayer")]
    public class GiantslayerModule : Module
    {
        protected override Type[] ValidTypes => null;

        [SerializeField]
        [Tooltip("The percentage damage change if attacking a boss")]
        private float damagePercentage;

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
        /// <param name="damager">The turret that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        private void OnHit(Enemy target, Damager damager, Bullet bullet = null)
        {
            if (!target.isBoss) return;

            switch (damager)
            {
                case Smasher smasher:
                    float locationPercentage = 1 - (damager.transform.position - target.transform.position).sqrMagnitude /
                        (smasher.range.GetTrueStat() * smasher.range.GetTrueStat());
                    // Only deal damage if it will actually damage the enemy
                    if (locationPercentage > 0)
                    {
                        target.TakeDamageWithoutAbilities(damager.damage.GetTrueStat() * locationPercentage * damagePercentage);
                    }

                    break;
                case Turret:
                    target.TakeDamageWithoutAbilities(damager.damage.GetStat() * damagePercentage);
                    break;
            }
        }
    }
}