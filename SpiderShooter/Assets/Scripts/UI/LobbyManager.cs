using Mirror;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private LobbyPlayer lobbyPlayerView;

        [SerializeField]
        private Button playButton;

        [SerializeField]
        private Button quitButton;

        [SerializeField]
        private TMP_Text whoIAM;

        [SerializeField]
        private TMP_Text lobbyTitle;

        public static LobbyManager Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            if (!NetworkServer.active)
            {
                playButton.gameObject.SetActive(false);
                quitButton.gameObject.SetActive(false);
            }
            if (NetworkServer.active)
            {
                whoIAM.text =
                    $"You are client and server. Lobby code: {NetworkRoomManagerExt.Singleton.RoomCode}";
            }
        }

        private string lobbyName;
        private string lobbyMode;

        public void SetLobbyName(string oldValue, string newValue)
        {
            lobbyName = newValue;
            SetLobbyTitle(lobbyName, lobbyMode);
        }

        public void SetLobbyMode(LobbyMode oldValue, LobbyMode newValue)
        {
            lobbyMode = newValue == LobbyMode.Private ? "Private" : "Public";
            SetLobbyTitle(lobbyName, lobbyMode);
        }

        public void SetLobbyTitle(string lobbyName, string lobbyMode)
        {
            lobbyTitle.text = "Lobby: " + lobbyName + " - mode: " + lobbyMode;
        }

        public void CreateLobbyPlayerView(NetworkRoomPlayerExt roomPlayerExt)
        {
            LobbyPlayer lobbyPlayer = Instantiate(lobbyPlayerView, container);
            lobbyPlayer.SetLobbyPlayerModel(roomPlayerExt);
        }

        // Called By Button Play
        public void Play()
        {
            NetworkRoomManagerExt.Singleton.PlayGameplayScene();
        }

        // Called By Button Quit
        public void Quit()
        {
            NetworkRoomManagerExt.Singleton.StopHost();
        }
    }
}
