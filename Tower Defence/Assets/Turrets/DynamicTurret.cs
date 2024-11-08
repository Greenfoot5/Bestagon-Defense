using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abstract.Data;
using Enemies;
using UnityEngine;

namespace Turrets
{
    public abstract class DynamicTurret : Turret
    {
        /// <summary>
        /// The various targeting methods a turret can use to find a target
        /// </summary>
        public enum TargetingMethod
        {
            Closest = 0,
            Weakest,
            Strongest,
            First,
            Last
        }
        
        // How long between each target update
        private const float UpdateTargetTimer = 0.5f;
        
        // Targeting
        [Tooltip("What TargetingMethod the turret uses to pick it's next target")]
        public TargetingMethod targetingMethod = TargetingMethod.Closest;
        [Tooltip("If the turret should always be searching for the target that best matches the targeting method.\n\n" +
                 "If false, it keeps one until the target dies or goes out of range")]
        [SerializeField]
        private bool aggressiveRetargeting;
        
        [Tooltip("The current target")]
        protected Transform target;
        [Tooltip("The Enemy script of the current target")]
        private Enemy _targetEnemy;

        // Reference
        [Tooltip("The transform at which attack from (e.g. instantiate bullets)")]
        [SerializeField]
        protected Transform firePoint;

        // Rotation
        [Tooltip("How fast the turret rotates towards it's target")]
        public UpgradableStat rotationSpeed = new(3f);
        [Tooltip("The Transform to perform any rotations on")]
        [SerializeField]
        protected Transform partToRotate;
        
        /// <summary>
        /// Begins the target searching
        /// </summary>
        private void Start()
        {
            // Start finding targets
            StartCoroutine(TargetCoroutine());
        }
        
        /// <summary>
        /// Calls our targeting method every UpdateTargetTimer.
        /// </summary>
        private IEnumerator TargetCoroutine()
        {
            while (gameObject.activeSelf)
            {
                UpdateTarget();
                yield return new WaitForSeconds(UpdateTargetTimer);
            }
        }

        /// <summary>
        /// Update our current target to check if it's still the most valuable, or pick a new one.
        /// </summary>
        private void UpdateTarget()
        {
            // If the turret is not aggressively retargeting, check if the target is still in range
            if (!aggressiveRetargeting && target != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, target.position);
                if (distanceToEnemy <= range.GetStat()) return;
            }

            // Create a list of enemies within range
            GameObject[] enemiesInRange = (from enemy in GameObject.FindGameObjectsWithTag(enemyTag)
                let distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position)
                where distanceToEnemy <= range.GetStat()
                select enemy).ToArray();
            // Set the current value to be too high or too low.
            // Value is based on targeting method
            float currentValue = Mathf.Infinity;
            if (targetingMethod == TargetingMethod.Strongest || targetingMethod == TargetingMethod.First)
            {
                currentValue = Mathf.NegativeInfinity;
            }

            GameObject mostValuableEnemy = null;

            // Check there are enemies in range, and if not, the turret has no target
            if (enemiesInRange.Length == 0)
            {
                target = null;
                return;
            }

            // Loop through the enemies and find the most valuable
            foreach (GameObject enemy in enemiesInRange)
            {
                switch (targetingMethod)
                {
                    case TargetingMethod.Closest:
                        // Find if the enemy is closer than our current most valuable
                        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                        if (distanceToEnemy < currentValue)
                        {
                            currentValue = distanceToEnemy;
                            mostValuableEnemy = enemy;
                        }

                        break;
                    case TargetingMethod.Weakest:
                        // Find if the enemy has less health than our current most valuable
                        float health = enemy.GetComponent<Enemy>().health;
                        if (health < currentValue)
                        {
                            currentValue = health;
                            mostValuableEnemy = enemy;
                        }

                        break;
                    case TargetingMethod.Strongest:
                        // Find if the enemy has more health than our current most valuable
                        float enemyHealth = enemy.GetComponent<Enemy>().health;
                        if (enemyHealth > currentValue)
                        {
                            currentValue = enemyHealth;
                            mostValuableEnemy = enemy;
                        }

                        break;
                    case TargetingMethod.First:
                        // Find if the enemy has the most map progress than our current most valuable
                        float mapProgress = enemy.GetComponent<EnemyMovement>().mapProgress;
                        if (mapProgress > currentValue)
                        {
                            currentValue = mapProgress;
                            mostValuableEnemy = enemy;
                        }

                        break;
                    case TargetingMethod.Last:
                        // Find if the enemy has the lease map progress than our current most valuable
                        float pathProgress = enemy.GetComponent<EnemyMovement>().mapProgress;
                        if (pathProgress < currentValue)
                        {
                            currentValue = pathProgress;
                            mostValuableEnemy = enemy;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            // Check if the turret have a valid target
            if (mostValuableEnemy != null)
            {
                target = mostValuableEnemy.transform;
                _targetEnemy = target.GetComponent<Enemy>();
            }
            // Set our target to null if there is none
            else
            {
                target = null;
            }
        }
        
        /// <summary>
        /// Rotates the turret towards our target
        /// </summary>
        protected void LookAtTarget()
        {
            Vector2 aimDir = (target.position - partToRotate.position).normalized;

            float rotationAngleNeed = Vector2.SignedAngle(partToRotate.up, aimDir);
            float zAngle = Mathf.Clamp(rotationAngleNeed, -rotationSpeed.GetStat() * Time.deltaTime,
                rotationSpeed.GetStat() * Time.deltaTime);
            partToRotate.Rotate(0f, 0f, zAngle);
        }

        /// <summary>
        /// Check the turret is currently looking at our target.
        /// Used to see if the turret can shoot or needs to rotate more
        /// </summary>
        /// <returns>If the turret is currently looking at the target</returns>
        protected bool IsLookingAtTarget()
        {
            if (_targetEnemy == null) return false;

            // Setup the raycast
            var results = new List<RaycastHit2D>();
            var contactFilter = new ContactFilter2D()
            {
                layerMask = LayerMask.GetMask("Enemies")
            };
            Physics2D.Raycast(firePoint.position, firePoint.up, contactFilter, results, range.GetStat());

            // Loop through the hits to see if the turret can hit the target
            var foundEnemy = false;
            foreach (RaycastHit2D unused in results.Where(hit => hit.transform == _targetEnemy.transform))
            {
                foundEnemy = true;
            }
            return foundEnemy;
        }
    }
}