using System;
using UnityEngine;

namespace Abstract.Data
{
    /// <summary>
    /// A stat that is upgradable.
    /// This allows for additive upgrades in an easy manner
    /// </summary>
    [Serializable]
    public struct UpgradableStat
    {
        [SerializeField]
        private float stat;
        [SerializeField]
        private float modifier;
        
        /// <summary>
        /// Creates an Upgradable stat, with default 1f for modifier
        /// </summary>
        /// <param name="baseValue">The staring base value</param>
        public UpgradableStat(float baseValue)
        {
            stat = baseValue;
            modifier = 1f;
        }
    
        /// <summary>
        /// Gets the base value of the stat.
        /// If the value is less than 0, it will return 0
        /// </summary>
        /// <returns>The base value of the stat</returns>
        public float GetBase()
        {
            return stat <= 0f ? 0f : stat;
        }
        
        /// <summary>
        /// Gets the modifier of the stat
        /// </summary>
        /// <returns>The current modifier</returns>
        public float GetModifier()
        {
            return modifier;
        }

        /// <summary>
        /// Gets the stat after it's been modified
        /// If the value is less than 0, it will return 0
        /// </summary>
        /// <returns>The stat after being multiplied by the modifier</returns>
        public float GetStat()
        {
            return stat * modifier <= 0f ? 0f : stat * modifier;
        }
    
        /// <summary>
        /// Gets the stat after it's been modified
        /// If the value is less than 0, it will still return it's actual value
        /// </summary>
        /// <returns>The stat after being multiplied by the modifier</returns>
        public float GetTrueStat()
        {
            return stat * modifier;
        }
        
        /// <summary>
        /// Sets a new value for the base stat.
        /// </summary>
        /// <param name="newValue">The new value for the base stat</param>
        public void SetBase(float newValue)
        {
            stat = newValue;
        }
        
        /// <summary>
        /// Increases the stat's modifier by a certain amount
        /// Upgrades the stat in an additive way
        /// </summary>
        /// <param name="newValue">How much increase the modifier by</param>
        public void AddModifier(float newValue)
        {
            modifier += newValue;
        }
        
        /// <summary>
        /// Multiplies the modifier by a value
        /// Upgrades the stat in a multiplicative way
        /// </summary>
        /// <param name="multiplier">What to multiply the modifier by</param>
        public void MultiplyModifier(float multiplier)
        {
            modifier *= multiplier;
        }

        /// <summary>
        /// Decreases the stat's modifier by a certain amount
        /// Downgrades the stat in an additive way
        /// </summary>
        /// <param name="oldValue">How much decrease the modifier by</param>
        public void TakeModifier(float oldValue)
        {
            modifier -= oldValue;
        }
        
        /// <summary>
        /// Divides the modifier by a value
        /// Downgrades the stat in a multiplicative way
        /// </summary>
        /// <param name="multiplier">What to multiply the modifier by</param>
        public void DivideModifier(float multiplier)
        {
            modifier /= multiplier;
        }
        
        /// <summary>
        /// Gets a nice pretty version of our stat in a readable format.
        /// Formatted to 1 d.p.
        /// </summary>
        /// <returns>The stats as a formatted string</returns>
        public override string ToString()
        {
            float value = modifier * stat;
            return value > 50 ? $"{value:#,##0.#}" : $"{value:#0.0##}";
        }
    }
}
