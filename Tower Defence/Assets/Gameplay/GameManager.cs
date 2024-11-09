using System.Collections.Generic;
using Abstract.Data;
using Abstract.Saving;
using Levels._Nodes;
using Levels.Maps;
using MaterialLibrary.Trapezium;
using TMPro;
using Turrets;
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
            saveData.Lives = GameStats.Lives;
            saveData.Energy = GameStats.Energy;
            saveData.Powercells = GameStats.Powercells;
            saveData.WaveIndex = GameStats.Rounds - 1;
            saveData.RandomState = Random.state;
            saveData.ShopCost = shop.GetComponent<Shop>().nextCost;
            saveData.Nodes = new List<SaveLevel.NodeData>();
            saveData.TurretInventory = new List<TurretBlueprint>();
            saveData.ModuleInventory = new List<ModuleChainHandler>();

            // Node Data
            foreach (Node node in nodeParent.GetComponentsInChildren<Node>())
            {
                if (node.turret == null)
                {
                    continue;
                }
                
                var nodeData = new SaveLevel.NodeData
                {
                    uuid = node.name,
                    turretBlueprint = node.turretBlueprint,
                    turretRotation = node.turret.transform.rotation,
                    moduleChainHandlers = node.turret.GetComponent<Turret>().moduleHandlers
                };
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
            GameStats.PopulateRounds(saveData.WaveIndex + 1);
            Random.state = saveData.RandomState;
            var shopComponent = shop.GetComponent<Shop>();
            shopComponent.nextCost = saveData.ShopCost;
            GameStats.Powercells = saveData.Powercells;
            GameStats.Energy = saveData.Energy;

            foreach (SaveLevel.NodeData nodeData in saveData.Nodes)
            {
                foreach (Node node in nodeParent.GetComponentsInChildren<Node>())
                {
                    if (node.name != nodeData.uuid) continue;
                    
                    node.LoadTurret(nodeData.turretBlueprint);
                    node.turret.transform.rotation = nodeData.turretRotation;
                    foreach (ModuleChainHandler moduleHandler in nodeData.moduleChainHandlers)
                    {
                        node.LoadModule(moduleHandler);
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
