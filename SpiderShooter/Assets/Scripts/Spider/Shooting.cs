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
        private GameObject bulletPrefab;

        [SerializeField]
        private Movement movement;

        [Command]
        public void Shoot(SpiderImpl spider)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            bullet.GetComponent<Bullet>().SetDamage(spider.BulletDamage);
            NetworkServer.Spawn(bullet);
            RpcOnFire();
        }

        [ClientRpc]
        private void RpcOnFire()
        {
            //animator.SetTrigger("Shoot");
        }
    }
}
