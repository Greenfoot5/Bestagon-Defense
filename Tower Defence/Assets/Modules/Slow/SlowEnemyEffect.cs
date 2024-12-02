using Abstract;
using Enemies;
using UnityEngine;

namespace Modules.Slow
{
    [CreateAssetMenu(fileName = "SlowEffectT0", menuName = "Effects/Slow")]
    public class SlowEnemyEffect : EnemyEffect
    {
        [Tooltip("Multiplicative percentage modifier enemy's speed")]
        [SerializeField]
        private float slowPercentage;

        public override bool Apply(Enemy target)
        {
            if (!base.Apply(target))
                return false;
            
            float slowValue = 1f - slowPercentage;

            // TODO - Is this required with unique effects? Maybe we shouldn't return and enemy should handle minimum speed?
            if (Target.speed.GetModifier() * slowValue <= 0.4f)
            {
                return false;
            }
            
            Target.speed.MultiplyModifier(slowValue);

            return true;
        }

        protected override void Remove()
        {
            base.Remove();
            
            float slowValue = 1f - slowPercentage;
            
            Target.speed.DivideModifier(slowValue);
        }

        protected override void DoEffect() { }
    }
}