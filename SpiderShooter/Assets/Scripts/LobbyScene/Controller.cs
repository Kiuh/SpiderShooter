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
        private RectTransform initContainer;

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
            Visual.OnHostPlayTrigger += RoomManager.Singleton.PlayGameplayScene;
            Visual.OnHostQuitTrigger += RoomManager.Singleton.StopHost;
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
            Visual.SetPlayTriggerMode(
                NetworkServer.active && newValue == LobbyMode.Private
                    ? VisualElementMode.Interactable
                    : VisualElementMode.Hidden
            );
            Visual.SetQuitTriggerMode(
                NetworkServer.active && newValue == LobbyMode.Private
                    ? VisualElementMode.Interactable
                    : VisualElementMode.Hidden
            );
            Visual.SetLobbyMode(newValue);
        }

        public void CreateLobbyPlayer(RoomPlayer roomPlayer)
        {
            GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab, initContainer);
            lobbyPlayer.GetComponent<Player.Controller>().SetNetworkRoomPlayer(roomPlayer);
        }
    }
}
