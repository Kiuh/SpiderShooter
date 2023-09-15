using Assets.Scripts.Spider;
using InGameNetworking;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Networking
{
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        public static NetworkRoomManagerExt Singleton { get; private set; }

        public string RoomCode =>
            networkAddress.Split('.').Skip(2).Aggregate((x, y) => x + "-" + y);

        public bool IsFullLobby => roomSlots.Count >= maxBlueTeamCount + maxRedTeamCount;

        [Header("Custom Properties")]
        [Range(1, 4)]
        [SerializeField]
        private int maxBlueTeamCount;

        [Range(1, 4)]
        [SerializeField]
        private int maxRedTeamCount;

        [Range(1, 4)]
        [SerializeField]
        private int minimumPlayersToPlayInPublic;

        public override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        public TeamColor GetFreeTeamColor()
        {
            IEnumerable<NetworkRoomPlayerExt> extRoomsSlots =
                roomSlots.Cast<NetworkRoomPlayerExt>();
            int reds = extRoomsSlots.Count(
                x => x.TeamColor.Value == TeamColor.Red && x.TeamColor.IsNotNull
            );
            int blues = extRoomsSlots.Count(
                x => x.TeamColor.Value == TeamColor.Blue && x.TeamColor.IsNotNull
            );

            Debug.Log($"reds {reds}, blues {blues}");

            return IsFullLobby
                ? throw new System.InvalidOperationException()
                : reds == maxRedTeamCount
                    ? TeamColor.Blue
                    : blues == maxBlueTeamCount
                        ? TeamColor.Red
                        : reds > blues
                            ? TeamColor.Blue
                            : TeamColor.Red;
        }

        public override void OnRoomServerPlayersReady()
        {
            if (
                ServerStorage.Singleton.LobbyMode == UI.LobbyMode.Public
                && minimumPlayersToPlayInPublic <= roomSlots.Count
            )
            {
                PlayGameplayScene();
            }
        }

        public void PlayGameplayScene()
        {
            if (!roomSlots.All(x => x.readyToBegin))
            {
                Debug.Log("Not all players Ready.");
                return;
            }
            IEnumerable<NetworkRoomPlayerExt> extRoomsSlots =
                roomSlots.Cast<NetworkRoomPlayerExt>();
            if (!extRoomsSlots.Any(x => x.TeamColor.Value == TeamColor.Blue))
            {
                Debug.Log("At Lest 1 must be in blue team.");
                return;
            }
            if (!extRoomsSlots.Any(x => x.TeamColor.Value == TeamColor.Red))
            {
                Debug.Log("At Lest 1 must be in red team.");
                return;
            }
            if (extRoomsSlots.Count(x => x.TeamColor.Value == TeamColor.Red) > maxRedTeamCount)
            {
                Debug.Log($"Max players in red team - {maxRedTeamCount}.");
                return;
            }
            if (extRoomsSlots.Count(x => x.TeamColor.Value == TeamColor.Blue) > maxBlueTeamCount)
            {
                Debug.Log($"Max players in blue team - {maxBlueTeamCount}.");
                return;
            }
            ServerChangeScene(GameplayScene);
        }

        public override GameObject OnRoomServerCreateGamePlayer(
            NetworkConnectionToClient conn,
            GameObject roomPlayer
        )
        {
            NetworkRoomPlayerExt roomPlayerExt = roomPlayer.GetComponent<NetworkRoomPlayerExt>();

            IEnumerable<NetworkStartPositionByTeam> netStartPositions = startPositions.Select(
                x => x.GetComponent<NetworkStartPositionByTeam>()
            );

            NetworkStartPositionByTeam startPosition = netStartPositions
                .Where(x => x.TeamColor == roomPlayerExt.TeamColor.Value && !x.IsBusy)
                .First();

            GameObject spiderGameObject = Instantiate(playerPrefab, startPosition.transform);
            SpiderImpl spider = spiderGameObject.GetComponent<SpiderImpl>();

            spider.PlayerName = roomPlayerExt.PlayerName;
            spider.CmdSetTeamColor(roomPlayerExt.TeamColor.Value);

            return spiderGameObject;
        }
    }
}
