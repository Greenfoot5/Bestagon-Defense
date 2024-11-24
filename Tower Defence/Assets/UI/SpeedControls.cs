using Abstract.Saving;
using Gameplay;
using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

namespace UI
{
    /// <summary>
    /// Handles the pause menu in a level
    /// </summary>
    public class SpeedControls : MonoBehaviour
    {
        [Tooltip("The UI panel to enable when paused")]
        [SerializeField]
        private GameObject ui;

        [Tooltip("Timer to hide when paused")]
        [SerializeField]
        private GameObject timer;
        [Tooltip("Timer to show when paused")]
        [SerializeField]
        private GameObject pausedTimer;
        [Tooltip("Text showing the current wave on pause")]
        [SerializeField]
        private TMP_Text pausedWaveText;
        [Tooltip("Prefix to the wave")]
        [SerializeField]
        private LocalizedString waveText;

        /// <summary>
        /// Allows the class to listen to the pause button press
        /// </summary>
        private void Start()
        {
            GameStats.controls.Game.Pause.performed += ToggleMenu;
        }

        /// <summary>
        /// Disconnects the event from running when the level is closed
        /// </summary>
        private void OnDestroy()
        {
            GameStats.controls.Game.Pause.performed -= ToggleMenu;
        }

        public void SetSpeed(float speed)
        {
            // Pause
            if (speed == 0f)
            {
                // Resume
                if (Time.timeScale == 0f)
                {
                    Time.timeScale = 1f;
                    UpdateTimer();
                    return;
                }
                
                
                Time.timeScale = 0f;
                UpdateTimer();
                return;
            }
            
            // Toggle current speed
            if (Mathf.Approximately(Time.timeScale, speed))
            {
                Time.timeScale = 0f;
                UpdateTimer();
                return;
            }

            Time.timeScale = speed;
            UpdateTimer();
        }

        /// <summary>
        /// Pauses/unpauses the game, and enables/disables the UI by input button press
        /// </summary>
        public void ToggleMenu(InputAction.CallbackContext ctx)
        {
            ui.SetActive(!ui.activeSelf);
            
            Time.timeScale = ui.activeSelf ? 0f : 1f;
            UpdateTimer();
        }
        
        /// <summary>
        /// Pauses/unpauses the game and enables/disables the UI by UI button press
        /// </summary>
        public void ToggleMenu()
        {
            ui.SetActive(!ui.activeSelf);
            
            Time.timeScale = ui.activeSelf ? 0f : 1f;
            UpdateTimer();
        }
    
        /// <summary>
        /// Restarts the current level
        /// </summary>
        public void Retry()
        {
            GameStats.ClearStats();
            SaveManager.ClearSave(SceneManager.GetActiveScene().name);
            TransitionManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void UpdateTimer()
        {
            if (Time.timeScale != 0f)
            {
                timer.SetActive(true);
                pausedTimer.SetActive(false);
            }
            else
            {
                timer.SetActive(false);
                pausedTimer.SetActive(true);
                pausedWaveText.text = waveText.GetLocalizedString() + GameStats.Rounds;
            }
        }
    
        /// <summary>
        /// Returns the player to the main menu
        /// </summary>
        public void Menu()
        {
            // Transition to the main menu
            TransitionManager.Instance.LoadScene("LevelSelect");
        }
    }
}
