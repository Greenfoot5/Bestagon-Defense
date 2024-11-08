using Abstract.Data;
using UnityEngine;

namespace Turrets
{
    public abstract class Turret : Damager
    {
        [Tooltip("The range of the turret")]
        public UpgradableStat range = new(2.5f);
        [Tooltip("The shader that display's the turret's range when clicked")]
        [SerializeField]
        public GameObject rangeDisplay;
        
        // Attack speed
        [Tooltip("How many times per second the turret attacks")]
        public UpgradableStat fireRate = new(1f);
        /// <summary> How long left until the next attack </summary>
        public float fireCountdown;
        
        /// <summary>
        /// Stops the range displaying
        /// </summary>
        private void Awake()
        {
            rangeDisplay.SetActive(false);
        }
        
        /// <summary>
        /// Update the range shader's size
        /// </summary>
        public virtual void UpdateRange()
        {
            // Update the range shader's size
            Vector3 localScale = transform.localScale;
            rangeDisplay.transform.localScale = new Vector3(
                range.GetStat() / localScale.x * 2,
                range.GetStat() / localScale.y * 2,
                1);
        }
        
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
        /// <param name="handler">The ModuleChainHandler to apply to the turret</param>
        /// <returns>true If the Module was applied successfully</returns>
        public override bool AddModule(ModuleChainHandler handler)
        {
            bool value = base.AddModule(handler);

            UpdateRange();
            return value;
        }
        
        /// <summary>
        /// Removes a module from the turret
        /// </summary>
        /// <param name="handler">The handler of the module to remove</param>
        protected override void RemoveModule(ModuleChainHandler handler)
        {
            base.RemoveModule(handler);
            
            UpdateRange();
        }
        
        /// <summary>
        /// Called when the turret is selected, displays the turret's range
        /// </summary>
        public override void Selected()
        {
            UpdateRange();
            rangeDisplay.SetActive(true);
        }
        
        /// <summary>
        /// Called when the turret is deselected, disables the turret's range view.
        /// </summary>
        public override void Deselected()
        {
            rangeDisplay.SetActive(false);
        }
    }
}