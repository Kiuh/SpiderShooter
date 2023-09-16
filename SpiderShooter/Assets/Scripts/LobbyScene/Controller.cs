using AYellowpaper;
using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using UnityEngine;

namespace SpiderShooter.LobbyScene
{
    [AddComponentMenu("LobbyScene.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private InterfaceReference<IView> view;
        private IView Visual => view.Value;

        [SerializeField]
        private GameObject lobbyPlayerPrefab;

        public LobbyMode LobbyMode { get; private set; }

        public static Controller Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            Visual.OnHostPlay += RoomManager.Singleton.PlayGameplayScene;
            Visual.OnHostQuit += RoomManager.Singleton.StopHost;
            if (NetworkServer.active)
            {
                Visual.SetPlayTriggerMode(VisualElementMode.Interactable);
                Visual.SetQuitTriggerMode(VisualElementMode.Interactable);
                Visual.SetLobbyCode(RoomManager.Singleton.RoomCode);
                Visual.SetHostMode();
            }
            else
            {
                Visual.SetPlayTriggerMode(VisualElementMode.Hidden);
                Visual.SetQuitTriggerMode(VisualElementMode.Hidden);
                Visual.SetClientMode();
            }
        }

        public void SetLobbyName(string oldValue, string newValue)
        {
            Visual.SetLobbyName(newValue);
        }

        public void SetLobbyMode(LobbyMode oldValue, LobbyMode newValue)
        {
            LobbyMode = newValue;
            Visual.SetPlayTriggerMode(VisualElementMode.Hidden);
            Visual.SetQuitTriggerMode(VisualElementMode.Hidden);
            Visual.SetLobbyMode(newValue);
        }

        public void CreateLobbyPlayer(RoomPlayer roomPlayer)
        {
            GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            lobbyPlayer.GetComponent<Player.Controller>().SetNetworkRoomPlayer(roomPlayer);
        }
    }
}
