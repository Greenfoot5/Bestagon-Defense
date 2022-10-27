﻿using System;
using Levels.Maps;
using Modules;
using TMPro;
using Turrets;
using UI.Modules;
using UI.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    /// <summary>
    /// Handles the shop and inventory of the player
    /// </summary>
    public class Shop : MonoBehaviour
    {
        private BuildManager _buildManager;
        private LevelData _levelData;
        private Module _selectedModule;
        private GameObject _selectedModuleButton;
        
        [SerializeField]
        [Tooltip("The inventory to place turret buttons")]
        private GameObject turretInventory; 
        [SerializeField]
        [Tooltip("The inventory to place module buttons under")]
        private GameObject moduleInventory;
        [SerializeField]
        [Tooltip("The generic turret button prefab")]
        private GameObject defaultTurretButton;
        [Tooltip("The generic module button prefab")]
        [SerializeField]
        private GameObject defaultModuleButton;
        
        [Tooltip("The UI to display when the player opens the shop")]
        [SerializeField]
        private GameObject selectionUI;
        
        public int selectionCost;
        private bool _hasPlayerMadePurchase = false;

        private TextMeshProUGUI _turretInventoryButton;
        private TextMeshProUGUI _moduleInventoryButton;

        [Tooltip("The percentage of the selection cost to sell turrets for")]
        [SerializeField]
        private double sellPercentage = 0.85;
        
        /// <summary>
        /// Initialises values and set's starting prices
        /// </summary>
        private void Start()
        {
            _buildManager = BuildManager.instance;
            _levelData = _buildManager.GetComponent<GameManager>().levelData;
            selectionCost = _levelData.initialSelectionCost;
            // Update button text
            _turretInventoryButton = turretInventory.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _moduleInventoryButton = moduleInventory.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _turretInventoryButton.text = "<sprite=\"UI-Gold\" name=\"gold\"> " + selectionCost;
            _moduleInventoryButton.text = "<sprite=\"UI-Gold\" name=\"gold\"> " + selectionCost;
        }
        
        /// <summary>
        /// Selects a turret from the inventory
        /// </summary>
        /// <param name="turret">The TurretBlueprint of the selected turret</param>
        /// <param name="button">The button that selected the turret</param>
        private void SelectTurret(TurretBlueprint turret, GameObject button)
        {
            _buildManager.SelectTurretToBuild(turret, button);
        }
        
        /// <summary>
        /// Selects a module from the inventory
        /// </summary>
        /// <param name="module">The selected module</param>
        /// <param name="button">The button that selected the module</param>
        private void SelectModule(Module module, GameObject button)
        {
            if (_selectedModuleButton != null) _selectedModuleButton.transform.GetChild(0).gameObject.SetActive(false);
            _selectedModuleButton = button;
            button.transform.GetChild(0).gameObject.SetActive(true);
            _selectedModule = module;
        }
        
        /// <summary>
        /// Returns the currently selected module
        /// </summary>
        /// <returns>The currently selected Module</returns>
        public Module GetModule()
        {
            return _selectedModule;
        }
        
        /// <summary>
        /// Removes a module from the inventory
        /// </summary>
        public void RemoveModule()
        {
            Destroy(_selectedModuleButton);
            _selectedModule = null;
        }
        
        /// <summary>
        /// Adds a new turret button to the turret inventory
        /// </summary>
        /// <param name="turret">The blueprint of the turret to add</param>
        public void SpawnNewTurret(TurretBlueprint turret)
        {
            _hasPlayerMadePurchase = true;
            // Add and display the new item
            GameObject turretButton = Instantiate(defaultTurretButton, turretInventory.transform);
            turretButton.name = "_" + turretButton.name;
            turretButton.GetComponent<Image>().sprite = turret.shopIcon;
            turretButton.GetComponent<Button>().onClick.AddListener(delegate { SelectTurret(turret, turretButton); });
            
            selectionUI.GetComponentInChildren<AddSelection>().AddTurretType(turret.prefab.GetComponent<Turret>().GetType());
        }
        
        /// <summary>
        /// Adds a new module button to the module inventory
        /// </summary>
        /// <param name="module">The module to add</param>
        public void SpawnNewModule(Module module)
        {
            GameObject moduleButton = Instantiate(defaultModuleButton, moduleInventory.transform);
            moduleButton.name = "_" + moduleButton.name;
            moduleButton.GetComponentInChildren<ModuleIcon>().SetData(module);
            moduleButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { SelectModule(module, moduleButton); });
        }
        
        /// <summary>
        /// Opens the shop and displays the selection
        /// </summary>
        public void OpenSelectionUI()
        {
            selectionUI.SetActive(true);
        }
        
        /// <summary>
        /// Gets if the player has made a purchase yet
        /// </summary>
        /// <returns>If the player has made a purchase</returns>
        public bool HasPlayerMadePurchase()
        {
            return _hasPlayerMadePurchase || (!_levelData.hasInitialSelection);
        }
        
        /// <summary>
        /// Increases the shop cost by the amount in the level data
        /// </summary>
        public void IncrementSelectionCost()
        {
            GameStats.money -= selectionCost;
            selectionCost += _levelData.selectionCostIncrement;
            UpdateCostText();
        }

        /// <summary>
        /// Updates the text displaying the cost of the next shop opening
        /// </summary>
        public void UpdateCostText()
        {
            _turretInventoryButton.text = "<sprite=\"UI-Gold\" name=\"gold\"> " + selectionCost;
            _moduleInventoryButton.text = "<sprite=\"UI-Gold\" name=\"gold\"> " + selectionCost;
        }
        
        /// <summary>
        /// Displays the module inventory and hides the turret inventory
        /// </summary>
        /// <param name="turret">The selected turret</param>
        public void EnableModuleInventory(Turret turret)
        {
            turretInventory.SetActive(false);
            moduleInventory.SetActive(true);

            Transform moduleTransform = moduleInventory.transform;
            
            // Loop through all modules and check if they are applicable
            for(var i = 0; i < moduleTransform.childCount; i++)
            {
                Transform child = moduleTransform.GetChild(i);
                try
                {
                    child.GetComponentInChildren<Button>().interactable =
                        child.GetComponentInChildren<ModuleIcon>().GetModule().ValidModule(turret);
                }
                // One will be the shop button
                catch (NullReferenceException)
                { }
            }

            if (moduleTransform.childCount != 2 ||
                !moduleTransform.GetChild(1).GetComponentInChildren<Button>().interactable) return;
            
            var button = moduleTransform.GetChild(1).GetComponentInChildren<Button>();
            button.onClick.Invoke();
            button.Select();
        }
        
        /// <summary>
        /// Displays the turret inventory and hides the module inventory
        /// </summary>
        public void EnableTurretInventory()
        {
            turretInventory.SetActive(true);
            moduleInventory.SetActive(false);

            if (turretInventory.transform.childCount != 2) return;

            var button = turretInventory.transform.GetChild(1).GetComponent<Button>();
            button.onClick.Invoke();
            button.Select();
        }

        public int GetSellAmount()
        {
            return (int) (sellPercentage * selectionCost);
        }
    }
}
