using System;
using System.Collections.Generic;
using System.Linq;
using Abstract.Data;
using Gameplay;
using Levels.Maps;
using Turrets;
using UnityEngine;

namespace UI.Shop
{
    public class GenerateShopSelection : MonoBehaviour
    {
        [Tooltip("The game object for a turret selection card")]
        [SerializeField]
        private GameObject turretSelectionUI;
        [Tooltip("The game object for a module selection card")]
        [SerializeField]
        private GameObject moduleSelectionUI;
        [Tooltip("The game object for a life selection card")]
        [SerializeField]
        private GameObject lifeSelectionUI;
        private LevelData _levelData;
        private Shop _shop;
        [Tooltip("The parent to add the selection to")]
        [SerializeField]
        private Transform selectionCardsParent;
        
        [Tooltip("The turrets already purchased")]
        private readonly List<Type> _turretTypes = new();

        [Tooltip("The button to show when unlocked")]
        [SerializeField]
        private GameObject lockButton;
        [Tooltip("The status to show when locked")]
        [SerializeField]
        private GameObject lockedButton;
        private bool _isLocked;

        private void Awake()
        {
            _shop = GetComponent<Shop>();
        }
    
        /// <summary>
        /// Setups references, checks the player has enough gold and freezes the game when enabled
        /// </summary>
        private void Start()
        {
            _levelData = BuildManager.instance.GetComponent<GameManager>().levelData;
        }
        
        /// <summary>
        /// Generates the selection of the shop
        /// </summary>
        public void GenerateSelection()
        {
            if (_isLocked) return;

            Shop.OldState = Shop.random.GetState();
            
            // Destroy the previous selection
            for (int i = selectionCardsParent.childCount - 1; i >= 0; i--)
            {
                Destroy(selectionCardsParent.GetChild(i).gameObject);
            }
            
            int selectionCount = _shop.HasPlayerMadePurchase() ? _levelData.selectionChoices : _levelData.initialChoices;
            // Tracks what the game has given the player, so the game don't give duplicates
            var selectedTurrets = new List<TurretBlueprint>();
            var selectedModules = new List<ModuleChainHandler>();
            var hasLife = false;
            
            for (var i = 0; i < selectionCount; i++)
            {
                // If it's the first time opening the shop this level, the game should display a different selection
                if (!_shop.HasPlayerMadePurchase())
                {
                    // Grants an Module option
                    selectedTurrets.Add(GenerateInitialItem(i, selectedTurrets));
                }
                else
                {
                    // Select if the game should get a module, turret or life
                    // Can only have one life option
                    float choice = Shop.random.Range(0f, _levelData.turretOptionWeight.Value.Evaluate(GameStats.Rounds)
                                                    + _levelData.moduleOptionWeight.Value.Evaluate(GameStats.Rounds)
                                                    + (!hasLife ? 1 : 0) * _levelData.lifeOptionWeight.Value.Evaluate(GameStats.Rounds));
                    if (choice <= _levelData.moduleOptionWeight.Value.Evaluate(GameStats.Rounds))
                    {
                        // Grants an Module option
                        selectedModules.Add(GenerateModuleItem(i, selectedModules));

                    }
                    else if (_levelData.moduleOptionWeight.Value.Evaluate(GameStats.Rounds) < choice && choice <=
                             _levelData.moduleOptionWeight.Value.Evaluate(GameStats.Rounds) + _levelData.turretOptionWeight.Value.Evaluate(GameStats.Rounds))
                    {
                        selectedTurrets.Add(GenerateTurretItem(i, selectedTurrets));
                    }
                    else
                    {
                        GenerateLifeItem();
                        hasLife = true;
                    }
                }
            }
        }

        private TurretBlueprint GenerateInitialItem(int selectionIndex, ICollection<TurretBlueprint> selectedTurrets)
        {
            // Grants a turret option
            var turrets = new WeightedList<TurretBlueprint>(_levelData.initialTurretSelection);
            TurretBlueprint selected = turrets.GetRandomItem(duplicateType: _levelData.initialDuplicateCheck,
                previousPicks: selectedTurrets.Take(selectionIndex).ToArray(), rng: Shop.random);
            
            // Add the turret to the ui for the player to pick
            GenerateTurretUI(selected);
            
            return selected;
        }

        private TurretBlueprint GenerateTurretItem(int selectionIndex, ICollection<TurretBlueprint> selectedTurrets)
        {
            // Grants a turret option
            WeightedList<TurretBlueprint> turrets = _levelData.turrets.ToWeightedList(GameStats.Rounds);
            TurretBlueprint selected = turrets.GetRandomItem(duplicateType: _levelData.turretDuplicateCheck,
                previousPicks: selectedTurrets.Take(selectionIndex).ToArray(), rng: Shop.random);
            
            // Add the turret to the ui for the player to pick
            GenerateTurretUI(selected);

            return selected;
        }
        
        private ModuleChainHandler GenerateModuleItem(int selectionIndex, ICollection<ModuleChainHandler> selectedModules)
        { 
            WeightedList<ModuleChainHandler> modules = _levelData.moduleHandlers.ToWeightedList(GameStats.Rounds);

            // Only show modules that can be equipped on a turret the player has (or had)
            for (var i = 0; i < modules.Count; i++)
            {
                Type[] validTypes = modules[i].item.GetModule().GetValidTypes();
                if (validTypes == null || validTypes.Any(x => _turretTypes.Contains(x))) continue;
                
                modules.RemoveAt(i);
                i--;
            }
            
            ModuleChainHandler selected = modules.GetRandomItem(duplicateType: _levelData.moduleDuplicateCheck,
                previousPicks: selectedModules.Take(selectionIndex).ToArray(), rng: Shop.random);

            // Adds the Module as an option to the player
            GenerateModuleUI(selected);

            return selected;
        }

        private void GenerateLifeItem()
        {
            // Create the ui as a child
            GameObject lifeUI = Instantiate(lifeSelectionUI, selectionCardsParent);
            lifeUI.name = "_" + lifeUI.name;
            lifeUI.GetComponent<LifeSelectionUI>().Init(_levelData.lifeCount, _shop);
        }
    
        /// <summary>
        /// Adds a new Module UI option to the player's choice
        /// </summary>
        /// <param name="handler">The Module the player can pick</param>
        private void GenerateModuleUI(ModuleChainHandler handler)
        {
            // Create the ui as a child
            GameObject moduleUI = Instantiate(moduleSelectionUI, selectionCardsParent);
            moduleUI.name = "_" + moduleUI.name;
            moduleUI.GetComponent<ModuleSelectionUI>().Init(handler, _shop);
        }
    
        /// <summary>
        /// Adds a new turret UI option to the player's choice
        /// </summary>
        /// <param name="turret">The turret the player can pick</param>
        private void GenerateTurretUI(TurretBlueprint turret)
        {
            turret.glyph = _shop.glyphsLookup.GetForType(turret.prefab.GetComponent<Turret>().GetType());
            GameObject turretUI = Instantiate(turretSelectionUI, selectionCardsParent);
            turretUI.name = "_" + turretUI.name;
            turretUI.GetComponent<TurretSelectionUI>().Init(turret, _shop);
        }
        
        /// <summary>
        /// Adds a turret type to the selected type.
        /// Makes sure we have a full list of turret the player has purchased
        /// so we can display only ones they can use
        /// </summary>
        /// <param name="type">The type of the turret to add</param>
        public void AddTurretType(Type type)
        {
            if (!_turretTypes.Contains(type))
                _turretTypes.Add(type);
        }
        
        public void Open()
        {
            Time.timeScale = 0f;
            selectionCardsParent.parent.gameObject.SetActive(true);
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            selectionCardsParent.parent.gameObject.SetActive(false);
        }

        public void Lock()
        {
            // TODO - Reveal Hidden
            _isLocked = true;
            lockButton.SetActive(false);
            lockedButton.SetActive(true);
        }

        public void Unlock()
        {
            _isLocked = false;
            lockButton.SetActive(true);
            lockedButton.SetActive(false);
        }
    }
}
