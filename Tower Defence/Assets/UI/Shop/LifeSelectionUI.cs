using Gameplay;
using MaterialLibrary.Hexagons;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI.Shop
{
    /// <summary>
    /// Displays a shop card for a Module
    /// </summary>
    public class LifeSelectionUI : MonoBehaviour
    {
        [Tooltip("The amount of lives to grant on click")]
        private int _count;
        
        [Tooltip("The hexagons background of the card (the card's background shader)")]
        [SerializeField]
        private Hexagons bg;
        
        [Tooltip("The TMP text to contain the effect/description of the module")]
        [SerializeField]
        private TextMeshProUGUI effect;
        
        [Tooltip("The generic glyph prefab to use to display the applicable turrets")]
        [SerializeField]
        private LocalizedString lifeCount;

        /// <summary>
        /// Creates the UI
        /// </summary>
        /// <param name="count">The amount of lives to grant</param>
        /// <param name="shop">The shop script</param>
        public void Init (int count, Gameplay.Shop shop)
        {
            _count = count;

            effect.text = lifeCount.GetLocalizedString(count);
            
            // When the card is clicked, the game picks the module
            bg.GetComponent<Button>().onClick.AddListener(delegate { MakeSelection(); });
        }

        /// <summary>
        /// Called when the player clicks on the card.
        /// </summary>
        private void MakeSelection ()
        {
            transform.parent.parent.gameObject.SetActive(false);
            Time.timeScale = 1f;
        
            GameStats.Lives += _count;
        }
    }
}
