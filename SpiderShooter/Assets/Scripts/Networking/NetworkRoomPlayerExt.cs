using Assets.Scripts;
using Common;
using Mirror;
using SpiderShooter.Common;
using UI;
using UnityEngine;

namespace Networking
{
    public enum TeamColor
    {
        Blue,
        Red
    }

    public struct NullableTeamColor
    {
        public TeamColor Value;
        public bool IsNotNull;
    }

    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        [Header("Custom Properties")]
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
        public void CmdSetPlayerName(string newValue)
        {
            playerName = newValue;
        }

        [SyncVar]
        [SerializeField]
        [InspectorReadOnly]
        private NullableTeamColor teamColor;
        public NullableTeamColor TeamColor
        {
            get => teamColor;
            set => teamColor = value;
        }

        [Command]
        public void CmdSetTeamColor(TeamColor newValue)
        {
            teamColor.Value = newValue;
            teamColor.IsNotNull = true;
        }

        [Command]
        public void CmdServerChooseTeamColor()
        {
            teamColor.Value = NetworkRoomManagerExt.Singleton.GetFreeTeamColor();
            teamColor.IsNotNull = true;
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
                CmdSetPlayerName(LocalGlobalData.Singleton.PlayerName);
                CmdServerChooseTeamColor();
            }
            LobbyManager.Singleton.CreateLobbyPlayerView(this);
        }
    }
}