using System;
using System.Linq;
using Turrets;
using UnityEngine;

namespace Modules
{
    /// <summary>
    /// A base abstract class to create a turret module from.
    /// </summary>
    public abstract class Module : ScriptableObject
    {
        [Tooltip("The tier number of the module")]
        public int moduleTier;
        [Tooltip("If the module can be obtained through upgrades")]
        public bool isUpgradableTo = true;

        protected abstract Type[] ValidTypes { get; }

        /// <summary>
        /// Checks if a DamagerObject is of a valid type for the module to work
        /// </summary>
        /// <param name="damager">The DamagerObject to check</param>
        /// <returns>If the DamagerObject is in the valid types</returns>
        public virtual bool ValidModule(Damager damager)
        {
            return ValidTypes.Any(type => type.IsInstanceOfType(damager));
        }
        
        /// <summary>
        /// Gets all the turrets that the module can be applied to
        /// </summary>
        /// <returns>A list of valid turret types</returns>
        public Type[] GetValidTypes()
        {
            return ValidTypes;
        }

        /// <summary>
        /// Called when a module is added to a damager
        /// </summary>
        /// <param name="damager">The damager to modify</param>
        public abstract void AddModule(Damager damager);

        /// <summary>
        /// Called when a module is removed from a turret
        /// </summary>
        /// <param name="damager">The turret to modify</param>
        public abstract void RemoveModule(Damager damager);
    }
}