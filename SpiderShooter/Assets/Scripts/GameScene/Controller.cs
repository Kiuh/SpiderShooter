using FIMSpace.Basics;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using SpiderShooter.Spider;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("SpiderShooter/GameScene.Controller")]
    public class Controller : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text heathLabel;

        [SerializeField]
        private TMP_Text teamNameLabel;

        [SerializeField]
        private TMP_Text playerNameLabel;

        [SerializeField]
        private TMP_Text vsLabel;

        [SerializeField]
        private TMP_Text killsToWinLabel;

        [SerializeField]
        private TMP_Text killDeathLabel;

        [SerializeField]
        private GameObject winPanel;

        [SerializeField]
        private TMP_Text titleLabel;

        [Header("Red team")]
        [SerializeField]
        private TMP_Text redTeamName;

        [SerializeField]
        private TMP_Text redTeamKills;

        [SerializeField]
        private TMP_Text redTeamBestPlayer;

        [SerializeField]
        private TMP_Text redTeamPlayersList;

        [SerializeField]
        private TMP_Text redTeamSuicides;

        [Header("Blue team")]
        [SerializeField]
        private TMP_Text blueTeamName;

        [SerializeField]
        private TMP_Text blueTeamKills;

        [SerializeField]
        private TMP_Text blueTeamBestPlayer;

        [SerializeField]
        private TMP_Text blueTeamPlayersList;

        [SerializeField]
        private TMP_Text blueTeamSuicides;

        [SerializeField]
        [InspectorReadOnly]
        private SpiderImpl localPlayer = null;

        public static Controller Singleton { get; private set; }

        private void Awake()
        {
            Singleton = this;
        }

        public void SetLocalPlayer(SpiderImpl localPlayer)
        {
            this.localPlayer = localPlayer;
        }

        private void Update()
        {
            killsToWinLabel.text = $"Kills to win: {RoomPlayer.Singleton.KillsToWin}";
            vsLabel.text =
                $"{RoomPlayer.Singleton.RedTeamName} - [{RoomPlayer.Singleton.RedTeamKillCount}] VS "
                + $"[{RoomPlayer.Singleton.BlueTeamKillCount}] - {RoomPlayer.Singleton.BlueTeamName}";

            if (localPlayer != null)
            {
                teamNameLabel.text =
                    $"{(localPlayer.TeamColor == TeamColor.Red ? RoomPlayer.Singleton.RedTeamName : RoomPlayer.Singleton.BlueTeamName)}";
                playerNameLabel.text = $"{localPlayer.PlayerName}";
                killDeathLabel.text = $"{localPlayer.KillCount}/{localPlayer.DeathCount}";
                heathLabel.text = $"HP/{(int)localPlayer.Health}";
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        [ClientRpc]
        public void RpcShowWinPanel()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            winPanel.SetActive(true);

            FindObjectOfType<FBasic_TPPCameraBehaviour>()
                .GetComponent<FBasic_TPPCameraBehaviour>()
                .enabled = false;

            Time.timeScale = 0f;
            RoomPlayer storage = RoomPlayer.Singleton;
            TeamColor winTeamColor =
                storage.BlueTeamKillCount > storage.RedTeamKillCount
                    ? TeamColor.Blue
                    : TeamColor.Red;
            titleLabel.text =
                $"{(winTeamColor == TeamColor.Red ? storage.RedTeamName : storage.BlueTeamName)} Wins";

            List<GameObject> playersObjects = GameObject.FindGameObjectsWithTag("Player").ToList();
            IEnumerable<SpiderImpl> players = playersObjects.Select(
                x => x.GetComponent<SpiderImpl>()
            );

            IEnumerable<SpiderImpl> redTeamPlayers = players.Where(
                x => x.TeamColor == TeamColor.Red
            );
            IEnumerable<SpiderImpl> blueTeamPlayers = players.Where(
                x => x.TeamColor == TeamColor.Blue
            );

            int redPlayersKills = 0;

            if (redTeamPlayers.Count() > 0)
            {
                redTeamName.text = storage.RedTeamName;
                redTeamKills.text = $"{storage.RedTeamKillCount} kills";

                SpiderImpl bestPlayer = redTeamPlayers.OrderBy(x => x.KillCount).First();

                redTeamBestPlayer.text =
                    $"Best Player - {bestPlayer.PlayerName} {bestPlayer.KillCount} kills {bestPlayer.DeathCount} deaths";

                string buffer = "";
                foreach (SpiderImpl player in redTeamPlayers)
                {
                    redPlayersKills += player.KillCount;
                    buffer +=
                        $"{player.PlayerName} - {player.KillCount} kills {player.DeathCount} deaths\n";
                }
                redTeamPlayersList.text = buffer;
            }
            else
            {
                redTeamName.text = "No players in team.";
            }

            int bluePlayersKills = 0;

            if (blueTeamPlayers.Count() > 0)
            {
                blueTeamName.text = storage.BlueTeamName;
                blueTeamKills.text = $"{storage.BlueTeamKillCount} kills";

                SpiderImpl bestPlayer = blueTeamPlayers.OrderBy(x => x.KillCount).First();

                blueTeamBestPlayer.text =
                    $"Best Player - {bestPlayer.PlayerName} {bestPlayer.KillCount} kills {bestPlayer.DeathCount} deaths";

                string buffer = "";
                foreach (SpiderImpl player in blueTeamPlayers)
                {
                    bluePlayersKills += player.KillCount;
                    buffer +=
                        $"{player.PlayerName} - {player.KillCount} kills {player.DeathCount} deaths\n";
                }
                blueTeamPlayersList.text = buffer;
            }
            else
            {
                blueTeamName.text = "No players in team.";
            }

            redTeamSuicides.text = $"Suicide: {storage.BlueTeamKillCount - bluePlayersKills}";
            blueTeamSuicides.text = $"Suicide: {storage.RedTeamKillCount - redPlayersKills}";
        }

        // Called by button
        public void ToMainMenuButtonClick()
        {
            Time.timeScale = 1;
            if (isServer)
            {
                RoomManager.Singleton.StopHost();
            }
            else
            {
                RoomManager.Singleton.StopClient();
            }
        }
    }
}
