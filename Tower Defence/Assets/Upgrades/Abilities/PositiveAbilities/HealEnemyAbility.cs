using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "TimedMinorSelfHeal", menuName = "Enemy Abilities/Heal Enemy")]
    public class HealEnemyAbility : EnemyAbility
    {
        [Header("Ability Stats")]
        public bool isPercentage = true;
        public int healAmount = 20;
        public float healPercentage = 0.2f;

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
            
            // Heal the target
            if (isPercentage)
            {
                enemyComponent.health += enemyComponent.maxHealth * healPercentage;
            }
            else
            {
                enemyComponent.health += healAmount;
            }
            
            // Make sure health isn't above cap
            if (enemyComponent.health > enemyComponent.maxHealth)
            {
                enemyComponent.health = enemyComponent.maxHealth;
            }

            enemyComponent.leftBar.fillAmount = enemyComponent.health / enemyComponent.maxHealth;
            enemyComponent.rightBar.fillAmount = enemyComponent.health / enemyComponent.maxHealth;
        }

        public override void OnCounterEnd(GameObject target) { }
    }
}
