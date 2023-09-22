using FIMSpace.Basics;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using SpiderShooter.Spider;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("SpiderShooter/GameScene.Controller")]
    public class Controller : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text teamsKillsLabel;

        [SerializeField]
        private TMP_Text playerInfoLabel;

        [SerializeField]
        private GameObject winPanel;

        [SerializeField]
        private TMP_Text titleLabel;

        [SerializeField]
        private TMP_Text redTeamStatistic;

        [SerializeField]
        private TMP_Text blueTeamStatistic;

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
            teamsKillsLabel.text =
                $"Kills to win: {RoomPlayer.Singleton.KillsToWin}\n"
                + $"{RoomPlayer.Singleton.RedTeamName} - {RoomPlayer.Singleton.RedTeamKillCount}\n"
                + $"{RoomPlayer.Singleton.BlueTeamName} - {RoomPlayer.Singleton.BlueTeamKillCount}";

            if (localPlayer != null)
            {
                playerInfoLabel.text =
                    $"{(localPlayer.TeamColor == TeamColor.Red ? RoomPlayer.Singleton.RedTeamName : RoomPlayer.Singleton.BlueTeamName)}"
                    + $"\n{localPlayer.PlayerName}"
                    + $"\n{localPlayer.KillCount} - your score";
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        [ClientRpc]
        public void RpcShowWinPanel()
        {
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

            if (redTeamPlayers.Count() > 0)
            {
                StringBuilder stringBuilder = new("");
                _ = stringBuilder.AppendLine(
                    $"{storage.RedTeamName} - total kills {storage.RedTeamKillCount}"
                );
                SpiderImpl bestPlayer = redTeamPlayers.OrderBy(x => x.KillCount).First();
                _ = stringBuilder.AppendLine($"Best Player - {bestPlayer.PlayerName}");
                _ = stringBuilder.AppendLine($"{bestPlayer.KillCount} kills");
                _ = stringBuilder.AppendLine();
                int playersKillCount = 0;
                foreach (SpiderImpl player in redTeamPlayers)
                {
                    playersKillCount += player.KillCount;
                    _ = stringBuilder.AppendLine($"{player.PlayerName} - {player.KillCount} kills");
                }
                _ = stringBuilder.AppendLine(
                    $"Suicide: {storage.BlueTeamKillCount - playersKillCount}"
                );
                redTeamStatistic.text = stringBuilder.ToString();
            }
            else
            {
                redTeamStatistic.text = "No players in team.";
            }

            if (blueTeamPlayers.Count() > 0)
            {
                StringBuilder stringBuilder = new("");
                _ = stringBuilder.AppendLine(storage.BlueTeamName);
                SpiderImpl bestPlayer = blueTeamPlayers.OrderBy(x => x.KillCount).First();
                _ = stringBuilder.AppendLine($"Best Player - {bestPlayer.PlayerName}");
                _ = stringBuilder.AppendLine($"{bestPlayer.KillCount} kills");
                _ = stringBuilder.AppendLine();
                int playersKillCount = 0;
                foreach (SpiderImpl player in blueTeamPlayers)
                {
                    playersKillCount += player.KillCount;
                    _ = stringBuilder.AppendLine($"{player.PlayerName} - {player.KillCount} kills");
                }
                _ = stringBuilder.AppendLine(
                    $"Suicide: {storage.RedTeamKillCount - playersKillCount}"
                );
                blueTeamStatistic.text = stringBuilder.ToString();
            }
            else
            {
                blueTeamStatistic.text = "No players in team.";
            }
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
