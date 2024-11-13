using Abstract.Data;
using Gameplay;
using MaterialLibrary;
using MaterialLibrary.GlowBox;
using TMPro;
using Turrets;
using UI.Modules;
using UI.TurretStats;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class TurretInventoryItem : MonoBehaviour
    {
        private TurretBlueprint _turretBlueprint;
        
        [Tooltip("The TMP text to display the turret's display name")]
        [SerializeField]
        private TextMeshProUGUI displayName;
        
        [Tooltip("The Image to place the turret's icon")]
        [SerializeField]
        private Image icon;
        [Tooltip("The Image to place the turret's glyph")]
        [SerializeField]
        private Image glyph;
        [Tooltip("The body colour of the turret's glyph")]
        [SerializeField]
        private HexagonSprite glyphBody;
        
        [Header("Modules")]
        [Tooltip("The none text of modules to disable if the turret has any modules")]
        [SerializeField]
        private GameObject noneText;
        [Tooltip("The parent of any module icons to display")]
        [SerializeField]
        private GameObject modulesLayout;
        [Tooltip("The prefab of a generic module icon to instantiate under the modulesLayout")]
        [SerializeField]
        private GameObject moduleUI;
        
        [Header("Stats")]
        [Tooltip("The TurretStat used to display the damage")]
        [SerializeField]
        private TurretStat damage;
        [Tooltip("The TurretStat used to display the fire rate")]
        [SerializeField]
        private TurretStat rate;
        [Tooltip("The TurretStat used to display the range")]
        [SerializeField]
        private TurretStat range;
        
        [Header("Colors")]
        [Tooltip("The Hexagons shader background of the card")]
        [SerializeField]
        private GlowBox bg;
        [Tooltip("The background Image of the modules section")]
        [SerializeField]
        private Image modulesBg;

        /// <summary>
        /// Creates and setups the Selection UI.
        /// </summary>
        /// <param name="turret">The turret the option selects</param>
        public void Init(TurretBlueprint turret)
        {
            _turretBlueprint = turret;
            
            // Turret text
            displayName.text = turret.displayName.GetLocalizedString();
            
            // Icon and Glyph
            icon.sprite = turret.shopIcon;
            glyph.sprite = turret.glyph.glyph;
            glyphBody.color = turret.glyph.body;
            
            // Turret stats
            var turretPrefab = turret.prefab.GetComponent<Turret>();
            damage.SetData(turretPrefab.damage);
            rate.SetData(turretPrefab.fireRate);
            range.SetData(turretPrefab.range);
            
            // Colors
            bg.color = turret.accent;
            modulesBg.color = turret.accent * new Color(1, 1, 1, .16f);

            damage.SetColor(turret.accent);
            rate.SetColor(turret.accent);
            range.SetColor(turret.accent);
            
            // Turret's Modules
            if (turret.moduleHandlers.Count != 0)
            {
                noneText.SetActive(false);
                foreach (ModuleChainHandler handler in turret.moduleHandlers)
                {
                    GameObject mod = Instantiate(moduleUI, modulesLayout.transform);
                    mod.name = "_" + mod.name;
                    mod.GetComponentInChildren<ModuleIcon>().SetData(handler);
                }
            }
        }

        /// <summary>
        /// Called when a player clicks the card,
        /// selecting it and closing the shop
        /// </summary>
        public void Select()
        {
            BuildManager.instance.SelectTurretToBuild(_turretBlueprint, gameObject);
        }
    }
}
