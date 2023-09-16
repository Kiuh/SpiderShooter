using Cinemachine;
using Mirror;
using SpiderShooter.Common;
using System.Linq;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [SelectionBase]
    [AddComponentMenu("Spider.SpiderImpl")]
    public class SpiderImpl : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private string playerName;
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private TeamColor teamColor;
        public TeamColor TeamColor => teamColor;

        [Command(requiresAuthority = false)]
        public void CmdSetTeamColor(TeamColor teamColor)
        {
            this.teamColor = teamColor;
            Color color = teamColor == TeamColor.Red ? Color.red : Color.blue;
            foreach (Transform item in transform)
            {
                if (item.TryGetComponent(out MeshRenderer meshRenderer))
                {
                    meshRenderer.material.color = color;
                }
            }
        }

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
