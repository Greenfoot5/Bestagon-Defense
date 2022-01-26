using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "BurnEnemy", menuName = "Enemy Abilities/Burn Enemy")]
    public class BurnEnemyEffect : EnemyAbility
    {
        [Header("Ability Stats")]
        [Tooltip("% of max health damage per tick")]
        public float damage;
    
        public override void Activate(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            
            // Check the target is an enemy
            var enemyComponent = target.GetComponent<Enemy>();
            if (enemyComponent == null)
            {
                return;
            }

            enemyComponent.TakeDamageWithoutAbilities(enemyComponent.maxHealth * damage);
        }

        public override void OnCounterEnd(GameObject target) { }
    }
}