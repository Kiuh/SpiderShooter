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

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private int deathCount = 0;
        public int DeathCount
        {
            get => deathCount;
            set => deathCount = value;
        }

        [SerializeField]
        private Material redTeamMaterial;

        [SerializeField]
        private Material blueTeamMaterial;

        [SerializeField]
        private SkinnedMeshRenderer meshRenderer;

        [SerializeField]
        private Movement movement;

        [SerializeField]
        private Animator animator;

        private SpiderAnimator spiderAnimator;

        private void Awake()
        {
            spiderAnimator = new SpiderAnimator(movement, animator);
            movement.SetSpiderAnimator(spiderAnimator);
        }

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
            meshRenderer.materials[1] =
                teamColor == TeamColor.Red ? redTeamMaterial : blueTeamMaterial;
        }

        [ClientRpc]
        private void TeleportToPosition(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
            movement.ResetForces();
        }

        [ServerCallback]
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Death Zone"))
            {
                CmdKilled();
            }
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Bullet bullet))
            {
                if (bullet.FriendlyTeam != TeamColor)
                {
                    health -= bullet.Damage;
                    if (health == 0)
                    {
                        CmdKilled();
                        //RoomManager.Singleton.AddKillToPlayer(bullet.OwnerNetId);
                        //health = 100;
                        //Transform transform = RoomManager.Singleton
                        //    .GetRandomStartPosition(TeamColor)
                        //    .transform;
                        //TeleportToPosition(transform.position, transform.rotation);
                        //RoomPlayer.Singleton.AddTeamKill(TeamColor);
                        //CmdCheckForWin();
                    }
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdKilled()
        {
            health = 100;
            deathCount++;
            Transform transform = RoomManager.Singleton.GetRandomStartPosition(TeamColor).transform;
            TeleportToPosition(transform.position, transform.rotation);
            RoomPlayer.Singleton.AddTeamKill(TeamColor);
            CmdCheckForWin();
        }

        [Command(requiresAuthority = false)]
        public void CmdCheckForWin()
        {
            if (
                RoomPlayer.Singleton.RedTeamKillCount >= RoomPlayer.Singleton.KillsToWin
                || RoomPlayer.Singleton.BlueTeamKillCount >= RoomPlayer.Singleton.KillsToWin
            )
            {
                RoomPlayer.Singleton.GameEnds = true;
                GameScene.Controller.Singleton.RpcShowWinPanel();
            }
        }
    }
}
