using Abstract;
using Abstract.Saving;
using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Levels.Generic.MainMenu
{
    /// <summary>
    /// Handles all UI actions for the main menu
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        [Header("Wordmark")]
        [Tooltip("The wordmark/logo on the main screen to colour based on version")]
        private Image wordmark;

        [FormerlySerializedAs("nightlyColor")]
        [SerializeField]
        [Tooltip("The colour to make the wordmark if it's a dev build")]
        private Color devColor;
        [SerializeField]
        [Tooltip("The colour to make the wordmark if it's a alpha build")]
        private Color alphaColor;
        [SerializeField]
        [Tooltip("The colour to make the wordmark if it's a beta build")]
        private Color betaColor;
        [SerializeField]
        [Tooltip("The colour to make the wordmark if it's a release build")]
        private Color releaseColor;
        
        [Header("Username")]
        [Tooltip("The text that displays the username of the player")]
        [SerializeField]
        private TMP_Text loggedInAs;

        [Tooltip("The saved username")]
        [HideInInspector]
        public string username;

        /// <summary>
        /// Does the bits and bobs needed when the game starts
        /// </summary>
        private void Awake()
        {
            DisplayUsername();
            ColourWordmark();
            
            Runner.Run(SaveManager.InitialLoad());
        }
        
        /// <summary>
        /// Display the user's current username
        /// </summary>
        public void DisplayUsername()
        {
            username = PlayerPrefs.GetString("Username");
            loggedInAs.text = "Logged in as \n" + username;
        }
        
        /// <summary>
        /// Sends the player to the level select scene
        /// </summary>
        public void Play()
        {
            TransitionManager.Instance.LoadScene("LevelSelect");
        }
        
        /// <summary>
        /// Transition the user to the tutorial scene
        /// </summary>
        public void Tutorial()
        {
            TransitionManager.Instance.LoadScene("Tutorial");
        }

        public void Settings()
        {
            TransitionManager.Instance.LoadScene("Settings");
        }

        /// <summary>
        /// Quits the application
        /// </summary>
        public void Quit()
        {
            Debug.Log("Exiting...");
            Application.Quit();
        }
        
        /// <summary>
        /// Opens a web browser of the url
        /// </summary>
        /// <param name="url">The url to send the player to</param>
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }
        
        /// <summary>
        /// Changes the colour of the wordmark based on the version type
        /// </summary>
        private void ColourWordmark()
        {
            if (Application.version.Contains("dev"))
            {
                wordmark.color = devColor;
            }
            else if (Application.version.Contains("alpha"))
            {
                wordmark.color = alphaColor;
            }
            else if (Application.version.Contains("beta"))
            {
                wordmark.color = betaColor;
            }
            else
            {
                wordmark.color = releaseColor;
            }
        }
    }
}
    