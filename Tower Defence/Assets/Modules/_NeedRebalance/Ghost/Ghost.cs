using System;
using System.Collections;
using System.Linq;
using Turrets;
using Turrets.Gunner;
using Turrets.Shooter;
using Modules.Bombs;
using Modules.Missiles;
using UnityEngine;

namespace Modules.Ghost
{
    /// <summary>
    /// Grants bullets ethereal properties
    /// </summary>
    [CreateAssetMenu(fileName = "GhostT0", menuName = "Modules/Ghost")]
    public class Ghost : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Gunner) };

        public override void AddModule(Damager damager)
        {
            damager.OnShoot += OnShoot;
        }

        public override void RemoveModule(Damager damager)
        {
            damager.OnShoot -= OnShoot;
        }

        /// <summary>
        /// Check if the module can be applied to the turret
        /// The turret must be a valid type
        /// The turret cannot already have the ghost module applied
        /// </summary>
        /// <param name="damager">The turret the module might be applied to</param>
        /// <returns>If the module can be applied</returns>
        public override bool ValidModule(Damager damager)
        {
            return damager.moduleHandlers.All(module => (module.GetType() != typeof(BombsModule)
                                                 && module.GetType() != typeof(MissilesModule)))
                   && ((IList)ValidTypes).Contains(damager.GetType());
        }

        /// <summary>
        /// Makes the bullet ethereal
        /// </summary>
        /// <param name="bullet">The bullet to modify</param>
        public void OnShoot(Bullet bullet)
        {
            bullet.isEthereal = true;
        }
    }
}
