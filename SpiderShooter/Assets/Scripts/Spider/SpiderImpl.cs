using FIMSpace.Basics;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using System.Collections;
using System.Collections.Generic;
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
        private string uid;
        public string Uid => uid;

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
        private Material standardMaterial;

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
                uid = new System.Guid().ToString();
                FBasic_TPPCameraBehaviour virtualCamera =
                    FindObjectsOfType<FBasic_TPPCameraBehaviour>()
                        .First(x => x.gameObject.CompareTag("MainCamera"));
                virtualCamera.enabled = true;
                virtualCamera.ToFollow = transform;
                GameScene.Controller.Singleton.SetLocalPlayer(this);
            }
            ApplyTeamColor();
        }

        [Command(requiresAuthority = false)]
        public void ApplyTeamColor()
        {
            RpcTeamColor();
        }

        [ClientRpc]
        public void RpcTeamColor()
        {
            meshRenderer.materials = new Material[2]
            {
                standardMaterial,
                teamColor == TeamColor.Red ? redTeamMaterial : blueTeamMaterial
            };
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
            if (DeathLock)
            {
                return;
            }

            if (collision.collider.CompareTag("Death Zone"))
            {
                CmdKilled();
            }
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (DeathLock)
            {
                return;
            }

            if (other.TryGetComponent(out Bullet bullet))
            {
                if (bullet.FriendlyTeam != TeamColor)
                {
                    health -= bullet.Damage;
                    if (health == 0)
                    {
                        List<GameObject> playersObjects = GameObject
                            .FindGameObjectsWithTag("Player")
                            .ToList();
                        SpiderImpl player = playersObjects
                            .Select(x => x.GetComponent<SpiderImpl>())
                            .First(x => x.Uid == bullet.OwnerNetId);
                        player.killCount++;

                        CmdKilled();
                    }
                }
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdKilled()
        {
            deathCount++;
            RoomPlayer.Singleton.AddTeamKill(TeamColor);
            CmdCheckForWin();

            RpcPlayDeath();
            _ = StartCoroutine(WaitEndDeath(2));
        }

        private IEnumerator WaitEndDeath(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            health = 100;
            Transform transform = RoomManager.Singleton.GetRandomStartPosition(TeamColor).transform;
            TeleportToPosition(transform.position, transform.rotation);
            RpcContinueRegular();
        }

        public bool DeathLock = false;

        [ClientRpc]
        public void RpcPlayDeath()
        {
            DeathLock = true;
            spiderAnimator.PlayDeath();
        }

        [ClientRpc]
        public void RpcContinueRegular()
        {
            DeathLock = false;
            spiderAnimator.ContinueRegular();
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
