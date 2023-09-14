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

        public static LobbyManager Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
            if (!NetworkServer.active)
            {
                playButton.gameObject.SetActive(false);
                quitButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            playButton.gameObject.SetActive(NetworkServer.active);
            if (NetworkServer.active)
            {
                whoIAM.text = "You are client and server.";
            }
        }

        // Called By Button Quit
        public void Quit()
        {
            NetworkRoomManagerExt.Singleton.StopHost();
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
    }
}
