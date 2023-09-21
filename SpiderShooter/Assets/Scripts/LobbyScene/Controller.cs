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

        public static Controller Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        public override void OnStartClient()
        {
            if (!isServer)
            {
                startGameButton.gameObject.SetActive(false);
            }
        }

        [Command(requiresAuthority = false)]
        public void SetLobbyCode()
        {
            lobbyCode.text = ServerStorage.Singleton.LobbyCode;
        }

        public void CreateLobbyPlayer(RoomPlayer roomPlayer)
        {
            GameObject lobbyPlayer = Instantiate(lobbyPlayerPrefab);
            RoomPlayerView player = lobbyPlayer.GetComponent<RoomPlayerView>();
            player.SetContainersByTeam(redTeamContainer, blueTeamContainer);
            player.SetNetworkRoomPlayer(roomPlayer);
        }

        // Called by button
        public void StartGame()
        {
            Result result = RoomManager.Singleton.PlayGameplayScene();
            if (result.Failure)
            {
                Debug.Log(result.Error);
            }
        }
    }
}
