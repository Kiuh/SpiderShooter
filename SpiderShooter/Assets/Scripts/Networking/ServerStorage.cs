using Mirror;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Networking
{
    [AddComponentMenu("SpiderShooter/Networking.ServerStorage")]
    public class ServerStorage : NetworkBehaviour
    {
        public static ServerStorage Singleton { get; private set; }

        public void Initialize()
        {
            if (Singleton != null)
            {
                Destroy(Singleton.gameObject);
            }
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private string lobbyCode = "0.0.0.0";
        public string LobbyCode
        {
            get => lobbyCode;
            set => lobbyCode = value;
        }

        [SyncVar]
        [SerializeField]
        private string redTeamName = "Red Team";
        public string RedTeamName
        {
            get => redTeamName;
            set => redTeamName = value;
        }

        [SyncVar]
        [SerializeField]
        private string blueTeamName = "Blue Team";
        public string BlueTeamName
        {
            get => blueTeamName;
            set => blueTeamName = value;
        }

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private int redTeamKillCount = 0;
        public int RedTeamKillCount
        {
            get => redTeamKillCount;
            set => redTeamKillCount = value;
        }

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private int blueTeamKillCount = 0;
        public int BlueTeamKillCount
        {
            get => blueTeamKillCount;
            set => blueTeamKillCount = value;
        }

        [SyncVar]
        [SerializeField]
        private int killsToWin = 25;
        public int KillsToWin
        {
            get => killsToWin;
            set => killsToWin = value;
        }

        [SyncVar]
        [SerializeField]
        private bool gameEnds = false;
        public bool GameEnds
        {
            get => gameEnds;
            set => gameEnds = value;
        }

        public void AddTeamKill(TeamColor team)
        {
            if (team == TeamColor.Blue)
            {
                RedTeamKillCount++;
            }
            else
            {
                BlueTeamKillCount++;
            }
        }
    }
}
