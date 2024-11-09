using Abstract.Data;
using Gameplay;
using Turrets;
using UI;
using UI.Inventory;
using UI.Modules;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Levels._Nodes
{
    /// <summary>
    /// Manages all data and actions for a single node on a level map
    /// </summary>
    public class Node : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerUpHandler
    {
        [Tooltip("The colour to set the node when it's being hovered over and the player is trying to build something")]
        public Color hoverColour;
        private Color _defaultColour;
        
        [Tooltip("A turret that starts on the node")]
        [SerializeField]
        private TurretBlueprint initialTurret;

        [Tooltip("The GameObject to spawn the module icons as a child of")]
        [SerializeField]
        private Transform modulesDisplay;
        [Tooltip("The prefab of a module icon to instantiate to display the turret's modules")]
        [SerializeField]
        private GameObject moduleIconPrefab;
        
        // Turret info
        [HideInInspector]
        public GameObject turret;
        [HideInInspector]
        public TurretBlueprint turretBlueprint;
    
        private Renderer _rend;
        private BuildManager _buildManager;

        // Pointer handling
        private bool _isHolding;

        private void Awake()
        {
            if (initialTurret != null)
                LoadTurret(initialTurret);
        }

        private void Start()
        {
            _rend = GetComponent<Renderer>();
            _defaultColour = _rend.material.color;
            _buildManager = BuildManager.instance;
        }
        
        /// <summary>
        /// We load a turret into the node without any fancy build effects or adding modules (those are added separately).
        /// </summary>
        /// <param name="blueprint">The blueprint of the turret to build</param>
        public void LoadTurret(TurretBlueprint blueprint)
        {
            GameObject newTurret = Instantiate(blueprint.prefab, transform.position, Quaternion.identity);
            newTurret.name = "_" + newTurret.name;
            turret = newTurret;
            turretBlueprint = blueprint;
        }
        
        /// <summary>
        /// Loads a module without any fancy effects
        /// </summary>
        /// <param name="handler">The module handler to load</param>
        public bool LoadModule(ModuleChainHandler handler)
        {
            // Check handler has a module and tier
            if (handler.GetModule() == null)
            {
                return false;
            }
            
            // Apply the Module
            bool hasAppliedModule = turret.GetComponent<Turret>().AddModule(handler);
            return hasAppliedModule;
        }

        /// <summary>
        /// Places the turret on the node
        /// </summary>
        /// <param name="blueprint">The blueprint of the turret to build</param>
        private void BuildTurret(TurretBlueprint blueprint)
        {
            // Spawn the turret and set the turret and blueprint
            Vector3 nodePosition = transform.position;
            GameObject newTurret = Instantiate(blueprint.prefab, nodePosition, Quaternion.identity);
            newTurret.name = "_" + newTurret.name;
            turret = newTurret;
            var turretClass = turret.GetComponent<Turret>();
            turretBlueprint = blueprint;
            turretClass.displayName = blueprint.displayName;
        
            foreach (ModuleChainHandler handler in blueprint.moduleHandlers)
            {
                turretClass.AddModule(handler);
            }
        
            // Spawn the build effect and destroy after
            GameObject effect = Instantiate(_buildManager.buildEffect, nodePosition, Quaternion.identity);
            effect.name = "_" + effect.name;
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        }
    
        /// <summary>
        /// Called when upgrading a turret
        /// </summary>
        /// <param name="handler">The Module to add to the turret</param>
        /// <returns>If the Module was applied</returns>
        public bool ApplyModuleToTurret(ModuleChainHandler handler)
        {
            // Check handler has a module and tier
            if (handler.GetModule() == null)
            {
                return false;
            }
            
            // Apply the Module
            bool hasAppliedModule = turret.GetComponent<Turret>().AddModule(handler);
            if (!hasAppliedModule) return false;

            // Spawn the build effect
            GameObject effect = Instantiate(_buildManager.buildEffect, transform.position, Quaternion.identity);
            effect.name = "_" + effect.name;
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        
            // Update the TurretInfo
            TurretInfo.instance.UpdateSelection();
            return true;
        }
    
        /// <summary>
        /// Called when the turret is sold
        /// </summary>
        public void SellTurret(int sellAmount)
        {
            // Grant the money
            GameStats.Energy += sellAmount;

            // Spawn the sell effect
            GameObject effect = Instantiate(_buildManager.sellEffect, transform.position, Quaternion.identity);
            effect.name = "_" + effect.name;
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        
            // Destroy the turret and reset any of the node's selection variables
            Destroy(turret);
            turretBlueprint = null;

            BuildManager.instance.Deselect();
        }
    
        /// <summary>
        /// Called when the mouse is down.
        /// Depending on platform, processes the input for later interaction of the node
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            // Ignore camera pan as a click
            if (eventData.button == PointerEventData.InputButton.Middle)
                return;

            // If on Android, queue interaction for release of touch
            // Makes it possible to cancel the input if the touch was meant to drag the camera
            if (Application.platform == RuntimePlatform.Android)
            {
                _isHolding = true;
                return;
            }

            // If on Desktop and click wasn't a camera pan - handle input as normal
            HandlePointerInteract();
        }

        /// <summary>
        /// Called when the pointer was dragged.
        /// Acts as a cancel for interaction on Android
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            _isHolding = false;
        }

        /// <summary>
        /// Called when the pointer was released.
        /// If the pointer wasn't dragged, then on Android this calls the interaction handler
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isHolding)
                HandlePointerInteract();
        }

        /// <summary>
        /// Handles interaction.
        /// Either Selects the turret or builds
        /// </summary>
        private void HandlePointerInteract()
        {
            // Select the node/turret
            if (turret != null)
            {
                _buildManager.SelectNode(this);
                return;
            }
            // If the player is clicking an empty node

            // Player doesn't have a build button selected
            if (!_buildManager.HasTurretToBuild)
            {
                _buildManager.Deselect();
                return;
            }

            // Construct a turret
            BuildTurret(_buildManager.GetTurretToBuild());
            _buildManager.BuiltTurret();
            _buildManager.SelectNode(this);
        }
        
        /// <summary>
        /// Called when the mouse hovers over the node
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (turret != null)
            {
                UpdateModules();
                modulesDisplay.gameObject.SetActive(true);
            }
            
            // Make sure the player is trying to build
            if (!_buildManager.HasTurretToBuild)
            {
                return;
            }
            
            _rend.material.color = hoverColour;
            BuildManager.instance.currentPreview.transform.position = transform.position;
            BuildManager.instance.currentPreview.SetActive(true);
        }
    
        /// <summary>
        /// Called when the mouse is no longer over the node
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (turret != null)
            {
                modulesDisplay.gameObject.SetActive(false);
            }
            
            _rend.material.color = _defaultColour;
            if (BuildManager.instance.currentPreview != null)
                BuildManager.instance.currentPreview.SetActive(false);
        }
        
        /// <summary>
        /// Updates the render of the modules for a turret
        /// </summary>
        private void UpdateModules()
        {
            // Removes module icons created from the previously selected turret
            for (var i = 0; i < modulesDisplay.childCount; i++)
                Destroy(modulesDisplay.GetChild(i).gameObject);
            
            // Add each Module as an icon
            foreach (ModuleChainHandler handle in turret.GetComponent<Turret>().moduleHandlers)
            {
                GameObject moduleIcon = Instantiate(moduleIconPrefab, modulesDisplay);
                moduleIcon.name = "_" + moduleIcon.name;
                moduleIcon.GetComponent<ModuleIcon>().SetData(handle);
                foreach (Image image in moduleIcon.GetComponentsInChildren<Image>())
                {
                    image.raycastTarget = false;
                }
            }

            modulesDisplay.GetComponent<TriangleLayout>().SetLayoutHorizontal();
            modulesDisplay.GetComponent<TriangleLayout>().SetLayoutVertical();
        }
    }
}
