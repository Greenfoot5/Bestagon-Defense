using System;
using System.Collections;
using Abstract;
using Enemies;
using Turrets;
using Turrets.Gunner;
using Turrets.Lancer;
using Turrets.Shooter;
using Turrets.Smasher;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Modules.Stun
{
    /// <summary>
    /// Extends the Module class to create a DebuffEnemy upgrade,
    /// Used to add effects to enemies
    /// </summary>
    [CreateAssetMenu(fileName = "StunT0", menuName = "Modules/Stun")]
    public class StunModule : Module
    {
        protected override Type[] ValidTypes => new[] { typeof(Shooter), typeof(Smasher), typeof(Gunner), typeof(Lancer) };

        [FormerlySerializedAs("stunChance")]
        [SerializeField]
        [Tooltip("The percentage chance to stun an enemy")]
        private float enemyStunChance;
        [FormerlySerializedAs("duration")]
        [SerializeField]
        [Tooltip("How long to stun the enemy for")]
        private float enemyDuration;
        [SerializeField]
        [Tooltip("percentage chance to stun the tower each attack")]
        private float turretStunChance;
        [SerializeField]
        [Tooltip("How long to stun the tower for")]
        private float turretDuration;
        [SerializeField]
        [Tooltip("The VFX to spawn when the turret is stunned")]
        private GameObject stunEffect;
        [SerializeField]
        [Tooltip("The VFX to spawn when the ends it's turret stun")]
        private GameObject stunEndEffect;

        public override void AddModule(Damager damager)
        {
            damager.OnHit += OnHit;
            damager.OnAttack += OnAttack;
        }

        public override void RemoveModule(Damager damager)
        {
            damager.OnHit -= OnHit;
            damager.OnAttack -= OnAttack;
        }

        /// <summary>
        /// Adds the EnemyAbility to some target(s)
        /// </summary>
        /// <param name="target">The target(s) to apply the ability to</param>
        /// <param name="damager">The turret that attacked the enemies</param>
        /// <param name="bullet">The bullet (if any) that hit the enemies</param>
        private void OnHit(Enemy target, Damager damager, Bullet bullet = null)
        {
            if (damager is not Turret turret) return;
            Runner.Run(StunEnemy(target, turret));
        }
        
        /// <summary>
        /// Applies the slow effect for a set duration
        /// </summary>
        /// <param name="target">The enemy to stun</param>
        /// <param name="turret">The turret attempting to stun</param>
        private IEnumerator StunEnemy(Enemy target, Turret turret)
        {
            // Check the enemy isn't already stunned/has immunity
            if (target.uniqueEffects.Contains("Stun"))
            {
                yield break;
            }
            target.uniqueEffects.Add("Stun");
            
            float originalSpeed = target.speed.GetBase();
            // Check the target isn't already stunned (again) and the turret hit the chance
            if (originalSpeed <= 0 || Random.value > (enemyStunChance / turret.fireRate.GetStat())) yield break;
            
            target.speed.SetBase(0);

            yield return new WaitForSeconds(enemyDuration);

            target.speed.SetBase(originalSpeed);
        }

        private void OnAttack(Damager damager)
        {
            if (damager is not Turret turret) return;
            if (Random.value < turretStunChance) 
                Runner.Run(StunTurret(turret));
        }

        private IEnumerator StunTurret(Turret turret)
        {
            float originalFireRate = turret.fireRate.GetBase();
            float originalRotSpeed = 1;
            if (turret is DynamicTurret dynamicTurret)
                originalRotSpeed = dynamicTurret.rotationSpeed.GetBase();

            // Check the turret isn't already stunned
            if (originalFireRate <= 0)
                yield break;
            
            // Summons the stun particle effect
            Vector3 position = turret.transform.position;
            GameObject effect = Instantiate(stunEffect, position, Quaternion.identity);
            effect.name = "_" + effect.name;
            Destroy(effect, effect.GetComponent<ParticleSystem>().main.duration);
            
            // Updates the fire rate and rotation speed
            turret.fireRate.SetBase(0);
            if (turret is DynamicTurret dynamicTurret1)
                dynamicTurret1.rotationSpeed.SetBase(0);
            TurretInfo.instance.UpdateStats();
            
            yield return new WaitForSeconds(turretDuration);
            
            // Summons the end stun particle effect
            GameObject endEffect = Instantiate(stunEndEffect, position, Quaternion.identity);
            endEffect.name = "_" + endEffect.name;
            Destroy(endEffect, endEffect.GetComponent<ParticleSystem>().main.duration);
            
            // Resets fire rate and rotation speed
            turret.fireRate.SetBase(originalFireRate);
            if (turret is DynamicTurret dynamicTurret2)
                dynamicTurret2.rotationSpeed.SetBase(originalRotSpeed);
            TurretInfo.instance.UpdateStats();
        }
    }
}
