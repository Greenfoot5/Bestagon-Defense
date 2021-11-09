using System;
using System.Collections.Generic;
using UnityEngine;

namespace Turrets.Upgrades
{
    [CreateAssetMenu(fileName = "SniperT0", menuName = "Upgrades/Sniper")]
    public class Sniper : Upgrade
    {
        [Header("Bullet Turret")]
        public float bulletRange;
        public float bulletDamage;
        public float bulletFireRate;
        public float bulletTurnSpeed;
        public float bulletSpeed;

        [Header("Laser Turret")]
        public float laserRange;
        public float laserTurnSpeed;
        public float laserDamage;

        [Header("Area")]
        public float areaRange;
        public float areaDamage;
        public float areaFireRate;
        
        public override void AddUpgrade(Turret turret)
        {
            switch (turret.GetType().ToString())
            {
                case "Bullet":
                    turret.range += turret.range * bulletRange;
                    turret.fireRate *= 1 + bulletFireRate;
                    turret.turnSpeed -= turret.turnSpeed * bulletTurnSpeed;
                    break;
                case "Laser":
                    turret.range += turret.range * laserRange;
                    turret.turnSpeed -= turret.turnSpeed * laserTurnSpeed;
                    turret.damage += turret.damage * laserDamage;
                    break;
                case "Smasher":
                    turret.range += turret.range * areaRange;
                    turret.damage += turret.damage * areaDamage;
                    turret.fireRate *= 1 + areaFireRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void RemoveUpgrade(Turret turret)
        {
            switch (turret.GetType().ToString())
            {
                case "Bullet":
                    turret.range -= turret.range * bulletRange;
                    turret.fireRate /= 1 + bulletFireRate;
                    turret.turnSpeed -= turret.turnSpeed * bulletTurnSpeed;
                    break;
                case "Laser":
                    turret.range -= turret.range * laserRange;
                    turret.turnSpeed -= turret.turnSpeed * laserTurnSpeed;
                    turret.damage -= turret.damage * laserDamage;
                    break;
                case "Smasher":
                    turret.range -= turret.range * areaRange;
                    turret.damage -= turret.damage * areaDamage;
                    turret.fireRate /= 1 + areaFireRate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnShoot(Bullet bullet)
        {
            bullet.damage += bullet.damage * bulletDamage;
            bullet.speed += bullet.speed * bulletSpeed;
        }

        public override void OnHit(IEnumerable<Enemy> targets) { }
    }
}