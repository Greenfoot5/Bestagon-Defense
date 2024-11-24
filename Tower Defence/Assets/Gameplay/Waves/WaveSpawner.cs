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
        private enum State
        {
            // Countdown to next spawn
            Countdown,
            // Spawning Enemies
            Spawning,
            // Waiting for all enemies to die
            Waiting,
        }
        
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
        
        private int _waveIndex;
        
        [SerializeField]
        [Tooltip("The location to spawn the enemies. Should be the first waypoint")]
        private Transform spawnPoint;

        private State _currentState = State.Waiting;
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
            _waveIndex = GameStats.Rounds - 1;
            GameStats.OnRoundProgress += UpdateWaveText;
        }

        private void OnDestroy()
        {
            GameStats.OnRoundProgress -= UpdateWaveText;
        }
        
        /// <summary>
        /// Updates the countdown and checks if the game should start spawning the next wave.
        /// </summary>
        private void Update()
        {
            switch (_currentState)
            {
                case State.Spawning:
                    return;
                // If still waiting
                case State.Waiting when enemiesAlive > 0:
                    waveProgress.percentage = enemiesAlive / _totalEnemies;
                    waveCountdownText.text = enemiesAliveText.GetLocalizedString();
                    return;
                // If done waiting
                case State.Waiting when enemiesAlive <= 0:
                    _currentState = State.Countdown;
                    _waveIndex++;
                    GameStats.Rounds = _waveIndex + 1;
                    waveText.text = waveCountText.GetLocalizedString() + GameStats.Rounds;
                    SaveJsonData(gameObject.GetComponent<GameManager>());
                    break;
            }

            // If the countdown has finished, call the next wave
            if (_currentState == State.Countdown)
            {
                _countdown -= Time.deltaTime;
                _countdown = Mathf.Clamp(_countdown, 0f, Mathf.Infinity);

                waveCountdownText.text = $"{_countdown:0.00}";
                waveProgress.percentage = _countdown / timeBetweenWaves;
                
                if (_countdown <= 0f)
                {
                    // Save the level
                    SaveJsonData(gameObject.GetComponent<GameManager>());
                    // Start spawning in the enemies
                    StartCoroutine(SpawnWave());
                }
            }
        }
    
        /// <summary>
        /// Spawns the enemies from an entire wave
        /// </summary>
        private IEnumerator SpawnWave()
        {
            _currentState = State.Spawning;
            waveCountdownText.text = spawningText.GetLocalizedString();
            Wave wave = waves[_waveIndex % waveRepeatIndex];
            if (GameStats.Rounds > waveRepeatIndex)
                wave = waves[(_waveIndex - waveRepeatIndex) % (waves.Length - waveRepeatIndex) + waveRepeatIndex];
            
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
            
            _countdown = timeBetweenWaves;
            _currentState = State.Waiting;
        }
    
        /// <summary>
        /// Spawns an enemy and applies scaling based on wave number
        /// </summary>
        /// <param name="enemy">The enemy to spawn</param>
        private void SpawnEnemy(GameObject enemy)
        {
            enemiesAlive++;
            
            // Spawn Enemy
            GameObject spawnedEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            spawnedEnemy.name = "_" + spawnedEnemy.name;
            spawnedEnemy.layer = LayerMask.NameToLayer("Enemies");
        
            // Apply scaling
            var enemyComponent = spawnedEnemy.GetComponent<Enemy>();
            enemyComponent.maxHealth *= _levelData.health.Value.Evaluate(_waveIndex + 1);
            enemyComponent.health = spawnedEnemy.GetComponent<Enemy>().maxHealth;
            
            enemyComponent.OnDeath += () => { enemiesAlive--; };
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

        private void UpdateWaveText()
        {
            waveText.text = waveCountText.GetLocalizedString() + GameStats.Rounds;
        }
    }
}
