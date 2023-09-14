using Assets.Scripts;
using Common;
using Mirror;
using UI;
using UnityEngine;

namespace Networking
{
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
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

        [Command]
        public void SetPlayerName(string newValue)
        {
            playerName = newValue;
        }

        [SyncVar(hook = nameof(SetLobbyMode))]
        [SerializeField]
        [InspectorReadOnly]
        private LobbyMode lobbyMode;
        public LobbyMode LobbyMode
        {
            get => lobbyMode;
            set => lobbyMode = value;
        }

        public void SetLobbyMode(LobbyMode oldValue, LobbyMode newValue)
        {
            LobbyManager.Singleton.SetLobbyMode(oldValue, newValue);
        }

        [SyncVar(hook = nameof(SetLobbyName))]
        [SerializeField]
        [InspectorReadOnly]
        private string lobbyName;
        public string LobbyName
        {
            get => lobbyName;
            set => lobbyName = value;
        }

        public void SetLobbyName(string oldValue, string newValue)
        {
            LobbyManager.Singleton.SetLobbyName(oldValue, newValue);
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active)
            {
                LobbyName = ServerStorage.Singleton.LobbyName;
                LobbyMode = ServerStorage.Singleton.LobbyMode;
            }
            if (isLocalPlayer)
            {
                SetPlayerName(LocalGlobalData.Singleton.PlayerName);
            }
            LobbyManager.Singleton.CreateLobbyPlayerView(this);
        }
    }
}
