using Common;
using Mirror;
using UI;
using UnityEngine;

namespace Networking
{
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
    }
}
