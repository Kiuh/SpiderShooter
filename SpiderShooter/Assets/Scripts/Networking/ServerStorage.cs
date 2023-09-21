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
                Destroy(this);
            }
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        [SerializeField]
        [InspectorReadOnly]
        private string lobbyCode = "0.0.0.0";
        public string LobbyCode
        {
            get => lobbyCode;
            set => lobbyCode = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private string redTeamName = "Red Team";
        public string RedTeamName
        {
            get => redTeamName;
            set => redTeamName = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int blueTeamName = 0;
        public int BlueTeamName
        {
            get => blueTeamName;
            set => blueTeamName = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int redTeamKillCount = 0;
        public int RedTeamKillCount
        {
            get => redTeamKillCount;
            set => redTeamKillCount = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int blueTeamKillCount = 0;
        public int BlueTeamKillCount
        {
            get => blueTeamKillCount;
            set => blueTeamKillCount = value;
        }
    }
}
