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

        [SyncVar]
        [SerializeField]
        private float health;
        public float Health => health;

        [SerializeField]
        private float bulletDamage;
        public float BulletDamage => bulletDamage;

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private int killCount = 0;
        public int KillCount
        {
            get => killCount;
            set => killCount = value;
        }

        [SerializeField]
        private Movement movement;

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                FBasic_TPPCameraBehaviour virtualCamera =
                    FindObjectsOfType<FBasic_TPPCameraBehaviour>()
                        .First(x => x.gameObject.CompareTag("MainCamera"));
                virtualCamera.enabled = true;
                virtualCamera.ToFollow = transform;
                GameScene.Controller.Singleton.SetLocalPlayer(this);
            }
            ApplyTeamColor();
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

        [ClientRpc]
        private void TeleportToPosition(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            movement.ResetForces();
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet) && bullet.FriendlyTeam != TeamColor)
            {
                health -= bullet.Damage;
                if (health == 0)
                {
                    RoomManager.Singleton.AddKillToPlayer(bullet.OwnerNetId);
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
                    ServerStorage.Singleton.AddTeamKill(TeamColor);
                    CheckForWin();
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdKilled()
        {
            health = 100;
            Transform transform = RoomManager.Singleton.GetRandomStartPosition(TeamColor).transform;
            TeleportToPosition(transform.position, transform.rotation);
            ServerStorage.Singleton.AddTeamKill(TeamColor);
            CheckForWin();
        }

        [Command(requiresAuthority = false)]
        public void CheckForWin()
        {
            if (
                ServerStorage.Singleton.RedTeamKillCount >= ServerStorage.Singleton.KillsToWin
                || ServerStorage.Singleton.BlueTeamKillCount >= ServerStorage.Singleton.KillsToWin
            )
            {
                ServerStorage.Singleton.GameEnds = true;
                GameScene.Controller.Singleton.RpcShowWinPanel();
            }
        }
    }
}
