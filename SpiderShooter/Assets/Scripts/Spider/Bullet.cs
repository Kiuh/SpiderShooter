using Mirror;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [SelectionBase]
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("SpiderShooter/Spider.Bullet")]
    public class Bullet : NetworkBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private float damage;
        public float Damage => damage;

        [SerializeField]
        [InspectorReadOnly]
        private string ownerNetId;
        public string OwnerNetId => ownerNetId;

        [SerializeField]
        [InspectorReadOnly]
        private TeamColor friendlyTeam;
        public TeamColor FriendlyTeam => friendlyTeam;

        [SerializeField]
        [InspectorReadOnly]
        private float destroyAfter = 25;

        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private Collider ownCollider;

        [SerializeField]
        private float force = 1000;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        [SerializeField]
        private GameObject model;

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

        public void SetFriendlyColor(TeamColor teamColor)
        {
            friendlyTeam = teamColor;
        }

        public void SetOwnerNetId(string netId)
        {
            ownerNetId = netId;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                return;
            }

            if (
                other.gameObject.CompareTag("SpiderBody")
                && other.GetComponentInParent<SpiderImpl>().TeamColor == friendlyTeam
            )
            {
                return;
            }

            rigidBody.velocity = rigidBody.velocity.normalized * 25;
            rigidBody.useGravity = true;
            ownCollider.isTrigger = false;
        }
    }
}
