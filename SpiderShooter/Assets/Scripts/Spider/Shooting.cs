using Mirror;
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

        [Command]
        public void Shoot(SpiderImpl spider)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletComponent.SetDamage(spider.BulletDamage);
            bulletComponent.SetFriendlyColor(spider.TeamColor);
            bulletComponent.SetOwnerNetId(spider.netIdentity.netId);
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
