using Mirror;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Spider.Bullet")]
    public class Bullet : NetworkBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private float damage;
        public float Damage => damage;

        [SerializeField]
        [InspectorReadOnly]
        private float destroyAfter = 4;

        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        [InspectorReadOnly]
        private float force = 1000;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        private void Start()
        {
            rigidBody.AddForce(transform.forward * force);
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            DestroySelf();
        }
    }
}
