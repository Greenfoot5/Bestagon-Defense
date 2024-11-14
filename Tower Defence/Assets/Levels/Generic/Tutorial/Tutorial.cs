using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.Localization;

namespace Levels.Generic.Tutorial
{
    /// <summary>
    /// Allows players ot navigate the tutorial scene
    /// </summary>
    public class Tutorial : MonoBehaviour
    {
        [Tooltip("The tutorial viewport")]
        [SerializeField]
        private GameObject tutorialMenu;
        [Tooltip("The controls viewport")]
        [SerializeField]
        private GameObject controlsMenu;
        [Tooltip("The button to cycle between the different viewports")]
        [SerializeField]
        private TMP_Text toggleButtonText;
        
        [SerializeField]
        [Tooltip("The button string to display to select the tutorial")]
        private LocalizedString tutorialText;
        [SerializeField]
        [Tooltip("The button string to display to select the controls")]
        private LocalizedString controlsText;
        
        public void MainMenu()
        {
            TransitionManager.Instance.LoadScene("MainMenu");
        }

        public void ToggleControls()
        {
            if (controlsMenu.activeSelf)
            {
                tutorialMenu.SetActive(true);
                controlsMenu.SetActive(false);
                toggleButtonText.text = controlsText.GetLocalizedString();
            }
            else
            {
                tutorialMenu.SetActive(false);
                controlsMenu.SetActive(true);
                toggleButtonText.text = tutorialText.GetLocalizedString();
            }
        }
        
        /// <summary>
        /// Sends the player to the dev wiki if it's the dev version
        /// </summary>
        public void Wiki()
        {
            Application.OpenURL(Application.version.Contains("dev")
                ? "https://greenfoot5.notion.site/Nightly-Wiki-90094b3bcf284ae9834a828d4a4bfede"
                : "https://greenfoot5.notion.site/Wiki-ba485298423447b89f491091ec1687a7");
        }
    }
}
