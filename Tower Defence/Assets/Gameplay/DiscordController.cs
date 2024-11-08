using System;
using Abstract;
using Discord;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay
{
    /// <summary>
    /// Discord Rich Presence activity controller.
    /// </summary>
    public class DiscordController : MonoBehaviour
    {
        private const long ApplicationId = 927337431303352441;

        private Discord.Discord _discord;

        private Activity _activity;

        private Scene _currentScene;

        /// <summary>
        /// Prepares the bridge to Discord
        /// </summary>
        private void Start()
        {
            // Get Discord instance if it is running
            // Catch the error if Discord was not detected
            try
            {
                _discord = new Discord.Discord(ApplicationId, (ulong)CreateFlags.NoRequireDiscord);
            }
            catch (ResultException)
            {
                return;
            }
            # if UNITY_EDITOR
            catch (DllNotFoundException)
            {
                return;
            }
            #endif

            _activity = new Activity() 
            {
                Name = "Bestagon Defence",
                Assets = new ActivityAssets()
                {
                    SmallImage = "bestagon_logo_square_large",
                    SmallText = "v" + Application.version
                }
            };

            _currentScene = SceneManager.GetActiveScene();

            // Assign all events for Rich Presence updates
            SceneManager.activeSceneChanged += OnSceneChange;
            GameStats.OnRoundProgress += UpdateActivity;
            GameStats.OnGameOver += UpdateActivity;
        }

        /// <summary>
        /// Event called when a new scene/level is loaded.
        /// It saves the new scene and updates the "Elapsed" timer.<br/>
        /// Follows the UnityAction&lt;Scene, Scene> pattern.
        /// </summary>
        /// <param name="current">Previous scene</param>
        /// <param name="next">New scene</param>
        private void OnSceneChange(Scene current, Scene next)
        {
            _currentScene = next;
            _activity.Timestamps.Start = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
            UpdateActivity();
        }

        /// <summary>
        /// Updates the activity to display new stats, such as wave number, remaining lives.
        /// </summary>
        private void UpdateActivity()
        {
            GetState(_currentScene.name);
            try
            {
                _discord.GetActivityManager().UpdateActivity(_activity, (_) => { });
            }
            catch (ResultException)
            {
                _discord?.Dispose();
                _discord = null;
            }
        }

        /// <summary>
        /// Updates the <see cref="_activity"/> field to contain strings for display in the Rich Presence
        /// based on the scene name and game state.
        /// </summary>
        /// <param name="sceneName">The scene name for processing</param>
        private void GetState(string sceneName)
        {
            _activity.State = "";
            _activity.Assets.LargeImage = "";
            _activity.Assets.LargeText = "";

            // Hard coded scenes
            switch (sceneName)
            {
                case "MainMenu":
                    _activity.Details = "𝗠𝗮𝗶𝗻 Menu";
                    return;

                case "LevelSelect":
                    _activity.Details = "𝗟𝗲𝘃𝗲𝗹 Select";
                    return;
            }

            // Any other scene that isn't a level too
            if (sceneName.Substring(sceneName.Length - 5) != "Level")
            {
                _activity.Details = "Somewhere far away";
                return;
            }

            // A playable level
            string level = sceneName.Substring(0, sceneName.Length - 5);
            string levelName = Utils.AddSpacesToSentence(level);

            // Large image of the level
            _activity.Assets.LargeImage = level.ToLower();
            _activity.Assets.LargeText = levelName;

            // Game state text based on lives count
            _activity.Details = "𝗣𝗹𝗮𝘆𝗶𝗻𝗴 " + levelName;
            _activity.State = GameStats.Lives > 0
                ? $"𝗪𝗮𝘃𝗲 {GameStats.Rounds}｜𝗟𝗶𝘃𝗲𝘀 {GameStats.Lives}"
                : "𝗚𝗮𝗺𝗲 Over";
        }

        /// <summary>
        /// Runs Discord Rich Presence callbacks every frame.
        /// </summary>
        private void Update()
        {
            _discord?.RunCallbacks();
        }

        /// <summary>
        /// Closes down the bridge and clears the Rich Presence Activity of the player.
        /// </summary>
        private void OnDestroy()
        {
            if (_discord == null)
                return;
            _discord.GetActivityManager().ClearActivity((_) => { });
            _discord.RunCallbacks();
            _discord.Dispose();
        }
    }
}
