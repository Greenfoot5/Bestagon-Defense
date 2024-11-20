using System.Collections;
using Turrets;
using UnityEngine;

namespace Abstract
{
    public abstract class TurretEffect : ScriptableObject
    {
        public int tickCount;
        public int tickDuration;
        public int ticksLeft;
        public int tier;
        public bool isCancelled = false;

        protected Turret Target;

        public TurretEffect(Turret target)
        {
            Target = target;
        }

        public abstract void Apply();
        public abstract void Remove();
        public abstract void DoEffect();

        public IEnumerator Tick()
        {
            if (isCancelled)
                yield break;
            
            yield return new WaitForSeconds(tickDuration);
            ticksLeft--;

            DoEffect();

            if (ticksLeft <= 0)
            {
                Remove();
                yield break;
            }
        }
    }
}
