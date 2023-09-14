using Cinemachine;
using Mirror;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Spider
{
    [SelectionBase]
    [AddComponentMenu("Spider.SpiderImpl")]
    public class SpiderImpl : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        private float health;

        [SerializeField]
        private float bulletSpeed;
        public float BulletSpeed => bulletSpeed;

        [SerializeField]
        private float bulletDamage;
        public float BulletDamage => bulletDamage;

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                CinemachineVirtualCamera virtualCamera =
                    FindObjectsOfType<CinemachineVirtualCamera>()
                        .First(x => x.gameObject.CompareTag("MainCamera"));
                virtualCamera.LookAt = transform;
                virtualCamera.Follow = transform;
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
