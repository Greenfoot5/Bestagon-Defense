using Abstract;
using Abstract.Data;
using Gameplay;
using Levels.Maps;
using MaterialLibrary.Trapezium;
using TMPro;
using Turrets;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Shop
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
        [FormerlySerializedAs("selectionCost")]
        [Range(0, Mathf.Infinity)]
        [HideInInspector]
        public int nextCost;
        [HideInInspector]
        public int totalCellsCollected;
        
        [Tooltip("Current count of powercells")]
        [SerializeField]
        private TMP_Text powercellCount;
        [Tooltip("Current count of powercells")]
        [SerializeField]
        private Progress powercellProgress;
        
        [Header("Shop Button")]
        [Tooltip("Shop Button Colours Top when can afford")]
        [SerializeField]
        private Image buyButton;
        private Button _buyButtonButton;
        [Tooltip("Shop button image when can afford")]
        [SerializeField]
        private Sprite affordButtonImage;
        [Tooltip("Shop button image when can't afford")]
        [SerializeField]
        private Sprite expensiveButtonImage;
        [Tooltip("Shop Button Colours Bottom when can afford")]
        [SerializeField]
        private GameObject expensiveButtonOverlay;
        
        [Tooltip("The GlyphsLookup index in the scene")]
        [SerializeField]
        public TypeSpriteLookup glyphsLookup;

        public static Squirrel3 random;

        /// <summary>
        /// Initialises values and set's starting prices
        /// </summary>
        private void Start()
        {
            _buildManager = BuildManager.instance;
            _levelData = _buildManager.GetComponent<GameManager>().levelData;
            _buyButtonButton = buyButton.gameObject.GetComponent<Button>();
            
            // It should only be greater than 0 if we've loaded a save
            nextCost = GetEnergyCost();
            
            GameStats.OnGainEnergy += CalculateCells;
            GameStats.OnGainPowercell += UpdateBuyButton;
            CalculateCells();
            UpdateBuyButton();
        }

        private void OnDestroy()
        {
            GameStats.OnGainEnergy -= CalculateCells;
            GameStats.OnGainPowercell -= UpdateBuyButton;
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
            // Add and display the new item
            GameObject turretButton = Instantiate(defaultTurretButton, turretInventory.transform);
            turretButton.name = "_" + turretButton.name;
            turretButton.GetComponent<TurretInventoryItem>().Init(turret);
            
            selectionUI.GetComponentInChildren<GenerateShopSelection>().AddTurretType(turret.prefab.GetComponent<Turret>().GetType());
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
            return totalCellsCollected - GameStats.Powercells > _levelData.initialSelectionCount;
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
            while (GameStats.Energy - energyToSubtract > nextCost && nextCost != 0)
            {
                totalCellsCollected += 1;
                nextCost = GetEnergyCost();
                energyToSubtract += nextCost;
                GameStats.Powercells++;
            }
            if (energyToSubtract > 0)
                GameStats.Energy -= energyToSubtract;
            UpdateBuyButton();
        }

        private void UpdateBuyButton()
        {
            if (GameStats.Powercells > 0)
            {
                buyButton.sprite = affordButtonImage;
                expensiveButtonOverlay.SetActive(false);
                _buyButtonButton.enabled = true;
            }
            else
            {
                buyButton.sprite = expensiveButtonImage;
                expensiveButtonOverlay.SetActive(true);
                _buyButtonButton.enabled = false;
            }
            UpdateEnergyCount();
        }
        
        private void UpdateEnergyCount()
        {
            powercellCount.text = GameStats.Powercells.ToString();
            powercellProgress.percentage = GameStats.Energy / (float)nextCost;
        }

        /// <summary>
        /// To make sure the expression evaluator is doing the right thing each time, there's a function.
        /// </summary>
        public int GetEnergyCost()
        {
            ExpressionEvaluator.Evaluate(_levelData.selectionCostFormula.Replace("x", $"({totalCellsCollected.ToString()})"), out int output);
            if (output == 0)
                Debug.LogError("Energy Cost was 0, likely an issue with formula");
            return output;
        }
    }
}
