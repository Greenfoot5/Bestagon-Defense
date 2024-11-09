using System.Collections;
using Levels._Nodes;
using Unity.Collections;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// A script to move the enemy along our designated path
    /// Set out by a waypoints array
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class EnemyMovement : MonoBehaviour
    {
        // Waypoint indexes
        private Transform _target;
        [HideInInspector]
        public int waypointIndex;

        private Enemy _enemy;
        
        [SerializeField] 
        [Tooltip("The distance from enemy to waypoint before it's considered reached")]
        private float distanceToWaypoint = 0.05f;
        [SerializeField]
        [Tooltip("The duration of knockback")]
        private float knockbackDuration = 0.2f;

        [ReadOnly]
        [Tooltip("How many waypoints the enemy has passed, and the percentage to the next one")]
        public float mapProgress;
        private float _maxDistance;

        /// <summary>
        /// Initialises relevant variables
        /// </summary>
        private void Start()
        {
            // Set the next target to the first waypoint.
            // _waypointIndex will always be 0 at the start
            _target = Waypoints.points[waypointIndex];

            _enemy = GetComponent<Enemy>();
        }

        /// <summary>
        /// Every scene, the enemy needs to move
        /// </summary>
        private void Update()
        {
            if (_enemy is null)
                return;
            
            // If the enemy is moving backwards
            if (_enemy.speed.GetTrueStat() < 0)
            {
                MoveBackwards();
                return;
            }
            
            // Get the direction of the target, and the distance to move this frame
            Vector3 position = transform.position;
            Vector3 location = _target.position;
            float distanceThisFrame = _enemy.speed.GetStat() * Time.deltaTime;

            transform.position = Vector2.MoveTowards(position, location, distanceThisFrame);
            
            Vector2 difference = location - position; // Distance & direction to next target

            // If within this frame the enemy will pass the waypoint, it's a guaranteed hit
            if (difference.sqrMagnitude <= distanceToWaypoint * distanceToWaypoint)
            {
                GetNextWaypoint();
            }
            else
            {
                mapProgress = waypointIndex + 1 - (distanceToWaypoint / _maxDistance);
            }
            
            if (_enemy.doesRotation)
                // Attempt at rotation
                _enemy.rotationRoot.transform.up = (location - position).normalized;
        }
    
        /// <summary>
        /// Gets the next waypoint in the waypoints array
        /// </summary>
        private void GetNextWaypoint()
        {
            // If the enemy has reached the end, destroy
            if (waypointIndex >= Waypoints.points.Length - 1)
            {
                EndPath();
                return;
            }
        
            // Get the next waypoint
            waypointIndex++;
            _target = Waypoints.points[waypointIndex];
            mapProgress = waypointIndex;
            _maxDistance = Vector3.Distance(transform.position, _target.position);
        }
        
        /// <summary>
        /// Moves the enemy backwards along the path.
        /// </summary>
        private void MoveBackwards()
        {
            // If the enemy has reached the start, we can't go backwards further
            if (waypointIndex - 1 < 0)
            {
                return;
            }
                
            // Get the direction and move in that direction
            Vector3 dir = Waypoints.points[waypointIndex - 1].position - transform.position;
            transform.Translate(dir.normalized * (Mathf.Abs(_enemy.speed.GetTrueStat()) * Time.deltaTime), Space.World);
            mapProgress = waypointIndex - (distanceToWaypoint / _maxDistance);
        
            // If the enemy hasn't reached the previous waypoint, there's no point knocking it back further
            if (!(Vector3.Distance(transform.position, Waypoints.points[waypointIndex - 1].position) <= distanceToWaypoint)) return;

            // Get the next waypoint
            waypointIndex--;
            _target = Waypoints.points[waypointIndex];
            mapProgress = waypointIndex;
            _maxDistance = Vector3.Distance(Waypoints.points[waypointIndex + 1].position, _target.position);
        }
    
        /// <summary>
        /// Called when the enemy reach the final waypoint
        /// </summary>
        private void EndPath()
        {
            _enemy.FinishPath();
        }
        
        /// <summary>
        /// Called when a turret wants to deal knockback to an enemy
        /// </summary>
        /// <param name="amount">The amount of knockback to deal</param>
        /// <param name="turretLocation">The location of the turret</param>
        public void TakeKnockback(float amount, Vector3 turretLocation)
        {
            if (_enemy.knockbackModifier == 0)
            {
                return;
            }
            
            Vector3 v = _target.position - transform.position;
            Vector3 w = turretLocation - transform.position;
            float multiplier = Vector3.Dot(v.normalized, w.normalized);
            
            // Actually deal knockback
            // Multiply by -1 to knock backwards
            float knockback = amount * _enemy.knockbackModifier * multiplier * -1;

            StartCoroutine(DealKnockback(knockback));
        }
        
        /// <summary>
        /// Applies a temporary knockback effect to the enemy
        /// </summary>
        /// <param name="speedMultiplier">The temporary multiplier applied to speed to simulate knockback</param>
        /// <returns></returns>
        private IEnumerator DealKnockback(float speedMultiplier)
        {
            _enemy.speed.MultiplyModifier(speedMultiplier);
            yield return new WaitForSeconds(knockbackDuration);
            _enemy.speed.DivideModifier(speedMultiplier);
        }
    }
}
