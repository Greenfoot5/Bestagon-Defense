﻿using System.Collections.Generic;
using Abstract.Data;
using UnityEngine;
using Upgrades.Modules;

namespace Turrets
{
    /// <summary>
    /// The various targeting methods a turret can use to find a target
    /// </summary>
    public enum TargetingMethod
    {
        Closest = 0,
        Weakest,
        Strongest,
        First,
        Last
    }
    
    /// <summary>
    /// The base Turret class that can be extended to add other turret types
    /// </summary>
    public abstract class Turret : MonoBehaviour
    {
        public UpgradableStat damage;

        // System
        public string enemyTag = "Enemy";
        
        public UpgradableStat range = new UpgradableStat(2.5f);
        public GameObject rangeDisplay;

        // Attack speed
        [Tooltip("Time between each shot")]
        public UpgradableStat fireRate = new UpgradableStat(1f);
        protected float fireCountdown;

        // Modules
        public List<Module> modules = new List<Module>();
        
        /// <summary>
        /// Disables the range dislaying
        /// </summary>
        private void Awake()
        {
            rangeDisplay.SetActive(false);
        }
        
        /// <summary>
        /// Turret types will override this as attack type will be different for each turret
        /// </summary>
        protected abstract void Attack();

        /// <summary>
        /// Allows the editor to display the range of the turret
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range.GetStat());
        }
        
        /// <summary>
        /// Adds Modules to our turret after checking they're valid.
        /// </summary>
        /// <param name="module">The Module to apply to the turret</param>
        /// <returns>true If the Module was applied successfully</returns>
        public bool AddModule(Module module)
        {
            if (!module.ValidModule(this))
            {
                //Debug.Log("Invalid Module");
                return false;
            }
            
            // TODO - Add as difficulty modifier
            // Module oldModule = null;
            // // Check if the turret already have an Module of the same type
            // foreach (var turretModule in Modules.Where(turretModule => turretModule.GETModuleType() == Module.GETModuleType()))
            // {
            //     // If it's of a higher level, remove the current level
            //     if (turretModule.ModuleTier < Module.ModuleTier)
            //     {
            //         Debug.Log("Removing lower level Module");
            //         oldModule = turretModule;
            //     }
            //     // If it's of a lower level, the module can't be added
            //     else
            //     {
            //         Debug.Log("This turret already has an Module of the same type at" +
            //                   " the same level or better!");
            //         return false;
            //     }
            // }
            //
            // if (oldModule != null)
            // {
            //     oldModule.RemoveModule(this);
            //     Modules.Remove(oldModule);
            // }

            module.AddModule(this);
            modules.Add(module);
            
            // Update the range shader's size
            var localScale = transform.localScale;
            rangeDisplay.transform.localScale = new Vector3(
                range.GetStat() / localScale.x * 2,
                range.GetStat() / localScale.y * 2,
                1);
            return true;
        }
        
        /// <summary>
        /// Called when the turret is selected, displays the turret's range
        /// </summary>
        public void Selected()
        {
            // Update the range shader's size
            var localScale = transform.localScale;
            rangeDisplay.transform.localScale = new Vector3(
                range.GetStat() / localScale.x * 2,
                range.GetStat() / localScale.y * 2,
                1);
            rangeDisplay.SetActive(true);
        }
        
        /// <summary>
        /// Called when the turret is deselected, disables the turret's range view.
        /// </summary>
        public void Deselected()
        {
            rangeDisplay.SetActive(false);
        }
    }
}
