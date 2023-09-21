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
                $"Kills to win: {ServerStorage.Singleton.KillsToWin}\n"
                + $"{ServerStorage.Singleton.RedTeamName} - {ServerStorage.Singleton.RedTeamKillCount}\n"
                + $"{ServerStorage.Singleton.BlueTeamName} - {ServerStorage.Singleton.BlueTeamKillCount}";

            if (localPlayer != null)
            {
                playerInfoLabel.text =
                    $"{(localPlayer.TeamColor == TeamColor.Red ? ServerStorage.Singleton.RedTeamName : ServerStorage.Singleton.BlueTeamName)}"
                    + $"\n{localPlayer.PlayerName}"
                    + $"\n{localPlayer.KillCount} - your score";
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        [ClientRpc(includeOwner = true)]
        public void RpcShowWinPanel()
        {
            winPanel.SetActive(true);

            Time.timeScale = 0;
            ServerStorage storage = ServerStorage.Singleton;
            TeamColor winTeamColor =
                storage.BlueTeamKillCount > storage.RedTeamKillCount
                    ? TeamColor.Blue
                    : TeamColor.Red;
            titleLabel.text =
                $"{(winTeamColor == TeamColor.Red ? storage.RedTeamName : storage.BlueTeamName)} Wins";

            IEnumerable<SpiderImpl> players = GameObject
                .FindGameObjectsWithTag("Player")
                .ToList()
                .Cast<SpiderImpl>();

            IEnumerable<SpiderImpl> redTeamPlayers = players.Where(
                x => x.TeamColor == TeamColor.Red
            );
            IEnumerable<SpiderImpl> blueTeamPlayers = players.Where(
                x => x.TeamColor == TeamColor.Blue
            );

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
            _ = stringBuilder.AppendLine($"Suicide: {storage.RedTeamKillCount - playersKillCount}");
            redTeamStatistic.text = stringBuilder.ToString();

            stringBuilder = new("");
            _ = stringBuilder.AppendLine(storage.BlueTeamName);
            bestPlayer = blueTeamPlayers.OrderBy(x => x.KillCount).First();
            _ = stringBuilder.AppendLine($"Best Player - {bestPlayer.PlayerName}");
            _ = stringBuilder.AppendLine($"{bestPlayer.KillCount} kills");
            _ = stringBuilder.AppendLine();
            playersKillCount = 0;
            foreach (SpiderImpl player in blueTeamPlayers)
            {
                playersKillCount += player.KillCount;
                _ = stringBuilder.AppendLine($"{player.PlayerName} - {player.KillCount} kills");
            }
            _ = stringBuilder.AppendLine(
                $"Suicide: {storage.BlueTeamKillCount - playersKillCount}"
            );
            blueTeamStatistic.text = stringBuilder.ToString();
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
