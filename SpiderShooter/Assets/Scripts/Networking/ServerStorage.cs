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
        private LobbyMode lobbyMode;
        public LobbyMode LobbyMode
        {
            get => lobbyMode;
            set => lobbyMode = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private string lobbyName;
        public string LobbyName
        {
            get => lobbyName;
            set => lobbyName = value;
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
