using System;
using System.Collections;
using Gameplay;
using Levels.Generic.LevelSelect;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Abstract.Saving
{
    /// <summary>
    /// Handles saving and loading of persistent data,
    /// including which file it's kept in
    /// </summary>
    public static class SaveManager
    {
        private static LeaderboardServerBridge _bridge;
        
        /// <summary>
        /// Used to load any persistent data that needs to be loaded at the start of the game,
        /// </summary>
        public static IEnumerator InitialLoad()
        {
            // A "delay" to make sure files are loaded correctly
            yield return LocalizationSettings.InitializationOperation;
            
            // Load Locale
            if (FileManager.LoadFromFile("Settings.dat", out string json))
            {
                var sd = new SaveSettings();
                sd.LoadFromJson(json);
                LocalizationSettings.SelectedLocale = sd.locale;
            }
            else
            {
                FileManager.WriteToFile("Settings.dat", new SaveSettings().ToJson());
            }

            _bridge = new LeaderboardServerBridge();
        }
        
        /// <summary>
        /// Saves the settings
        /// </summary>
        /// <param name="saveable">The settings to save to the file</param>
        public static void SaveSettings(ISaveableSettings saveable)
        {
            var sd = new SaveSettings();
            saveable.PopulateSaveData(sd);
            
            if (FileManager.WriteToFile("Settings.dat", sd.ToJson()))
            {
                Debug.Log("Saving Settings successful");
            }
        }
        
        /// <summary>
        /// Loads the settings
        /// </summary>
        /// <param name="saveable">The settings to load</param>
        public static void LoadSettings(ISaveableSettings saveable)
        {
            if (!FileManager.LoadFromFile("Settings.dat", out string json)) return;
        
            var sd = new SaveSettings();
            sd.LoadFromJson(json);
            
            saveable.LoadFromSaveData(sd);

            Debug.Log("Loading settings complete");
        }

        /// <summary>
        /// Saves a level's current progress, and to the leaderboard
        /// </summary>
        /// <param name="saveable">The level data to save to file</param>
        /// <param name="sceneName">The name of the scene</param>
        public static void SaveLevel(ISaveableLevel saveable, string sceneName)
        {
            try
            {
                // Tell our leaderboard API to add the player
                string leaderboardData =
                    Environment.GetEnvironmentVariable(SceneManager.GetActiveScene().name + "Leaderboard");
                if (leaderboardData != null && _bridge != null)
                {
                    string[] splitData = leaderboardData.Split(';');
                    _bridge.SendPlayerValue(PlayerPrefs.GetString("Username"), GameStats.Rounds, splitData[0],
                        splitData[1]);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to save: " + e);
            }

            var sd = new SaveLevel();
            saveable.PopulateSaveData(sd);
            
            if (FileManager.WriteToFile(sceneName + "Save.dat", sd.ToJson()))
            {
                //Debug.Log("Saving " + sceneName + " successful");
            }
        }

        /// <summary>
        /// Loads a level save
        /// </summary>
        /// <param name="saveable">The level save to load</param>
        /// <param name="sceneName">The name of the level to load</param>
        public static void LoadLevel(ISaveableLevel saveable, string sceneName)
        {
            if (!FileManager.LoadFromFile(sceneName + "Save.dat", out string json)) return;
        
            var sd = new SaveLevel();
            sd.LoadFromJson(json);
            
            saveable.LoadFromSaveData(sd);
        }

        public static bool SaveExists(string sceneName)
        {
            if (!FileManager.FileExists(sceneName + "Save.dat")) return false;
            
            FileManager.LoadFromFile(sceneName + "Save.dat", out string json);
            var sd = new SaveLevel();
            sd.LoadFromJson(json);

            return sd.Version == Application.version;
        }

        public static void ClearSave(string sceneName)
        {
            FileManager.DeleteFile(sceneName + "Save.dat");
            GameStats.Rounds = 0;

            Debug.Log("Deleting save of " + sceneName + " complete");
        }
    }
}