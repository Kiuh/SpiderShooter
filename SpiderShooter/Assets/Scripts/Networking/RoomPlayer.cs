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
        public static RoomPlayer Singleton;

        public void Initialize()
        {
            Singleton = this;
        }

        [SyncVar]
        [SerializeField]
        private bool mainUser = false;

        [SyncVar]
        [SerializeField]
        private string lobbyCode = "----";
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
        private int redTeamKillCount = 0;
        public int RedTeamKillCount
        {
            get => redTeamKillCount;
            set => redTeamKillCount = value;
        }

        [SyncVar]
        [SerializeField]
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
                if (isServer)
                {
                    LobbyCode = LocalClientData.Singleton.BufferIP;
                    redTeamName = "Red Team";
                    blueTeamName = "BlueTeamName";
                    killsToWin = 25;
                    mainUser = true;
                }
                CmdSetPlayerName(LocalClientData.Singleton.PlayerName);
            }
            if (mainUser)
            {
                Initialize();
            }
            Controller.Singleton.CreateLobbyPlayer(this);
        }

        public event Action<bool> OnReadyStateChanged;

        public override void ReadyStateChanged(bool oldValue, bool newValue)
        {
            OnReadyStateChanged?.Invoke(newValue);
        }
    }
}
