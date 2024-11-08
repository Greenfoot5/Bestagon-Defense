using System.Collections;
using Abstract.Saving;
using Enemies;
using Levels.Maps;
using MaterialLibrary.Trapezium;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

namespace Gameplay.Waves
{
    /// <summary>
    /// Handles the current wave and spawning of enemies
    /// </summary>
    public class WaveSpawner : MonoBehaviour
    {
        [Tooltip("How many enemies are still alive in the level")]
        public static int enemiesAlive;
        
        [SerializeField]
        [Tooltip("The waves the level will loop through")]
        private Wave[] waves;
        
        [SerializeField]
        [Tooltip("How long to wait in between waves")]
        private float timeBetweenWaves = 5f;
        [SerializeField]
        [Tooltip("The time at the beginning before the start of the game")]
        private float preparationTime = 8f;
        private float _countdown = 5f;
        [SerializeField]
        [Tooltip("The index of the wave to start from after the boss wave")]
        private int waveRepeatIndex;
        
        [SerializeField]
        [Tooltip("The text to update with the countdown/spawning/enemies")]
        private TMP_Text waveCountdownText;
        [SerializeField]
        [Tooltip("The Progress Graphic to display the wave progress")]
        private Progress waveProgress;
        [SerializeField]
        [Tooltip("The text to display the current wave")]
        private TMP_Text waveText;
    
        // The current wave the player is on -1 (as it's indexed from 0)
        private int _waveIndex;
        
        [SerializeField]
        [Tooltip("The location to spawn the enemies. Should be the first waypoint")]
        private Transform spawnPoint;

        private bool _isSpawning;
        private float _totalEnemies;

        private LevelData _levelData;

        [Header("Localization")]
        [SerializeField]
        [Tooltip("The text to show with the wave count")]
        private LocalizedString waveCountText;
        [SerializeField]
        [Tooltip("The text to show how many enemies are alive")]
        private LocalizedString enemiesAliveText;
        [SerializeField]
        [Tooltip("The text to show when more enemies are being spawned")]
        private LocalizedString spawningText;

        /// <summary>
        /// Sets the starting variables
        /// </summary>
        private void Start()
        {
            enemiesAlive = 0;
            _levelData = gameObject.GetComponent<GameManager>().levelData;
            _countdown = preparationTime;
            _waveIndex = GameStats.Rounds;
            waveText.text = waveCountText.GetLocalizedString() + GameStats.Rounds;
        }
        
        /// <summary>
        /// Updates the countdown and checks if the game should start spawning the next wave.
        /// </summary>
        private void Update()
        {
            // Only reduce the countdown if there are no enemies remaining
            if (enemiesAlive > 0 || _isSpawning)
            {
                if (_isSpawning)
                    return;
                waveProgress.percentage = enemiesAlive / _totalEnemies;
                waveCountdownText.text = enemiesAliveText.GetLocalizedString();
                return;
            }

            if (Mathf.Abs(_countdown - timeBetweenWaves) < 0.00000001f)
            {
                // Save the level
                SaveJsonData(gameObject.GetComponent<GameManager>());
            }

            // If the countdown has finished, call the next wave
            if (_countdown <= 0f)
            {
                // Save the level
                SaveJsonData(gameObject.GetComponent<GameManager>());
                // Start spawning in the enemies
                StartCoroutine(SpawnWave());
                // Reset the timer
                _countdown = timeBetweenWaves;

                return;
            }

            _countdown -= Time.deltaTime;
            _countdown = Mathf.Clamp(_countdown, 0f, Mathf.Infinity);
        
            waveCountdownText.text = $"{_countdown:0.00}";
            waveProgress.percentage = _countdown / timeBetweenWaves;
        }
    
        /// <summary>
        /// Spawns the enemies from an entire wave
        /// </summary>
        private IEnumerator SpawnWave()
        {
            _isSpawning = true;
            waveCountdownText.text = spawningText.GetLocalizedString();
            Wave wave = waves[_waveIndex % waveRepeatIndex];
            if (GameStats.Rounds > waveRepeatIndex)
                wave = waves[(_waveIndex - waveRepeatIndex) % (waves.Length - waveRepeatIndex) + waveRepeatIndex];
            
            GameStats.Rounds = _waveIndex + 1;
            waveText.text = waveCountText.GetLocalizedString() + GameStats.Rounds;
            _totalEnemies = 0;

            for (var i = 0; i < wave.enemySets.Length; i++)
            {
                EnemySet set = wave.enemySets[i];
                waveProgress.percentage = i/ (float) wave.enemySets.Length;
            
                // For all the enemies the enemySet will spawn,
                // spawn one, then wait timeBetweenEnemies seconds
                int setCount = Mathf.FloorToInt(set.count * _levelData.enemyCount.Value.Evaluate(_waveIndex + 1));
                for (var j = 0; j < setCount; j++)
                {
                    SpawnEnemy(set.enemy);
                    _totalEnemies++;

                    if (j + 1 != setCount)
                    {
                        yield return new WaitForSeconds(set.rate);
                    }
                }

                if (i + 1 != wave.enemySets.Length)
                {
                    yield return new WaitForSeconds(wave.setDelays[i]);
                }
            }

            _waveIndex++;
            _isSpawning = false;
        }
    
        /// <summary>
        /// Spawns an enemy and applies scaling based on wave number
        /// </summary>
        /// <param name="enemy">The enemy to spawn</param>
        private void SpawnEnemy(GameObject enemy)
        {
            // Spawn Enemy
            GameObject spawnedEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            spawnedEnemy.name = "_" + spawnedEnemy.name;
            spawnedEnemy.layer = LayerMask.NameToLayer("Enemies");
        
            // Apply scaling
            spawnedEnemy.GetComponent<Enemy>().maxHealth *= _levelData.health.Value.Evaluate(_waveIndex + 1);
            spawnedEnemy.GetComponent<Enemy>().health = spawnedEnemy.GetComponent<Enemy>().maxHealth;
        
            enemiesAlive++;
        }
        
        /// <summary>
        /// Saves the settings to json data
        /// </summary>
        private static void SaveJsonData(ISaveableLevel level)
        {
            var saveData = new SaveLevel();
            level.PopulateSaveData(saveData);
            
            SaveManager.SaveLevel(level, SceneManager.GetActiveScene().name);
        }
    }
}
