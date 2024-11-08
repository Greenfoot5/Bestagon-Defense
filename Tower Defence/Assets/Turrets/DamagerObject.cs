using System.Collections.Generic;
using System.Linq;
using Abstract.Data;
using Enemies;
using UnityEngine;

namespace Turrets
{
    public abstract class Damager : PlacedObject
    {
        [Tooltip("How much damage the turret deals")]
        public UpgradableStat damage;
        
        // System
        [Tooltip("What GameObject tag the damager can hit")]
        public string enemyTag = "Enemy";
        
        // Modules
        [Tooltip("Which modules the turret has applied")]
        [SerializeField]
        public List<ModuleChainHandler> moduleHandlers = new();
        
        // Events
        public delegate void AttackEvent(Damager damager);
        public delegate void ShootEvent(Bullet bullet);
        public delegate void HitEvent(Enemy target, Damager damager, Bullet bullet = null);

        public event AttackEvent OnAttack;
        public event ShootEvent OnShoot;
        public event HitEvent OnHit;

        protected abstract void Attack();

        /// <summary>
        /// Turret types will override this as attack type will be different for each turret
        /// </summary>
        protected void Attack(Damager damager)
        {
            OnAttack?.Invoke(damager);
        }
        
        /// <summary>
        /// Turret types will override this as attack type will be different for each turret
        /// </summary>
        protected void Shoot(Bullet bullet)
        {
            OnShoot?.Invoke(bullet);
        }

        public void HitMany(IEnumerable<Enemy> targets, Damager damager, Bullet bullet = null)
        {
            foreach (Enemy target in targets)
            {
                OnHit?.Invoke(target, damager, bullet);
            }
        }

        public void Hit(Enemy target, Damager damager, Bullet bullet = null)
        {
            OnHit?.Invoke(target, damager, bullet);
        }
        
        /// <summary>
        /// Adds Modules to our turret after checking they're valid.
        /// </summary>
        /// <param name="handler">The ModuleChainHandler to apply to the turret</param>
        /// <returns>true If the Module was applied successfully</returns>
        public virtual bool AddModule(ModuleChainHandler handler)
        {
            if (!handler.ValidModule(this))
            {
                return false;
            }
            
            // Checks if the module is unique
            // Then if there is a module of the same type but different tier,
            // it cannot be upgraded
            if (handler.GetChain().unique && 
                (moduleHandlers.Any(x => x.GetModule().GetType() == handler.GetModule().GetType() &&
                                         !handler.CanUpgrade(x.GetTier()))))
            {
                return false;
            }

            handler = CalculateUpgrades(handler);
            // TODO - Duplicate handler
            moduleHandlers.Add(handler);
            handler.GetModule().AddModule(this);
            
            return true;
        }
        
        /// <summary>
        /// Removes a module from the turret
        /// </summary>
        /// <param name="handler">The handler of the module to remove</param>
        protected virtual void RemoveModule(ModuleChainHandler handler)
        {
            moduleHandlers.Remove(handler);
            handler.GetModule().RemoveModule(this);
        }
        
        /// <summary>
        /// Performs any module upgrades that are possible with the addition of a new handler
        /// </summary>
        /// <param name="handler">The handler to check for upgrades against</param>
        private ModuleChainHandler CalculateUpgrades(ModuleChainHandler handler)
        {
            var i = 0;
            while (i < moduleHandlers.Count)
            {
                bool canUpgrade = handler.Upgrade(moduleHandlers[i]);
                if (canUpgrade)
                {
                    RemoveModule(moduleHandlers[i]);
                    handler = CalculateUpgrades(handler);
                    break;
                }

                i++;
            }

            return handler;
        }
    }
}