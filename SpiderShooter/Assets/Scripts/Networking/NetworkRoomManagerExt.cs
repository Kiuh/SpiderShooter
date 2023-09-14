using Mirror;
using System;
using System.Linq;
using UnityEngine;

namespace Networking
{
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        public static NetworkRoomManagerExt Singleton { get; private set; }

        public string RoomCode =>
            networkAddress.Split('.').Skip(2).Aggregate((x, y) => x + "-" + y);

        public override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        public override void OnRoomServerSceneChanged(string sceneName)
        {
            if (sceneName == GameplayScene)
            {
                // Do something
            }
        }

        public override bool OnRoomServerSceneLoadedForPlayer(
            NetworkConnectionToClient conn,
            GameObject roomPlayer,
            GameObject gamePlayer
        )
        {
            //PlayerScore playerScore = gamePlayer.GetComponent<PlayerScore>();
            //playerScore.index = roomPlayer.GetComponent<NetworkRoomPlayer>().index;
            return true;
        }

        public event Action<bool> PlayButtonActiveChange;

        public override void OnRoomServerPlayersReady()
        {
            PlayButtonActiveChange?.Invoke(true);
        }

        public void PlayGameplayScene()
        {
            PlayButtonActiveChange?.Invoke(false);
            ServerChangeScene(GameplayScene);
        }
    }
}
