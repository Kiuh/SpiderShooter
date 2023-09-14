using Mirror;
using System;
using UnityEngine;

namespace Networking
{
    public class NetworkRoomManagerExt : NetworkRoomManager
    {
        public static NetworkRoomManagerExt Singleton { get; private set; }

        /// <summary>
        /// Runs on both Server and Client
        /// Networking is NOT initialized when this fires
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            Singleton = this;
        }

        /// <summary>
        /// This is called on the server when a networked scene finishes loading.
        /// </summary>
        /// <param name="sceneName">Name of the new scene.</param>
        public override void OnRoomServerSceneChanged(string sceneName)
        {
            // spawn the initial batch of Rewards
            if (sceneName == GameplayScene)
            {
                // Do something
            }
        }

        /// <summary>
        /// Called just after GamePlayer object is instantiated and just before it replaces RoomPlayer object.
        /// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
        /// into the GamePlayer object as it is about to enter the Online scene.
        /// </summary>
        /// <param name="roomPlayer"></param>
        /// <param name="gamePlayer"></param>
        /// <returns>true unless some code in here decides it needs to abort the replacement</returns>
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
