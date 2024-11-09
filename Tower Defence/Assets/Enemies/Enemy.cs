using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _WIP.Abilities;
using Abstract.Data;
using Gameplay;
using Gameplay.Waves;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies
{
    /// <summary>
    /// The base skeleton for the enemy, holding its stats and abilities
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [Tooltip("The starting speed of the enemy")]
        public float startSpeed = 2f;
        [Tooltip("The starting maximum health of the enemy")]
        public float maxHealth = 20f;
        [Tooltip("A percentage modifier to any knockback taken")]
        public float knockbackModifier = 1f;
        
        [HideInInspector]
        [Tooltip("The current speed of the enemy")]
        public UpgradableStat speed;
        [HideInInspector]
        [Tooltip("The current health of the enemy")]
        public float health;
    
        [Header("Death Stats")]
        [Tooltip("The amount of money to grant the player when the enemy is kill")]
        public int deathMoney = 10;
        [Tooltip("The amount of lives lost if the enemy finishes the path")]
        public int deathLives = 1;
        [Tooltip("The amount of money to grant the player if the enemy finishes the path")]
        public int endPathMoney = 10;

        [Header("Health Bar")]
        [Tooltip("The left health bar")]
        public Image leftBar;
        [Tooltip("The right health bar")]
        public Image rightBar;

        [Space(20f)]
        [Tooltip("The root game object to rotate to change the enemy's looking direction")]
        public GameObject rotationRoot;
        [SerializeField]
        [Tooltip("If the enemy rotates towards the next waypoint")]
        public bool doesRotation = true;
        [Tooltip("The particle effect prefab to spawn when the enemy dies")]
        public GameObject deathEffect;
        
        [Header("Abilities")]
        [Tooltip("Any abilities the enemy starts with")]
        public EnemyAbility[] startingAbilities;
        [Tooltip("The parent object for any ability icons so they have the correct layout")]
        public GameObject iconLayout;

        [Header("Bosses")]
        [Tooltip("If the enemy is a boss")]
        public bool isBoss;

        // Unique Effects/Immunities
        // Effects that can only be applied once are added and removed at runtime
        // as they are immune to getting the effect while they have it
        [Header("Effect Immunities")]
        [Tooltip("A list of effect names (internal names) that the enemy is immune to" +
                 "\n\nDuring runtime, also contains any unique effects applied to the enemy as they are immune while they have it")]
        public List<string> uniqueEffects;
        
        // Abilities for each trigger
        private readonly List<EnemyAbility> _timerAbilities = new();
        private readonly List<(EnemyAbility ability, int count)> _hitAbilities = new();
        private readonly List<EnemyAbility> _deathAbilities = new();
        private readonly List<EnemyAbility> _finishAbilities = new();

        // If the enemy has died
        private bool _isDead;
        public delegate void DeathEvent();
        public event DeathEvent OnDeath;
        
        /// <summary>
        /// Grants abilities and sets current stats to max when spawning the enemy
        /// </summary>
        private void Awake()
        {
            speed = new UpgradableStat(startSpeed);
            health = maxHealth;
            
            // Add each starting ability to the correct list
            foreach (EnemyAbility ability in startingAbilities)
            {
                GrantAbility(ability);
            }
        }
        
        /// <summary>
        /// Grants the enemy an ability so they can use it when triggered
        /// </summary>
        /// <param name="ability">The ability to grant</param>
        private void GrantAbility(EnemyAbility ability)
        {
            // TODO - Have better ability checking. Perhaps check if one is a higher tier than another,
            // TODO - or, for an extra challenge, reset the timer on the current one.
            Transform existingIcon = iconLayout.transform.Find($"{ability.name} Icon");
            if (existingIcon != null)
            {
                return;
            }
            
            // Adds to ability to the lists based on it's trigger(s)
            if (ability.triggers.Contains(AbilityTrigger.OnTimer))
            {
                _timerAbilities.Add(ability);
                StartCoroutine(TimerAbility(ability));
            }
            if (ability.triggers.Contains(AbilityTrigger.OnDamage))
            {
                _hitAbilities.Add((ability, (int) ability.startCount));
            }
            if (ability.triggers.Contains(AbilityTrigger.OnDeath))
            {
                _deathAbilities.Add(ability);
            }
            if (ability.triggers.Contains(AbilityTrigger.OnEnd))
            {
                _finishAbilities.Add(ability);
            }
            if (ability.triggers.Contains(AbilityTrigger.OnGrant))
            {
                ActivateAbilities(new[] {ability}, null);
                if (ability.startCount > 0)
                {
                    StartCoroutine(GrantedAbilityCounter(ability));
                }
            }
            
            // Adds the icon above the enemy's health bar
            var icon = new GameObject($"{ability.name} Icon");
            icon.AddComponent(typeof(Image));
            icon.GetComponent<Image>().sprite = ability.abilityIcon;
            icon.transform.SetParent(iconLayout.transform);

            // Fix the icon's size
            var iLayoutTransform = (RectTransform)iconLayout.transform;
            var iTransform = (RectTransform)icon.transform;
            float ratio = ability.abilityIcon.rect.height / iLayoutTransform.rect.height;
            iTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ability.abilityIcon.rect.width / ratio);
        }
        
        /// <summary>
        /// Removes an ability from the enemy
        /// </summary>
        /// <param name="ability">The ability to remove</param>
        private void RevokeAbility(EnemyAbility ability)
        {
            if (ability.triggers.Contains(AbilityTrigger.OnTimer))
            {
                _timerAbilities.Remove(ability);
            }
            if (ability.triggers.Contains(AbilityTrigger.OnDamage))
            {
                // TODO - Find the counter properly
                _hitAbilities.Remove((ability, 0));
            }
            if (ability.triggers.Contains(AbilityTrigger.OnDeath))
            {
                _deathAbilities.Remove(ability);
            }
            if (ability.triggers.Contains(AbilityTrigger.OnEnd))
            {
                _finishAbilities.Remove(ability);
            }

            Transform icon = iconLayout.transform.Find($"{ability.name} Icon");
            Destroy(icon.gameObject);
        }

        private IEnumerator GrantedAbilityCounter(EnemyAbility ability)
        {
            yield return new WaitForSeconds(ability.startCount);
            
            RevokeAbility(ability);
            EndCounterAbility(ability, null);
        }
        
        /// <summary>
        /// Activates abilities on a timer
        /// </summary>
        /// <param name="ability">The ability to activate</param>
        /// <returns></returns>
        private IEnumerator TimerAbility(EnemyAbility ability)
        {
            float counter = ability.startCount;
            // Check the enemy still has the ability to use
            while (_timerAbilities.Contains(ability))
            {
                yield return new WaitForSeconds(ability.timer);
                
                // Check we aren't stunned
                if (speed.GetStat() <= 0)
                    continue;
                
                ActivateAbilities(new[] {ability}, null);
                
                // Decrease the counter
                counter -= 1;
                if (counter != 0) continue;
                
                EndCounterAbility(ability, null);
                // Remove the ability
                RevokeAbility(ability);
                yield break;
                
            }
        }
    
        /// <summary>
        /// Called whenever the enemy takes damage.
        /// This activates any ability with the OnDamage trigger
        /// </summary>
        /// <param name="amount">The amount of damage to deal</param>
        /// <param name="source">The GameObject that hurt the enemy</param>
        public void TakeDamage(float amount, GameObject source)
        {
            List<EnemyAbility> abilities = _hitAbilities.Select(item => item.ability).ToList();
            ActivateAbilities(abilities, source);
            
            // Check we aren't stunned
            if (speed.GetStat() <= 0)
                abilities.Clear();

            foreach ((EnemyAbility ability, int count) t in _hitAbilities)
            {
                // Decrease the counter
                (EnemyAbility ability, int count) = t;
                count -= 1;
                if (count != 0) continue;
                
                // Remove the ability
                RevokeAbility(ability);
                EndCounterAbility(ability, source);
            }
            
            // Edit the health
            health -= amount;

            leftBar.fillAmount = health / maxHealth;
            rightBar.fillAmount = health / maxHealth;

            if (health <= 0)
            {
                Die(gameObject);
            }
        }
        
        /// <summary>
        /// Called whenever the enemy takes damage
        /// but the source doesn't want OnDamage abilities to activate.
        /// For example, when taking effect damage.
        /// </summary>
        /// <param name="amount">The amount of damage to deal</param>
        public void TakeDamageWithoutAbilities(float amount)
        {
            // Base health stuff
            health -= amount;

            leftBar.fillAmount = health / maxHealth;
            rightBar.fillAmount = health / maxHealth;

            if (health <= 0)
            {
                Die(gameObject);
            }
        }

        /// <summary>
        /// Called when the enemy dies
        /// allows the game to clean up anything when removing the GameObject
        /// </summary>
        /// <param name="source">What killed the enemy</param>
        private void Die(GameObject source)
        {
            // Make sure we're not already dead.
            if (_isDead)
                return;
            _isDead = true;

            OnDeath?.Invoke();
            
            // Only do the abilities if the enemy isn't stunned
            if (speed.GetStat() > 0)
                ActivateAbilities(_deathAbilities, source);

            // Spawn death effect
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
            effect.name = "_" + effect.name;
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        
            // Let the wave spawner know the enemy is dead
            WaveSpawner.enemiesAlive--;
        
            // Grant money and destroy self
            GameStats.Energy += deathMoney;
            Destroy(gameObject);
        }

        /// <summary>
        /// Called when the enemy reaches the end of the map's path
        /// Activates any finishPath abilities
        /// </summary>
        public void FinishPath()
        {
            ActivateAbilities(_finishAbilities, null);
            
            // Let our other systems know the enemy reached the end
            GameStats.Lives -= deathLives;
            WaveSpawner.enemiesAlive--;
            GameStats.Energy += endPathMoney;
        
            Destroy(gameObject);
        }
        
        /// <summary>
        /// Activates an IEnumerable of abilities
        /// </summary>
        /// <param name="abilities">The IEnumerable of abilities to activate</param>
        /// <param name="source">The GameObject that triggered the activation (if any)</param>
        /// <exception cref="ArgumentOutOfRangeException">An ability has an invalid targeting type</exception>
        private void ActivateAbilities(IEnumerable<EnemyAbility> abilities, GameObject source)
        {
            foreach (EnemyAbility ability in abilities)
            {
                // TODO - Replace with an actual duration
                // Spawn ability effect
                //GameObject effect = Instantiate(ability.abilityEffect, transform.position, Quaternion.identity);
                //effect.name = "_" + effect.name;
                switch (ability.targetingType)
                {
                    case AbilityTarget.Single:
                        // The target may be the damage source or the enemy itself
                        ability.Activate(source != null ? source : gameObject);
                        break;
                    case AbilityTarget.Radius:
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ability.range);
                        foreach (Collider2D coll in colliders)
                        {
                            if (!coll.CompareTag("Enemy") && !coll.CompareTag("Turret")) continue;
                            
                            ability.Activate(coll.gameObject);
                        }
                        break;
                    case AbilityTarget.All:
                        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                        foreach (GameObject target in turrets)
                        {
                            ability.Activate(target);
                        }
                        
                        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                        foreach (GameObject target in enemies)
                        {
                            ability.Activate(target);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                //Destroy(effect, 0.6f);
            }
        }
        
        /// <summary>
        /// Called when a counter ability has finished
        /// </summary>
        /// <param name="ability">The ability to end</param>
        /// <param name="source">What caused the ability to end</param>
        /// <exception cref="ArgumentOutOfRangeException">If the ability has an invalid target</exception>
        private void EndCounterAbility(EnemyAbility ability, GameObject source)
        {
            // Spawn ability effect
            GameObject effect = Instantiate(ability.abilityEffect, transform.position, Quaternion.identity);
            effect.name = "_" + effect.name;
            switch (ability.targetingType)
            {
                case AbilityTarget.Single:
                    // The target may be the damage source or the enemy itself
                    ability.OnCounterEnd(source);
                    ability.OnCounterEnd(gameObject);
                    break;
                case AbilityTarget.Radius:
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ability.range);
                    foreach (Collider2D coll in colliders)
                    {
                        if (!coll.CompareTag("Enemy") && !coll.CompareTag("Turret")) continue;
                        
                        ability.OnCounterEnd(coll.gameObject);
                    }
                    break;
                case AbilityTarget.All:
                    GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
                    foreach (GameObject target in turrets)
                    {
                        ability.OnCounterEnd(target);
                    }
                    
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (GameObject target in enemies)
                    {
                        ability.OnCounterEnd(target);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
        }
    }
}
