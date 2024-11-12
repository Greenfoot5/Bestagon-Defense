using System.Collections;
using System.Collections.Generic;
using Abstract.Saving;
using Gameplay;
using TMPro;
using UI.Transition;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Levels.Generic.Settings
{
    /// <summary>
    /// Handles the saving/loading/changing of settings
    /// </summary>
    public class Settings : MonoBehaviour, ISaveableSettings
    {
        [SerializeField]
        [Tooltip("The dropdown to select language/locale ")]
        private TMP_Dropdown dropdown;
        [SerializeField]
        [Tooltip("The dropdown to enable/disable energy pickups")]
        private Toggle energyBitToggle;
        
        /// <summary>
        /// Loads all the UI & settings the player can change
        /// </summary>
        /// <returns></returns>
        private IEnumerator Start()
        {
            // Wait for the localization system to initialize
            yield return LocalizationSettings.InitializationOperation;
            
            // Load the current settings
            LoadJsonData(this);

            // Generate list of available Locales
            var options = new List<TMP_Dropdown.OptionData>();
            var selected = 0;
            for (var i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    selected = i;
                options.Add(new TMP_Dropdown.OptionData(locale.name));
            }
            dropdown.options = options;

            dropdown.value = selected;
            dropdown.onValueChanged.AddListener(LocaleSelected);

            energyBitToggle.isOn = DeathBitManager.dropsEnergy;
            energyBitToggle.onValueChanged.AddListener(EnergyDropsToggled);
        }
        
        /// <summary>
        /// Updates the locale
        /// </summary>
        /// <param name="index"></param>
        private void LocaleSelected(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            SaveJsonData(this);
        }

        private void EnergyDropsToggled(bool status)
        {
            DeathBitManager.dropsEnergy = status;
            SaveJsonData(this);
        }

        /// <summary>
        /// Sends the player to the main menu scene
        /// </summary>
        public void MainMenu()
        {
            TransitionManager.Instance.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// Saves the settings to json data
        /// </summary>
        private static void SaveJsonData(ISaveableSettings settings)
        {
            var saveData = new SaveSettings();
            settings.PopulateSaveData(saveData);
            
            SaveManager.SaveSettings(settings);
        }
        
        /// <summary>
        /// Writes the settings to a SaveSettings data
        /// </summary>
        /// <param name="saveData">The SaveSettings to load</param>
        public void PopulateSaveData(SaveSettings saveData)
        {
            saveData.Locale = LocalizationSettings.SelectedLocale;
            saveData.HasEnergyPickup = DeathBitManager.dropsEnergy;
        }
        
        /// <summary>
        /// Loads the data from json
        /// </summary>
        private static void LoadJsonData(ISaveableSettings settings)
        {
            SaveManager.LoadSettings(settings);
        }

        /// <summary>
        /// Loads data from a SaveSettings into the Settings
        /// </summary>
        /// <param name="saveData">The SaveSettings to load</param>
        public void LoadFromSaveData(SaveSettings saveData)
        {
            LocalizationSettings.SelectedLocale = saveData.Locale;
            DeathBitManager.dropsEnergy = saveData.HasEnergyPickup;
        }

        public void PrivacyPolicy()
        {
            Application.OpenURL("https://bestagon.alchemix.dev/legal/privacy.html");
        }
    }
}