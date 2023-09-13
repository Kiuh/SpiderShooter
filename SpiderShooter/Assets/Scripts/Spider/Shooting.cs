using Assets.Scripts.Spider;
using UnityEngine;

namespace Spider
{
    [AddComponentMenu("Spider.Shooting")]
    public class Shooting : MonoBehaviour
    {
        [SerializeField]
        private Transform shootPoint;

        [SerializeField]
        private Bullet bulletPrefab;

        [SerializeField]
        private Movement movement;

        public void Shoot(SpiderImpl spider)
        {
            Bullet bullet = Instantiate(bulletPrefab, shootPoint.position, new Quaternion());
            bullet.SetFlyingDirection(movement.MovingDirection);
            bullet.SetFlyingSpeed(spider.BulletSpeed);
            bullet.SetDamage(spider.BulletDamage);
        }
    }
}
