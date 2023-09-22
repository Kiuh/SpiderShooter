using Mirror;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiderShooter.LobbyScene
{
    [AddComponentMenu("SpiderShooter/LobbyScene.Controller")]
    public class Controller : NetworkBehaviour
    {
        [SerializeField]
        private RectTransform redTeamContainer;

        [SerializeField]
        private RectTransform blueTeamContainer;

        [SerializeField]
        private GameObject lobbyPlayerPrefab;

        [SerializeField]
        private TMP_Text lobbyCode;

        [SerializeField]
        private Button startGameButton;

        [SerializeField]
        private TMP_InputField redTeamName;

        [SerializeField]
        private TMP_InputField blueTeamName;

        [SerializeField]
        private GameObject errorPanel;

        [SerializeField]
        private TMP_Text errorText;

        public static Controller Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        // Called by input field
        public void RedTeamNameChange(string newName)
        {
            RoomPlayer.Singleton.RedTeamName = newName;
        }

        // Called by input field
        public void BlueTeamNameChange(string newName)
        {
            RoomPlayer.Singleton.BlueTeamName = newName;
        }

        public override void OnStartClient()
        {
            if (!isServer)
            {
                redTeamName.interactable = false;
                blueTeamName.interactable = false;
                startGameButton.gameObject.SetActive(false);
            }
        }

        public void CreateLobbyPlayer(RoomPlayer roomPlayer)
        {
            GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            RoomPlayerView player = lobbyPlayer.GetComponent<RoomPlayerView>();
            player.SetContainersByTeam(redTeamContainer, blueTeamContainer);
            player.SetNetworkRoomPlayer(roomPlayer);
        }

        private void Update()
        {
            if (RoomPlayer.Singleton != null)
            {
                lobbyCode.text = RoomPlayer.Singleton.LobbyCode;
                redTeamName.text = RoomPlayer.Singleton.RedTeamName;
                blueTeamName.text = RoomPlayer.Singleton.BlueTeamName;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        // Called by button
        public void StartGame()
        {
            redTeamName.text = redTeamName.text.Trim();
            blueTeamName.text = blueTeamName.text.Trim();
            if (redTeamName.text.Length <= 3 || blueTeamName.text.Length <= 3)
            {
                ShowError("Team name must be at least 4 characters.");
                return;
            }
            if (redTeamName.text == blueTeamName.text)
            {
                ShowError("Team names must be different.");
                return;
            }
            Result result = RoomManager.Singleton.PlayGameplayScene();
            if (result.Failure)
            {
                ShowError(result.Error);
            }
        }

        private void ShowError(string message)
        {
            errorPanel.gameObject.SetActive(true);
            errorText.text = message;
        }
    }
}
