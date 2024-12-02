using Gameplay;
using Levels.Maps;
using MaterialLibrary.Hexagons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
    public class SelectionRefunds : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The shop of the scene")]
        private Shop shop;
        [SerializeField]
        [Tooltip("The component that spawns the cards")]
        private GenerateShopSelection selectionCardsParent;

        [Header("Buttons")]
        [SerializeField]
        [Tooltip("Revitalise tagline")]
        private TMP_Text revitaliseText;
        [SerializeField]
        [Tooltip("Refund tagline")]
        private TMP_Text refundText;
        [SerializeField]
        [Tooltip("Reroll tagline")]
        private TMP_Text rerollText;
        [SerializeField]
        [Tooltip("The reroll game object with the hexagons and button.")]
        private GameObject reroll;
        [SerializeField]
        [Tooltip("The color of the reroll hexagons when the player cannot afford it")]
        private Color rerollDisabledColor = new(0.3f, 0.3f, 0.3f, 1f);

        private Color _rerollDefaultColor;
        private int _rerollsLeft;

        private LevelData _levelData;
        
        /// <summary>
        /// Sets up private references
        /// </summary>
        private void Awake()
        {
            _levelData = BuildManager.instance.GetComponent<GameManager>().levelData;
            _rerollDefaultColor = reroll.GetComponent<Hexagons>().color;
        }
        
        /// <summary>
        /// Displays the correct values in text, and checks the reroll
        /// </summary>
        private void OnEnable()
        {
            revitaliseText.text = "+" + _levelData.revitaliseLives + " <sprite=\"UI-Life\" name=\"life\">";
            refundText.text = "+" + (_levelData.refundPercentage * 100) + "% <sprite=\"UI-EnergyBit\" name=\"bit\">";
            CheckReroll();
        }
        
        /// <summary>
        /// Grants a user heart(s) and closes the selection menu
        /// </summary>
        public void Revitalise()
        {
            GameStats.Lives += _levelData.revitaliseLives;
            transform.parent.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// Refunds (part of) the cost of the selection opening and closes it.
        /// </summary>
        public void Refund()
        {
            int refundAmount = shop.GetEnergyCost();
            GameStats.Energy += (int) (refundAmount * _levelData.refundPercentage);
            transform.parent.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// Rerolls the current selection at the cost of lives
        /// </summary>
        public void Reroll()
        {
            // Calculate cost
            if (_rerollsLeft <= 0)
            {
                if (_levelData.rerollCost < 1)
                {
                    GameStats.Lives -= Mathf.CeilToInt(_levelData.rerollCost);
                    _rerollsLeft = (int) (1f / _levelData.rerollCost);
                }
                else
                {
                    GameStats.Lives -= (int) _levelData.rerollCost;
                }
            }
            _rerollsLeft--;
            
            // Regenerate and refresh
            selectionCardsParent.GenerateSelection();
            //shop.GenerateSelection();
            CheckReroll();
        }
        
        /// <summary>
        /// Check if the player can reroll the selection, and at what cost (if any)
        /// </summary>
        private void CheckReroll()
        {
            // Check if the player can afford a reroll
            if (GameStats.Lives > _levelData.rerollCost)
            {
                reroll.GetComponent<Button>().interactable = true;
                reroll.GetComponent<Hexagons>().color = _rerollDefaultColor;
                rerollText.color = _rerollDefaultColor;
            }
            else
            {
                reroll.GetComponent<Button>().interactable = false;
                reroll.GetComponent<Hexagons>().color = rerollDisabledColor;
                rerollText.color = rerollDisabledColor;
            }
            
            // Work out if it's a reroll
            if (_rerollsLeft > 0)
            {
                rerollText.text = "Free for " + _rerollsLeft + " rolls";
            }
            else if (_levelData.rerollCost < 1)
            {
                rerollText.text = "-" + Mathf.CeilToInt(_levelData.rerollCost) + " <sprite=\"UI-Life\" name=\"life\"> " +
                                  "for " + (int) (1 / _levelData.rerollCost) + " rerolls";
            }
        }
    }
}
