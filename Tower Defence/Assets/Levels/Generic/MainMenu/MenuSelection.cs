using UnityEngine;

namespace Levels.Generic.MainMenu
{
    /// <summary>
    /// Displays the correct menu when the main menu loads
    /// </summary>
    public class MenuSelection : MonoBehaviour
    {
        [Tooltip("The canvas for the main menu")]
        [SerializeField]
        private GameObject menuCanvas;
        [Tooltip("The canvas for the login menu")]
        [SerializeField]
        private GameObject loginCanvas;
        [Tooltip("The canvas for the update menu")]
        [SerializeField]
        private GameObject updateCanvas;

        /// <summary>
        /// Check to see which menu we should display
        /// </summary>
        private void Start()
        {
            // if (!RemoteConfig.IsValidVersion())
            // {
            //     menuCanvas.SetActive(false);
            //     loginCanvas.SetActive(false);
            //     updateCanvas.SetActive(true);
            // }
            if (!SetUsername.HasUsername())
            {
                menuCanvas.SetActive(false);
                loginCanvas.SetActive(true);
                updateCanvas.SetActive(false);
            }
            else
            {
                menuCanvas.SetActive(true);
                loginCanvas.SetActive(false);
                updateCanvas.SetActive(false);
            }
        }
        
        /// <summary>
        /// Closes the update menu
        /// </summary>
        public void ContinueWithoutUpdating()
        {
            if (!SetUsername.HasUsername())
            {
                menuCanvas.SetActive(false);
                loginCanvas.SetActive(true);
                updateCanvas.SetActive(false);
            }
            else
            {
                menuCanvas.SetActive(true);
                loginCanvas.SetActive(false);
                updateCanvas.SetActive(false);
            }
        }
        
        /// <summary>
        /// Sends the player off to download the update
        /// </summary>
        public void GetUpdate()
        {
            Application.OpenURL("https://greenfoot5.itch.io/bestagon-defence");
            Application.Quit();
        }
    }
}
