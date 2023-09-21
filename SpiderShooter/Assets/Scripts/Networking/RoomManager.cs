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

        public bool IsFullLobby => roomSlots.Count >= maxBlueTeamCount + maxRedTeamCount;

        [Header("Custom Properties")]
        [Range(1, 4)]
        [SerializeField]
        private int maxBlueTeamCount;

        [Range(1, 4)]
        [SerializeField]
        private int maxRedTeamCount;

        [Range(1, 8)]
        [SerializeField]
        private int minimumPlayersToPlayInPublic;

        public override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        public TeamColor GetNextTeamColor()
        {
            IEnumerable<RoomPlayer> extRoomsSlots = roomSlots.Cast<RoomPlayer>();
            int reds = extRoomsSlots.Count(x => x.TeamColor == TeamColor.Red);
            int blues = extRoomsSlots.Count(x => x.TeamColor == TeamColor.Blue);
            return reds > blues ? TeamColor.Blue : TeamColor.Red;
        }

        public StartPosition GetRandomStartPosition(TeamColor teamColor)
        {
            IEnumerable<StartPosition> list = startPositions
                .Select(x => x.GetComponent<StartPosition>())
                .Where(x => x.TeamColor == teamColor);
            return list.Skip(Random.Range(0, list.Count())).First();
        }

        public override void OnRoomServerPlayersReady() { }

        public void AddKillToPlayer(uint netId)
        {
            IEnumerable<SpiderImpl> players = GameObject
                .FindGameObjectsWithTag("Player")
                .ToList()
                .Cast<SpiderImpl>();

            foreach (SpiderImpl player in players)
            {
                if (player.netIdentity.netId == netId)
                {
                    player.KillCount++;
                }
            }
        }

        public Result PlayGameplayScene()
        {
            ServerChangeScene(GameplayScene);
            return new SuccessResult();

            if (!roomSlots.All(x => x.readyToBegin))
            {
                return new FailResult("Not all players Ready.");
            }
            IEnumerable<RoomPlayer> extRoomsSlots = roomSlots.Cast<RoomPlayer>();
            if (!extRoomsSlots.Any(x => x.TeamColor == TeamColor.Blue))
            {
                return new FailResult("At Lest 1 must be in blue team.");
            }
            if (!extRoomsSlots.Any(x => x.TeamColor == TeamColor.Red))
            {
                return new FailResult("At Lest 1 must be in red team.");
            }
            if (extRoomsSlots.Count(x => x.TeamColor == TeamColor.Red) > maxRedTeamCount)
            {
                return new FailResult($"Max players in red team - {maxRedTeamCount}.");
            }
            if (extRoomsSlots.Count(x => x.TeamColor == TeamColor.Blue) > maxBlueTeamCount)
            {
                return new FailResult($"Max players in blue team - {maxBlueTeamCount}.");
            }
            ServerChangeScene(GameplayScene);
            return new SuccessResult();
        }

        public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnectionToClient conn)
        {
            GameObject newRoomPlayer = Instantiate(
                roomPlayerPrefab.gameObject,
                Vector3.zero,
                Quaternion.identity
            );
            RoomPlayer roomPlayer = newRoomPlayer.GetComponent<RoomPlayer>();
            roomPlayer.TeamColor = GetNextTeamColor();
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
                .Where(x => x.TeamColor == roomPlayerExt.TeamColor && !x.IsBusy)
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

            spider.TeamColor = roomPlayerExt.TeamColor;
            spider.PlayerName = roomPlayerExt.PlayerName;

            return true;
        }
    }
}
