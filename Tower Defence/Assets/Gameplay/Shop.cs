using Abstract;
using Abstract.Data;
using Levels.Maps;
using MaterialLibrary.Trapezium;
using TMPro;
using Turrets;
using UI.Inventory;
using UI.Shop;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
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
        private ModuleChainHandler _selectedHandler;
        
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
        [FormerlySerializedAs("selectionCost")] [Range(0, Mathf.Infinity)]
        [HideInInspector]
        public int nextCost;
        private bool _hasPlayerMadePurchase;
        
        [FormerlySerializedAs("shopButton")] [FormerlySerializedAs("turretInventoryButton")] [SerializeField]
        private TextMeshProUGUI powercellsText;
        [SerializeField]
        private Progress energyProgress;
        
        [Header("Shop Button")]
        [Tooltip("Shop Button Colours Top when can afford")]
        private Color _defaultButtonTop;
        [Tooltip("Shop Button Colours Bottom when can afford")]
        private Color _defaultButtonBottom;
        [Tooltip("Shop Button Colours Top when can't afford")]
        [SerializeField]
        private Color expensiveButtonTop;
        [Tooltip("Shop Button Colours Bottom when can afford")]
        [SerializeField]
        private Color expensiveButtonBottom;
        [Tooltip("The text to display")]
        [SerializeField]
        private LocalizedString shopText;
        
        [Tooltip("The GlyphsLookup index in the scene")]
        [SerializeField]
        public TypeSpriteLookup glyphsLookup;

        /// <summary>
        /// Initialises values and set's starting prices
        /// </summary>
        private void Start()
        {
            _buildManager = BuildManager.instance;
            _levelData = _buildManager.GetComponent<GameManager>().levelData;
            
            // It should only be greater than 0 if we've loaded a save
            if (nextCost == 0)
                nextCost = _levelData.initialSelectionCost;
            else
                _hasPlayerMadePurchase = true;

            _defaultButtonTop = energyProgress.outColorA;
            _defaultButtonBottom = energyProgress.outColorB;
            
            // Update button text
            powercellsText.text = "<sprite=\"UI-Powercell\" name=\"full\"> " + nextCost;
            GameStats.OnGainEnergy += CalculateCells;
            GameStats.OnGainPowercell += UpdateEnergyButton;
            CalculateCells();
        }
        
        /// <summary>
        /// Removes a module from the inventory
        /// </summary>
        public void RemoveModule(GameObject button)
        {
            Destroy(button);
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
            turretButton.GetComponent<TurretInventoryItem>().Init(turret);
            
            selectionUI.GetComponentInChildren<AddSelection>().AddTurretType(turret.prefab.GetComponent<Turret>().GetType());
            GameManager.TurretInventory.Add(turret);
        }
        
        /// <summary>
        /// Adds a new module button to the module inventory
        /// </summary>
        /// <param name="module">The module to add</param>
        public void SpawnNewModule(ModuleChainHandler module)
        {
            GameObject moduleButton = Instantiate(defaultModuleButton, moduleInventory.transform);
            moduleButton.name = "_" + moduleButton.name;
            moduleButton.GetComponentInChildren<ModuleInventoryItem>().Init(module, glyphsLookup);
            moduleButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { TurretInfo.instance.ApplyModule(module, moduleButton); });
            
            GameManager.ModuleInventory.Add(module);
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
        
        public int GetSellPercentage()
        {
            return (int) (_levelData.sellPercentage * 100);
        }

        public int GetSellAmount()
        {
            return (int) (_levelData.sellPercentage * nextCost);
        }

        private void CalculateCells()
        {
            var energyToSubtract = 0;
            while (GameStats.Energy - energyToSubtract > nextCost)
            {
                nextCost += _levelData.selectionCostIncrement;
                energyToSubtract += nextCost;
                GameStats.Powercells++;
            }
            if (energyToSubtract > 0)
                GameStats.Energy -= energyToSubtract;
            UpdateEnergyButton();
        }

        private void UpdateEnergyButton()
        {
            energyProgress.outColorA = expensiveButtonTop;
            energyProgress.outColorB = expensiveButtonBottom;
            powercellsText.color = expensiveButtonTop;
            powercellsText.text = shopText.GetLocalizedString() + " - <sprite=\"UI-Powercell\" name=\"empty\"> " + 0;
            if (GameStats.Powercells > 0)
            {
                energyProgress.outColorA = _defaultButtonTop;
                energyProgress.outColorB = _defaultButtonBottom;
                powercellsText.color = new Color(1,1,1);
                powercellsText.text = shopText.GetLocalizedString() + " - <sprite=\"UI-Powercell\" name=\"full\"> " + GameStats.Powercells;
            }

            energyProgress.percentage = GameStats.Energy / (float)nextCost;
        }
    }
}
