using System.Collections.Generic;
using Abstract.Data;
using Abstract.Saving;
using Levels._Nodes;
using Levels.Maps;
using MaterialLibrary.Trapezium;
using TMPro;
using Turrets;
using UI.Shop;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Gameplay
{
    /// <summary>
    /// Manages the current game's state
    /// </summary>
    public class GameManager : MonoBehaviour, ISaveableLevel
    {
        // If the game has actually finished yet
        public static bool isGameOver;
        
        [Tooltip("The UI to display when the player loses")]
        public GameObject gameOverUI;

        [Tooltip("The UI that displays the shop")]
        [SerializeField]
        private GameObject shop;

        [Tooltip("The Text for the lives amount")]
        [SerializeField]
        private TMP_Text livesText;
        [Tooltip("The Progress Graphic for the lives bar")]
        [SerializeField]
        private Progress livesBar;
        private int _startLives;
        
        [Tooltip("The levelData to use for the current level")]
        public LevelData levelData;

        [Tooltip("The parent of all the nodes")]
        public GameObject nodeParent;

        public static readonly List<TurretBlueprint> TurretInventory = new();
        public static readonly List<ModuleChainHandler> ModuleInventory = new();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            TurretInventory.Clear();
            ModuleInventory.Clear();
        }

        private void Awake()
        {
            _startLives = GameStats.Lives;
            if (PlayerPrefs.GetInt("LoadingLevel", 0) == 0) return;
            LoadJsonData(this);
        }

        /// <summary>
        /// Makes sure the level has some data to run with
        /// Makes sure that the game isn't over.
        /// </summary>
        private void Start()
        {
            isGameOver = false;
            if (levelData == null)
            {
                Debug.LogError("No level data set!", this);
            }
            
            GameStats.OnLoseLife += UpdateLives;
            UpdateLives();
        }
    
        /// <summary>
        /// Checks if the game is over yet
        /// </summary>
        private void Update()
        {
            if (isGameOver)
            {
                return;
            }
        
            if (GameStats.Lives <= 0)
            {
                EndGame();
            }
        }

        private void OnDestroy()
        {
            GameStats.OnLoseLife -= UpdateLives;
        }
    
        /// <summary>
        /// Ends the game.
        /// Displays the game over screen and saves the player's score.
        /// </summary>
        private void EndGame()
        {
            isGameOver = true;
        
            gameOverUI.SetActive(true);
            shop.SetActive(false);

            Time.timeScale = 0;
            
            SaveManager.ClearSave(SceneManager.GetActiveScene().name);
        }

        public void PopulateSaveData(SaveLevel saveData)
        {
            var droppedEnergy = 0;
            foreach (DeathEnergy particle in DeathBitManager.Particles)
            {
                if (!particle.IsAnimating)
                    droppedEnergy += particle.Value;
            }
            
            saveData.Energy = GameStats.Energy + droppedEnergy;
            saveData.Powercells = GameStats.Powercells;
            saveData.Lives = GameStats.Lives;
            saveData.WaveIndex = GameStats.Rounds - 1;
            saveData.TotalCellsCollected = shop.GetComponent<Shop>().totalCellsCollected;
            saveData.Nodes = new List<SaveLevel.NodeData>();
            saveData.TurretInventory = new List<TurretBlueprint>();
            saveData.ModuleInventory = new List<ModuleChainHandler>();
            
            // Random
            saveData.RandomState = Random.state;
            saveData.RandomSeed = Shop.oldState.Item1;
            saveData.ShopRandomN = Shop.oldState.Item2;

            // Node Data
            foreach (Node node in nodeParent.GetComponentsInChildren<Node>())
            {
                if (node.turret == null)
                {
                    continue;
                }

                var turret = node.turret.GetComponent<Turret>();
                List<string> names = new();
                List<int> tiers = new();
                foreach (ModuleChainHandler handler in turret.moduleHandlers)
                {
                    names.Add(handler.GetChain().name);
                    tiers.Add(handler.GetTier());
                }
                
                var nodeData = new SaveLevel.NodeData
                {
                    uuid = node.name,
                    blueprintName = node.turretBlueprint.name,
                    moduleNames = names,
                    moduleTiers = tiers
                };

                if (turret.GetType().IsSubclassOf(typeof(DynamicTurret)))
                {
                    nodeData.turretRotation = ((DynamicTurret)turret).partToRotate.rotation;
                    nodeData.targetingMethod = ((DynamicTurret)turret).targetingMethod;
                }

                saveData.Nodes.Add(nodeData);
            }
            
            // Inventory
            // Turrets
            foreach (TurretBlueprint turret in TurretInventory)
            {
                saveData.TurretInventory.Add(turret);
            }
            // Modules
            foreach (ModuleChainHandler module in ModuleInventory)
            {
                saveData.ModuleInventory.Add(module);
            }
        }
        
        /// <summary>
        /// Loads the data from json
        /// </summary>
        private static void LoadJsonData(ISaveableLevel level)
        {
            SaveManager.LoadLevel(level, SceneManager.GetActiveScene().name);
        }

        public void LoadFromSaveData(SaveLevel saveData)
        {
            _startLives = GameStats.Lives;
            GameStats.Lives = saveData.Lives;
            GameStats.PopulateRounds(saveData.WaveIndex);
            var shopComponent = shop.GetComponent<Shop>();
            shopComponent.totalCellsCollected = saveData.TotalCellsCollected;
            GameStats.Powercells = saveData.Powercells;
            GameStats.Energy = saveData.Energy;
            
            // Random
            Random.state = saveData.RandomState;
            Shop.random = new Squirrel3(saveData.RandomSeed, saveData.ShopRandomN);

            foreach (SaveLevel.NodeData nodeData in saveData.Nodes)
            {
                foreach (Node node in nodeParent.GetComponentsInChildren<Node>())
                {
                    if (node.name != nodeData.uuid) continue;
                    
                    node.LoadTurret(SaveLevel.Blueprints[nodeData.blueprintName]);
                    for (var i = 0; i < nodeData.moduleNames.Count; i++)
                    {
                        node.LoadModule(new ModuleChainHandler(SaveLevel.Chains[nodeData.moduleNames[i]], nodeData.moduleTiers[i]));
                    }
                    
                    var turret = node.turret.GetComponent<Turret>();
                    shopComponent.selectionGenerator.AddTurretType(turret.GetType());
                    if (turret.GetType().IsSubclassOf(typeof(DynamicTurret)))
                    {
                        ((DynamicTurret)turret).partToRotate.rotation = nodeData.turretRotation;
                        ((DynamicTurret)turret).targetingMethod = nodeData.targetingMethod;
                    }
                }
            }

            foreach (TurretBlueprint turret in saveData.TurretInventory)
            {
                shopComponent.SpawnNewTurret(turret);
            }
            foreach (ModuleChainHandler module in saveData.ModuleInventory)
            {
                shopComponent.SpawnNewModule(module);
            }
        }

        private void UpdateLives()
        {
            livesBar.percentage = GameStats.Lives / (float)_startLives;
            livesText.text = $"{GameStats.Lives}";
        }
    }
}
