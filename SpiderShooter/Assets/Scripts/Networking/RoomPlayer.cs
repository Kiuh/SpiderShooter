using Mirror;
using SpiderShooter.Common;
using SpiderShooter.General;
using SpiderShooter.LobbyScene;
using System;
using UnityEngine;

namespace SpiderShooter.Networking
{
    [AddComponentMenu("SpiderShooter/Networking.RoomPlayer")]
    public class RoomPlayer : NetworkRoomPlayer
    {
        [Header("Custom Properties")]
        [SyncVar(hook = nameof(PlayerNameVarChanged))]
        [SerializeField]
        [InspectorReadOnly]
        private string playerName;
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }

        public void PlayerNameVarChanged(string _, string newValue)
        {
            OnPlayerNameChanged?.Invoke(newValue);
        }

        public event Action<string> OnPlayerNameChanged;

        [Command(requiresAuthority = false)]
        public void CmdSetPlayerName(string newValue)
        {
            playerName = newValue;
        }

        [SyncVar(hook = nameof(TeamColorVarChanged))]
        [SerializeField]
        [InspectorReadOnly]
        private TeamColor teamColor;
        public TeamColor TeamColor
        {
            get => teamColor;
            set => teamColor = value;
        }

        public void TeamColorVarChanged(TeamColor _, TeamColor newValue)
        {
            OnTeamColorChanged?.Invoke(newValue);
        }

        public event Action<TeamColor> OnTeamColorChanged;

        [Command(requiresAuthority = false)]
        public void CmdSetTeamColor(TeamColor newValue)
        {
            TeamColor = newValue;
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                CmdSetPlayerName(LocalClientData.Singleton.PlayerName);
            }
            Controller.Singleton.CreateLobbyPlayer(this);
            if (isLocalPlayer)
            {
                Controller.Singleton.SetLobbyCode();
            }
        }

        public event Action<bool> OnReadyStateChanged;

        public override void ReadyStateChanged(bool oldValue, bool newValue)
        {
            OnReadyStateChanged?.Invoke(newValue);
        }
    }
}
