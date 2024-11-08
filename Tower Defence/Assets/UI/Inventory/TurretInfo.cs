using System;
using Abstract.Data;
using Gameplay;
using Levels._Nodes;
using TMPro;
using Turrets;
using Turrets.Lancer;
using UI.Modules;
using UI.TurretStats;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class TurretInfo : MonoBehaviour
    {
        public static TurretInfo instance;
        private Node _target;

        [Tooltip("The Shop component of the scene")]
        [SerializeField]
        private Gameplay.Shop shop;
        [SerializeField]
        [Tooltip("The title for the inventory")]
        private TMP_Text inventoryTitle;
        
        [Header("Turret Inventory")]
        [SerializeField]
        [Tooltip("The inventory show/hide for the turrets")]
        private GameObject turretInventoryPage;
        [SerializeField]
        [Tooltip("The text to display for the title")]
        private LocalizedString turretInventoryTitle;
        [SerializeField]
        [Tooltip("The button to open the turret inventory")]
        private GameObject turretInventoryButton;
        
        [Header("Module Inventory")]
        [SerializeField]
        [Tooltip("The inventory to show/hide for the modules")]
        private GameObject moduleInventoryPage;
        [SerializeField]
        [Tooltip("The text to display for the title")]
        private LocalizedString moduleInventoryTitle;
        [SerializeField]
        [Tooltip("The button to open the turret inventory")]
        private GameObject turretInfoButton;
        [SerializeField]
        [Tooltip("The button list")]
        private Transform moduleInventoryContent;
        [SerializeField]
        [Tooltip("The colour to set the bg when disabled")]
        private Color moduleDisabledColor;
        
        [Header("Turret Info")]
        [SerializeField]
        [Tooltip("The page show/hide for the turret info")]
        private GameObject turretInfoPage;
        [SerializeField]
        [Tooltip("The button to add more modules")]
        private GameObject addModuleButton;
        
        [Space(10)]
        [Tooltip("The TurretStat used to display the damage")]
        [SerializeField]
        private TurretStat damage;
        [Tooltip("The TurretStat used to display the fire rate")]
        [SerializeField]
        private TurretStat rate;
        [Tooltip("The TurretStat used to display the range")]
        [SerializeField]
        private TurretStat range;
        
        [Space(10)]
        [Tooltip("The GameObject to spawn the module icons as a child of")]
        [SerializeField]
        private Transform modules;
        [Tooltip("The prefab of a module icon to instantiate to display the turret's modules")]
        [SerializeField]
        private GameObject moduleIconPrefab;
        
        [Header("Buttons")]
        [Tooltip("The button that changes the targeting method of the turret")]
        [SerializeField]
        private GameObject cycleTargetingButton;
        [SerializeField]
        [Tooltip("The text to display on the rotation button")]
        private LocalizedString rotateText;
        
        
        /// <summary>
        /// Check there is only one NodeUI when loading in
        /// </summary>
        private void Awake()
        {
            // Make sure there is only ever have one NodeUI
            if (instance != null)
            {
                Debug.LogWarning("More than one TurretInfo in scene!");
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        
        /// <summary>
        /// Called when selecting a new node
        /// </summary>
        /// <param name="node">The new node to display UI for</param>
        public void SetTarget(Node node)
        {
            _target = node;
            
            // Display the radius of the turret
            _target.turret.GetComponent<Turret>().Selected();

            // Enable/Disable Targeting types cycle button if it's (not) a dynamic turret.
            if (_target.turret.GetComponent<Turret>() is DynamicTurret)
            {
                cycleTargetingButton.SetActive(true);
                cycleTargetingButton.transform.GetComponentInChildren<TMP_Text>().text = "<b>Targeting:</b>\n" +
                    _target.turret.GetComponent<DynamicTurret>().targetingMethod;
                cycleTargetingButton.GetComponent<Button>().onClick.RemoveAllListeners();
                cycleTargetingButton.GetComponent<Button>().onClick.AddListener(CycleTargeting);
            }
            else if (_target.turret.GetComponent<Turret>() is Lancer)
            {
                cycleTargetingButton.SetActive(true);
                cycleTargetingButton.transform.GetComponentInChildren<TMP_Text>().text = rotateText.GetLocalizedString();
                cycleTargetingButton.GetComponent<Button>().onClick.RemoveAllListeners();
                cycleTargetingButton.GetComponent<Button>().onClick.AddListener(RotateLancer);
            }
            else
            {
                cycleTargetingButton.SetActive(false);
            }

            // // Rebuild the Modules and add the stats
            // if (moduleInventoryPage.activeSelf)
            //     OpenModuleInventory();
            // else
            OpenTurretInfo();
        }
        
        /// <summary>
        /// Gets the turret the NodeUI is targeting
        /// </summary>
        /// <returns>The turret the NodeUI is targeting</returns>
        public GameObject GetTurret()
        {
            return _target != null ? _target.turret : null;
        }
        
        /// <summary>
        /// Applies a module to the currently selected turret
        /// </summary>
        public void ApplyModule(ModuleChainHandler handler, GameObject button)
        {
            bool isApplied = _target.ApplyModuleToTurret(handler);
            if (!isApplied) return;
            
            GameManager.ModuleInventory.Remove(handler);
            shop.RemoveModule(button);
            UpdateModules();
            OpenTurretInfo();
        }
        
        /// <summary>
        /// Turret's targeting method increments once through the cycle of targeting methods
        /// </summary>
        private void CycleTargeting()
        {
            Array types = Enum.GetValues(typeof(DynamicTurret.TargetingMethod));
            var currentMethod = (int)_target.turret.GetComponent<DynamicTurret>().targetingMethod;
            _target.turret.GetComponent<DynamicTurret>().targetingMethod = (DynamicTurret.TargetingMethod)( (currentMethod + 1) % types.Length);
            
            // Update our button text
            cycleTargetingButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "<b>Targeting:</b>\n" +
                _target.turret.GetComponent<DynamicTurret>().targetingMethod;
        }
        
        /// <summary>
        /// Updates the stats display when the turret is selected or upgraded
        /// </summary>
        public void UpdateStats()
        {
            if (_target.turret == null) return;
            var turret = _target.turret.GetComponent<Turret>();
            // Stats
            damage.SetData(turret.damage);
            rate.SetData(turret.fireRate);
            range.SetData(turret.range);
            // Display the radius of the turret
            turret.Selected();
            Color color = turret.rangeDisplay.GetComponent<SpriteRenderer>().color;
            damage.SetColor(color);
            rate.SetColor(color);
            range.SetColor(color);
        }

        public void UpdateSelection()
        {
            UpdateStats();
            UpdateModules();
        }
        
        /// <summary>
        /// Sells the turret
        /// </summary>
        public void SellTurret()
        {
            _target.SellTurret(shop.GetSellAmount());
        }
        
        /// <summary>
        /// Rotates Lancer Turret
        /// </summary>
        private void RotateLancer()
        {
            _target.turret.GetComponent<Lancer>().partToRotate.Rotate(0, 0, -60);
        }
        
        /// <summary>
        /// Updates the render of the modules for a turret
        /// </summary>
        private void UpdateModules()
        {
            // Removes module icons created from the previously selected turret
            for (var i = 0; i < modules.childCount; i++)
                Destroy(modules.GetChild(i).gameObject);
            
            // Add each Module as an icon
            foreach (ModuleChainHandler handle in _target.turret.GetComponent<Turret>().moduleHandlers)
            {
                GameObject moduleIcon = Instantiate(moduleIconPrefab, modules);
                moduleIcon.name = "_" + moduleIcon.name;
                moduleIcon.GetComponent<ModuleIcon>().SetData(handle);
                foreach (Image image in moduleIcon.GetComponentsInChildren<Image>())
                {
                    image.raycastTarget = false;
                }
            }

            GameObject addModule = Instantiate(addModuleButton, modules);
            addModule.GetComponent<Button>().onClick.AddListener(OpenModuleInventory);
            
            // Display the radius of the turret
            _target.turret.GetComponent<Turret>().UpdateRange();
        }
        
        public void DisplayTurretInventory()
        {
            BuildManager.instance.Deselect();
            Show();
            inventoryTitle.text = turretInventoryTitle.GetLocalizedString();
            turretInventoryPage.SetActive(true);
            moduleInventoryPage.SetActive(false);
            turretInfoPage.SetActive(false);
        }
        
        public void ToggleTurretInventory()
        {
            if (turretInventoryPage.activeSelf)
            {
                BuildManager.instance.Deselect();
                return;
            }
            DisplayTurretInventory();
        }

        public void ToggleModuleInventory()
        {
            if (moduleInventoryPage.activeSelf)
            {
                BuildManager.instance.Deselect();
                return;
            }
            OpenModuleInventory();
        }

        public void OpenModuleInventory()
        {
            foreach (Transform child in moduleInventoryContent)
            {
                var item = child.GetComponent<ModuleInventoryItem>();
                if (_target != null && item.IsValid(_target.turret.GetComponent<Damager>()))
                {
                    item.bg.color = item.accent;
                    item.modulesBg.color = item.accent * new Color(1, 1, 1, 0.16f);
                    item.GetComponent<Button>().interactable = true;
                }
                else
                {
                    item.bg.color = moduleDisabledColor;
                    item.modulesBg.color = moduleDisabledColor * new Color(1, 1, 1, 0.16f);
                    item.GetComponent<Button>().interactable = false;
                }
            }
            
            Show();
            inventoryTitle.text = moduleInventoryTitle.GetLocalizedString();
            moduleInventoryPage.SetActive(true);
            turretInventoryPage.SetActive(false);
            turretInfoPage.SetActive(false);
        }

        public void OpenTurretInfo()
        {
            if (turretInfoPage.activeSelf)
            {
                BuildManager.instance.Deselect();
                return;
            }
            
            Show();
            inventoryTitle.text = _target.turretBlueprint.displayName.GetLocalizedString();
            turretInfoPage.SetActive(true);
            turretInventoryPage.SetActive(false);
            moduleInventoryPage.SetActive(false);
            turretInfoButton.SetActive(true);
            turretInventoryButton.SetActive(false);

            turretInfoButton.GetComponent<Image>().color = _target.turretBlueprint.accent;
            
            UpdateStats();
            UpdateModules();
        }

        public void Close()
        {
            turretInfoPage.SetActive(false);
            turretInventoryPage.SetActive(false);
            moduleInventoryPage.SetActive(false);
            turretInventoryButton.SetActive(true);
            turretInfoButton.SetActive(false);
            
            _target = null;
            
            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(-0.25f, rt.anchorMin.y);
            rt.anchorMax = new Vector2(0f, rt.anchorMax.y);
        }

        private void Show()
        {
            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
            rt.anchorMax = new Vector2(0.25f, rt.anchorMax.y);
        }
    }
}
