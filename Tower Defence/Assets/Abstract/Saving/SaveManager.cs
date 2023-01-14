using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Abstract.Saving
{
    public static class SaveManager
    {
        /// <summary>
        /// Used to load any data that needs to be loaded at the start of the game,
        /// but can't otherwise for whatever reason
        /// </summary>
        public static void InitialLoad()
        {
            // Load Locale
            if (FileManager.LoadFromFile("Settings.dat", out string json))
            {
                var sd = new SaveSettings();
                sd.LoadFromJson(json);
                LocalizationSettings.SelectedLocale = sd.locale;

                AsyncOperationHandle<LocalizationSettings> localInit = LocalizationSettings.InitializationOperation;
                localInit.Completed += _ => LocalizationSettings.SelectedLocale = sd.locale;
            }
        }
        
        public static void SaveSettings(ISaveableSettings saveable)
        {
            var sd = new SaveSettings();
            saveable.PopulateSaveData(sd);
            
            if (FileManager.WriteToFile("Settings.dat", sd.ToJson()))
            {
                Debug.Log("Saving Settings successful");
            }
        }
    
        public static void LoadSettings(ISaveableSettings saveable)
        {
            if (!FileManager.LoadFromFile("Settings.dat", out string json)) return;
        
            var sd = new SaveSettings();
            sd.LoadFromJson(json);
            
            saveable.LoadFromSaveData(sd);

            Debug.Log("Loading settings complete");
        }
    }
}