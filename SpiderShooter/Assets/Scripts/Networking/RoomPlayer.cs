using Mirror;
using SpiderShooter.Common;
using SpiderShooter.General;
using SpiderShooter.LobbyScene;
using UnityEngine;

namespace SpiderShooter.Networking
{
    public struct NullableTeamColor
    {
        public TeamColor Value;
        public bool IsNotNull;
    }

    [AddComponentMenu("Networking.RoomPlayer")]
    public class RoomPlayer : NetworkRoomPlayer
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
            teamColor.Value = RoomManager.Singleton.GetFreeTeamColor();
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
            Controller.Singleton.SetLobbyMode(oldValue, newValue);
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
            Controller.Singleton.SetLobbyName(oldValue, newValue);
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
                CmdSetPlayerName(LocalClientData.Singleton.PlayerName);
                CmdServerChooseTeamColor();
            }
            Controller.Singleton.CreateLobbyPlayer(this);
        }
    }
}
