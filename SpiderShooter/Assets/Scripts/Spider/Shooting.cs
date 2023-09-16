using Mirror;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("Spider.Shooting")]
    public class Shooting : NetworkBehaviour
    {
        [SerializeField]
        private Transform shootPoint;

        [SerializeField]
        private Bullet bulletPrefab;

        [SerializeField]
        private Movement movement;

        [Command]
        public void Shoot(SpiderImpl spider)
        {
            Bullet bullet = Instantiate(bulletPrefab, shootPoint.position, new Quaternion());
            bullet.SetFlyingDirection(movement.MovingDirection);
            bullet.SetFlyingSpeed(spider.BulletSpeed);
            bullet.SetDamage(spider.BulletDamage);
            NetworkServer.Spawn(bullet.gameObject);
        }
    }
}
