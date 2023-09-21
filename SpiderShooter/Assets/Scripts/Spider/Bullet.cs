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
        private uint ownerNetId;
        public uint OwnerNetId => ownerNetId;

        [SerializeField]
        [InspectorReadOnly]
        private TeamColor friendlyTeam;
        public TeamColor FriendlyTeam => friendlyTeam;

        [SerializeField]
        [InspectorReadOnly]
        private float destroyAfter = 4;

        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
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

        public void SetFriendlyColor(TeamColor teamColor)
        {
            friendlyTeam = teamColor;
        }

        public void SetOwnerNetId(uint netId)
        {
            ownerNetId = netId;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (other.TryGetComponent(out SpiderImpl spider) && spider.TeamColor == friendlyTeam)
            {
                return;
            }

            DestroySelf();
        }
    }
}
