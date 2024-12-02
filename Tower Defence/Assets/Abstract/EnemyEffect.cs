using Enemies;
using UnityEngine;

namespace Abstract
{
    public abstract class EnemyEffect : ScriptableObject
    {
        [Tooltip("The amount of ticks the effect starts with")]
        public int tickCount;
        [Tooltip("How long each tick lasts in seconds")]
        public float tickDuration;
        [Tooltip("The number of ticks the effect currently has left")]
        [HideInInspector]
        public int ticksLeft;

        private float _durationLeft;
        
        [Tooltip("The effect type to use when checking duplicates and immunities")]
        public string effectType;
        [Tooltip("The tier of effect")]
        public int tier;
        [Tooltip("If the effect should be stopped after the current tick")]
        [HideInInspector]
        public bool isCancelled;

        [Tooltip("The enemy the effect has been applied to")]
        protected Enemy Target;

        public void Update()
        {
            if (isCancelled)
            {
                Remove();
                return;
            }
            
            if (_durationLeft > 0)
            {
                _durationLeft -= Time.deltaTime;
                return;
            }
            
            ticksLeft--;
            if (ticksLeft >= 0)
            {
                _durationLeft = tickDuration;
                DoEffect();
                return;
            }

            Remove();
        }

        public virtual bool Apply(Enemy target)
        {
            Target = target;
            ticksLeft = tickCount;
            
            if (Target.uniqueEffects.Contains(effectType))
                return false;

            if (Target.ActiveEffects.ContainsKey(effectType))
            {
                if (Target.ActiveEffects[effectType].tier > tier && !Target.ActiveEffects[effectType].isCancelled)
                {
                    return false;
                }

                if (Target.ActiveEffects[effectType].tier == tier)
                {
                    Target.ActiveEffects[effectType].ticksLeft = tickCount;
                    Target.ActiveEffects[effectType]._durationLeft = 0f;
                    return false;
                }
                if (Target.ActiveEffects[effectType].tier <= tier)

                if (Target.ActiveEffects[effectType].tier < tier)
                {
                    Target.ActiveEffects[effectType].isCancelled = true;
                    Target.ActiveEffects.Remove(effectType);
                }
            }
            
            Target.ActiveEffects.Add(effectType, this);
            Target.OnDeath += CancelFromDeath;

            return true;
        }

        protected virtual void Remove()
        {
            Target.ActiveEffects.Remove(effectType);
            Target.OnDeath -= CancelFromDeath;
        }

        protected abstract void DoEffect();

        private void CancelFromDeath()
        {
            isCancelled = true;
        }
    }
}
