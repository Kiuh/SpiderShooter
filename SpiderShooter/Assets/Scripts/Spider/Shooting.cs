using Mirror;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.Shooting")]
    public class Shooting : NetworkBehaviour
    {
        [SerializeField]
        private Transform shootPoint;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private float reloadTime;

        [SerializeField]
        [InspectorReadOnly]
        private float currentReloadTime;

        [SerializeField]
        private float backstepForce;

        private void Update()
        {
            if (currentReloadTime > 0)
            {
                currentReloadTime -= Time.deltaTime;
            }
        }

        [Command]
        public void Shoot(SpiderImpl spider)
        {
            if (currentReloadTime > 0)
            {
                return;
            }

            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.SetDamage(spider.BulletDamage);
            bulletComponent.SetFriendlyColor(spider.TeamColor);
            bulletComponent.SetOwnerNetId(spider.Uid);
            NetworkServer.Spawn(bullet);

            spider.Movement.PushBack(backstepForce);

            currentReloadTime = reloadTime;
        }
    }
}
