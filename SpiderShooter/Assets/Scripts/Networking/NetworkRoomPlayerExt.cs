using Mirror;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class NetworkRoomPlayerExt : NetworkRoomPlayer
    {
        public UnityAction<bool> CmdChangeReadyStateAction;

        public NetworkIdentity NetworkIdentity => GetComponent<NetworkIdentity>();

        private void Awake()
        {
            LobbyManager.Singleton.CreateLobbyPlayerView(this);
        }

        public override void OnStartClient()
        {
            Debug.Log($"OnStartClient {gameObject}");
            CmdChangeReadyStateAction = CmdChangeReadyState;
        }

        public override void OnClientEnterRoom()
        {
            Debug.Log($"OnClientEnterRoom {SceneManager.GetActiveScene().path}");
        }

        public override void OnClientExitRoom()
        {
            Debug.Log($"OnClientExitRoom {SceneManager.GetActiveScene().path}");
        }

        public override void IndexChanged(int oldIndex, int newIndex)
        {
            Debug.Log($"IndexChanged {newIndex}");
        }

        public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
        {
            Debug.Log($"ReadyStateChanged {newReadyState}");
        }
    }
}
