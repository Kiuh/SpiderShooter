using Cinemachine;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
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
        public TeamColor TeamColor
        {
            get => teamColor;
            set => teamColor = value;
        }

        public void ApplyTeamColor()
        {
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
        public float Health => health;

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
            ApplyTeamColor();
        }

        [ClientRpc]
        private void TeleportToPosition(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        [ClientRpc]
        private void RpcAddTeamKill(TeamColor team)
        {
            if (team == TeamColor.Blue)
            {
                GameScene.Controller.Singleton.RedTeamKillCount++;
            }
            else
            {
                GameScene.Controller.Singleton.BlueTeamKillCount++;
            }
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                health -= bullet.Damage;
                if (health == 0)
                {
                    if (TeamColor == TeamColor.Red)
                    {
                        ServerStorage.Singleton.BlueTeamKillCount++;
                    }
                    else
                    {
                        ServerStorage.Singleton.RedTeamKillCount++;
                    }
                    health = 100;
                    Transform transform = RoomManager.Singleton
                        .GetRandomStartPosition(TeamColor)
                        .transform;
                    TeleportToPosition(transform.position, transform.rotation);
                    RpcAddTeamKill(TeamColor);
                }
            }
        }
    }
}
