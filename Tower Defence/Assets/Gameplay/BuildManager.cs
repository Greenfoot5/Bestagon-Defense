using Levels._Nodes;
using Turrets;
using UI.Inventory;
using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Handles all tasks related to building turrets and selecting nodes
    /// </summary>
    public class BuildManager : MonoBehaviour
    {
        [Tooltip("The instance of the BuildManager")]
        public static BuildManager instance;
        
        [Tooltip("The effect spawned when a turret is built.")]
        public GameObject buildEffect;
        [Tooltip("The effect spawned when a turret is sold")]
        public GameObject sellEffect;

        private TurretBlueprint _turretToBuild;
        private GameObject _buildingButton;
        private Node _selectedNode;
        
        /// <summary>
        /// If the player is currently building or not
        /// </summary>
        public bool HasTurretToBuild => _turretToBuild != null;
        
        /// <summary>
        /// Check there is only one build manager when loading in
        /// </summary>
        private void Awake()
        {
            // Make sure there is only ever have one BuildManager
            if (instance != null)
            {
                Debug.LogError("More than one build manager in scene!");
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Sets the turret the player want's to build
        /// </summary>
        /// <param name="turret">The blueprint of the turret to build</param>
        /// <param name="buttonToDelete">The inventory button to remove</param>
        public void SelectTurretToBuild(TurretBlueprint turret, GameObject buttonToDelete)
        {
            _turretToBuild = turret;
            _buildingButton = buttonToDelete;
        }
        
        /// <summary>
        /// Let the build manager know the turret has been constructed
        /// </summary>
        public void BuiltTurret()
        {
            Destroy(_buildingButton);
            GameManager.TurretInventory.Remove(_turretToBuild);
            _turretToBuild = null;
        }
    
        /// <summary>
        /// Gets the blueprint of the turret the player currently want to build
        /// </summary>
        /// <returns>The turret blueprint of the turret the player wants to build</returns>
        public TurretBlueprint GetTurretToBuild()
        {
            return _turretToBuild;
        }
    
        /// <summary>
        /// Sets the selected node
        /// </summary>
        /// <param name="node">The selected node</param>
        public void SelectNode(Node node)
        {
            if (_selectedNode == node)
            {
                Deselect();
                TurretInfo.instance.Close();
                return;
            }

            if (_selectedNode != null)
            {
                // Clear any previous selection
                Deselect();
            }

            _selectedNode = node;
            TurretInfo.instance.SetTarget(node);
        }

        public void Deselect()
        {
            _turretToBuild = null;

            if (_selectedNode != null && _selectedNode.turret != null)
            {
                _selectedNode.turret.GetComponent<Turret>().Deselected();
            }
            _selectedNode = null;
            TurretInfo.instance.Close();
        }
    }
}
