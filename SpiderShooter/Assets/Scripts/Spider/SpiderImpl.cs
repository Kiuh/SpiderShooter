using FIMSpace.Basics;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using System.Linq;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [SelectionBase]
    [AddComponentMenu("SpiderShooter/Spider.SpiderImpl")]
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
        private float bulletDamage;
        public float BulletDamage => bulletDamage;

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                FBasic_TPPCameraBehaviour virtualCamera =
                    FindObjectsOfType<FBasic_TPPCameraBehaviour>()
                        .First(x => x.gameObject.CompareTag("MainCamera"));
                virtualCamera.enabled = true;
                virtualCamera.ToFollow = transform;
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
            if (other.TryGetComponent(out Bullet bullet) && bullet.FriendlyTeam != TeamColor)
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

        [Command(requiresAuthority = false)]
        public void CmdKilled()
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
            Transform transform = RoomManager.Singleton.GetRandomStartPosition(TeamColor).transform;
            TeleportToPosition(transform.position, transform.rotation);
            RpcAddTeamKill(TeamColor);
        }
    }
}
