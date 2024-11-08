using System;
using Abstract;
using Abstract.Data;
using MaterialLibrary;
using MaterialLibrary.GlowBox;
using TMPro;
using Turrets;
using UI.Glyphs;
using UI.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class ModuleInventoryItem : MonoBehaviour
    {
        private ModuleChainHandler _module;
        
        [Tooltip("The TMP text to display the module's display name")]
        [SerializeField]
        private TMP_Text displayName;
        [Tooltip("The text to set for the module's effect")]
        [SerializeField]
        private TMP_Text effectText;
        
        [Tooltip("The ModuleIcon for the module card")]
        [SerializeField]
        private ModuleIcon icon;
        
        [Header("Colors")]
        [Tooltip("The Hexagons shader background of the card")]
        [SerializeField]
        public GlowBox bg;
        [Tooltip("The background Image of the modules section")]
        [SerializeField]
        public Image modulesBg;
        
        [Tooltip("The generic glyph prefab to use to display the applicable turrets")]
        [SerializeField]
        private GameObject glyphPrefab;
        [Tooltip("The Transform to set as the parent for the module's turret glyphs")]
        [SerializeField]
        private Transform applicableGlyphs;
        
        [Tooltip("The list of types the turret has")]
        public Type[] turretTypes;
        //[HideInInspector]
        [Tooltip("The original accent colour")]
        public Color accent;

        /// <summary>
        /// Creates and setups the Selection UI.
        /// </summary>
        /// <param name="module">The module the option selects</param>
        /// <param name="lookup">The TypeSpriteLookup</param>
        public void Init (ModuleChainHandler module, TypeSpriteLookup lookup)
        {
            _module = module;
            
            // Module text
            displayName.text = module.GetDisplayName();
            effectText.text = module.GetChain().description.GetLocalizedString();
            
            // Icon
            icon.SetData(module);

            // Colors
            bg.color = module.GetChain().accentColor;
            accent = module.GetChain().accentColor;
            modulesBg.color = module.GetChain().accentColor * new Color(1, 1, 1, .16f);
            
            // Adds the any glyph
            if (_module.GetModule().GetValidTypes() == null)
            {
                TurretGlyphSo glyphSo = lookup.GetForType(null);
                Transform glyph = Instantiate(glyphPrefab, applicableGlyphs.transform).transform;
                glyph.name = "_" + glyph.name;
                glyph.Find("Body").GetComponent<HexagonSprite>().color = glyphSo.body;
                glyph.Find("Shade").GetComponent<HexagonSprite>().color = glyphSo.shade;
                glyph.Find("Glyph").GetComponent<Image>().sprite = glyphSo.glyph;
            }
            // Adds the glyph for every turret the module supports
            else
            {
                foreach (Type turretType in module.GetModule().GetValidTypes())
                {
                    TurretGlyphSo glyphSo = lookup.GetForType(turretType);
                    Transform glyph = Instantiate(glyphPrefab, applicableGlyphs).transform;
                    glyph.name = "_" + glyph.name;
                    glyph.Find("Body").GetComponent<HexagonSprite>().color = glyphSo.body;
                    glyph.Find("Shade").GetComponent<HexagonSprite>().color = glyphSo.shade;
                    glyph.Find("Glyph").GetComponent<Image>().sprite = glyphSo.glyph;
                }
            }

            turretTypes = module.GetModule().GetValidTypes();
        }

        public bool IsValid(Damager damager)
        {
            return _module.ValidModule(damager);
        }
    }
}
