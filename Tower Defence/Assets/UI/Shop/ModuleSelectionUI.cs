using System;
using Abstract.Data;
using MaterialLibrary;
using MaterialLibrary.Hexagons;
using Modules;
using TMPro;
using UI.Glyphs;
using UI.Inventory;
using UI.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    /// <summary>
    /// Displays a shop card for a Module
    /// </summary>
    public class ModuleSelectionUI : MonoBehaviour
    {
        [Tooltip("The module to display on the card")]
        [SerializeField]
        private ModuleChainHandler handler;
        
        [Tooltip("The hexagons background of the card (the card's background shader)")]
        [SerializeField]
        private Hexagons bg;
        
        [Tooltip("The TMP text display name of the module")]
        [SerializeField]
        private TextMeshProUGUI displayName;
        [Tooltip("The TMP text tagline of the module")]
        [SerializeField]
        private TextMeshProUGUI tagline;
        
        [Tooltip("The ModuleIcon of the module")]
        [SerializeField]
        private ModuleIcon icon;
        
        [Tooltip("The TMP text to contain the effect/description of the module")]
        [SerializeField]
        private TextMeshProUGUI effect;
        
        [Tooltip("The generic glyph prefab to use to display the applicable turrets")]
        [SerializeField]
        private GameObject glyphPrefab;
        [Tooltip("The Transform to set as the parent for the module's turret glyphs")]
        [SerializeField]
        private Transform applicableGlyphs;

        /// <summary>
        /// Creates the UI
        /// </summary>
        /// <param name="initHandler">The ModuleChainHandler the card is for</param>
        /// <param name="shop">The shop script</param>
        public void Init (ModuleChainHandler initHandler, Shop shop)
        {
            handler = initHandler;

            ModuleChain chain = initHandler.GetChain();
            Module module = initHandler.GetModule();

            bg.color = chain.accentColor;

            displayName.text = initHandler.GetDisplayName();
            tagline.text = chain.tagline.GetLocalizedString();
            tagline.color = chain.accentColor;

            icon.SetData(initHandler);
        
            effect.text = chain.description.GetLocalizedString();
            effect.color = chain.accentColor;
            
            // Adds the any glyph
            if (module.GetValidTypes() == null)
            {
                TurretGlyphSo glyphSo = shop.glyphsLookup.GetForType(null);
                Transform glyph = Instantiate(glyphPrefab, applicableGlyphs.transform).transform;
                glyph.name = "_" + glyph.name;
                glyph.Find("Body").GetComponent<HexagonSprite>().color = glyphSo.body;
                glyph.Find("Shade").GetComponent<HexagonSprite>().color = glyphSo.shade;
                glyph.Find("Glyph").GetComponent<Image>().sprite = glyphSo.glyph;
            }
            // Adds the glyph for every turret the module supports
            else
            {
                foreach (Type turretType in module.GetValidTypes())
                {
                    TurretGlyphSo glyphSo = shop.glyphsLookup.GetForType(turretType);
                    Transform glyph = Instantiate(glyphPrefab, applicableGlyphs).transform;
                    glyph.name = "_" + glyph.name;
                    glyph.Find("Body").GetComponent<HexagonSprite>().color = glyphSo.body;
                    glyph.Find("Shade").GetComponent<HexagonSprite>().color = glyphSo.shade;
                    glyph.Find("Glyph").GetComponent<Image>().sprite = glyphSo.glyph;
                }
            }
            
            // When the card is clicked, the game picks the module
            bg.GetComponent<Button>().onClick.AddListener(delegate { MakeSelection(shop); });
        }

        /// <summary>
        /// Called when the player clicks on the card.
        /// </summary>
        /// <param name="shop"></param>
        private void MakeSelection (Shop shop)
        {
            transform.parent.parent.gameObject.SetActive(false);
            Time.timeScale = 1f;
        
            shop.SpawnNewModule(handler);
            TurretInfo.instance.OpenModuleInventory();
        }
    }
}
