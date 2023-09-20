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

    public struct NullableLobbyMode
    {
        public LobbyMode Value;
        public bool IsNotNull;
    }

    [AddComponentMenu("SpiderShooter/Networking.RoomPlayer")]
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

        [Command(requiresAuthority = false)]
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

        [Command(requiresAuthority = false)]
        public void CmdSetTeamColor(TeamColor newValue)
        {
            teamColor.Value = newValue;
            teamColor.IsNotNull = true;
        }

        [Command(requiresAuthority = false)]
        public void CmdServerChooseTeamColor()
        {
            teamColor.Value = RoomManager.Singleton.GetFreeTeamColor();
            teamColor.IsNotNull = true;
        }

        [SyncVar(hook = nameof(SetLobbyMode))]
        [SerializeField]
        [InspectorReadOnly]
        private NullableLobbyMode lobbyMode;
        public NullableLobbyMode LobbyMode
        {
            get => lobbyMode;
            set => lobbyMode = value;
        }

        public void SetLobbyMode(NullableLobbyMode oldValue, NullableLobbyMode newValue)
        {
            Controller.Singleton.SetLobbyMode(oldValue.Value, newValue.Value);
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
            Controller.Singleton.CreateLobbyPlayer(this);
            if (NetworkServer.active)
            {
                LobbyName = ServerStorage.Singleton.LobbyName;
                LobbyMode = new() { Value = ServerStorage.Singleton.LobbyMode, IsNotNull = true };
            }
            if (isLocalPlayer)
            {
                CmdSetPlayerName(LocalClientData.Singleton.PlayerName);
                CmdServerChooseTeamColor();
            }
        }
    }
}
