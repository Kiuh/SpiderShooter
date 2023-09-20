using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Spider;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpiderShooter.Networking
{
    [AddComponentMenu("SpiderShooter/Networking.RoomManager")]
    public class RoomManager : NetworkRoomManager
    {
        public static RoomManager Singleton { get; private set; }

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
            IEnumerable<RoomPlayer> extRoomsSlots = roomSlots.Cast<RoomPlayer>();
            int reds = extRoomsSlots.Count(
                x => x.TeamColor.Value == TeamColor.Red && x.TeamColor.IsNotNull
            );
            int blues = extRoomsSlots.Count(
                x => x.TeamColor.Value == TeamColor.Blue && x.TeamColor.IsNotNull
            );

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

        public StartPosition GetRandomStartPosition(TeamColor teamColor)
        {
            IEnumerable<StartPosition> list = startPositions
                .Select(x => x.GetComponent<StartPosition>())
                .Where(x => x.TeamColor == teamColor);
            return list.Skip(Random.Range(0, list.Count())).First();
        }

        public override void OnRoomServerPlayersReady()
        {
            if (
                ServerStorage.Singleton.LobbyMode == LobbyMode.Public
                && minimumPlayersToPlayInPublic <= roomSlots.Count
            )
            {
                PlayGameplayScene();
            }
        }

        public void PlayGameplayScene()
        {
            //if (!roomSlots.All(x => x.readyToBegin))
            //{
            //    Debug.Log("Not all players Ready.");
            //    return;
            //}
            //IEnumerable<RoomPlayer> extRoomsSlots = roomSlots.Cast<RoomPlayer>();
            //if (!extRoomsSlots.Any(x => x.TeamColor.Value == TeamColor.Blue))
            //{
            //    Debug.Log("At Lest 1 must be in blue team.");
            //    return;
            //}
            //if (!extRoomsSlots.Any(x => x.TeamColor.Value == TeamColor.Red))
            //{
            //    Debug.Log("At Lest 1 must be in red team.");
            //    return;
            //}
            //if (extRoomsSlots.Count(x => x.TeamColor.Value == TeamColor.Red) > maxRedTeamCount)
            //{
            //    Debug.Log($"Max players in red team - {maxRedTeamCount}.");
            //    return;
            //}
            //if (extRoomsSlots.Count(x => x.TeamColor.Value == TeamColor.Blue) > maxBlueTeamCount)
            //{
            //    Debug.Log($"Max players in blue team - {maxBlueTeamCount}.");
            //    return;
            //}
            ServerChangeScene(GameplayScene);
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            GameObject newRoomPlayer = Instantiate(
                roomPlayerPrefab.gameObject,
                Vector3.zero,
                Quaternion.identity
            );
            return newRoomPlayer;
        }

        public override GameObject OnRoomServerCreateGamePlayer(
            NetworkConnectionToClient conn,
            GameObject roomPlayer
        )
        {
            IEnumerable<StartPosition> netStartPositions = startPositions.Select(
                x => x.GetComponent<StartPosition>()
            );

            RoomPlayer roomPlayerExt = roomPlayer.GetComponent<RoomPlayer>();

            StartPosition startPosition = netStartPositions
                .Where(x => x.TeamColor == roomPlayerExt.TeamColor.Value && !x.IsBusy)
                .First();

            GameObject spiderGameObject = Instantiate(
                playerPrefab,
                startPosition.transform.position,
                startPosition.transform.rotation
            );

            startPosition.IsBusy = true;

            return spiderGameObject;
        }

        public override bool OnRoomServerSceneLoadedForPlayer(
            NetworkConnectionToClient conn,
            GameObject roomPlayer,
            GameObject gamePlayer
        )
        {
            RoomPlayer roomPlayerExt = roomPlayer.GetComponent<RoomPlayer>();
            SpiderImpl spider = gamePlayer.GetComponent<SpiderImpl>();

            spider.TeamColor = roomPlayerExt.TeamColor.Value;
            spider.PlayerName = roomPlayerExt.PlayerName;

            return true;
        }
    }
}
