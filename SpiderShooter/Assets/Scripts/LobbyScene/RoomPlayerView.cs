using SpiderShooter.Common;
using SpiderShooter.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiderShooter.LobbyScene
{
    [AddComponentMenu("SpiderShooter/LobbyScene.RoomPlayerView")]
    public class RoomPlayerView : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private RoomPlayer playerModel;

        [SerializeField]
        private Button changeTeamButton;

        [SerializeField]
        private Button leave;

        [SerializeField]
        private TMP_Text leaveButtonText;

        [SerializeField]
        private Button changeReadyState;

        [SerializeField]
        private TMP_Text readyStateText;

        [SerializeField]
        private TMP_Text playerNameText;

        [SerializeField]
        private Image playerImage;

        private RectTransform redTeamContainer;
        private RectTransform blueTeamContainer;

        public void SetContainersByTeam(
            RectTransform redTeamContainer,
            RectTransform blueTeamContainer
        )
        {
            this.redTeamContainer = redTeamContainer;
            this.blueTeamContainer = blueTeamContainer;
        }

        public void SetNetworkRoomPlayer(RoomPlayer roomPlayerExt)
        {
            playerModel = roomPlayerExt;

            SetPlayerName(roomPlayerExt.PlayerName);
            roomPlayerExt.OnPlayerNameChanged += SetPlayerName;

            SetReadyState(roomPlayerExt.readyToBegin);
            roomPlayerExt.OnReadyStateChanged += SetReadyState;

            SetTeamColor(roomPlayerExt.TeamColor);
            roomPlayerExt.OnTeamColorChanged += SetTeamColor;

            if (roomPlayerExt.isLocalPlayer)
            {
                playerImage.color = new Color(121 / 255.0f, 255 / 255.0f, 140 / 255.0f);
            }
            else
            {
                if (roomPlayerExt.isServer)
                {
                    changeReadyState.interactable = false;
                    leaveButtonText.text = "Kick";
                }
                else
                {
                    changeReadyState.interactable = false;
                    changeTeamButton.interactable = false;
                    leave.interactable = false;
                }
            }
        }

        private void SetPlayerName(string playerName)
        {
            playerNameText.text = playerName;
        }

        private void SetReadyState(bool readyState)
        {
            readyStateText.text = readyState ? "READY" : "NOT READY";
        }

        private void SetTeamColor(TeamColor teamColor)
        {
            transform.SetParent(teamColor == TeamColor.Red ? redTeamContainer : blueTeamContainer);
        }

        // Called by button
        public void ChangeReadyStateButtonClick()
        {
            playerModel.CmdChangeReadyState(!playerModel.readyToBegin);
        }

        // Called by button
        public void ChangeTeamColorButtonClick()
        {
            playerModel.CmdSetTeamColor(
                playerModel.TeamColor == TeamColor.Blue ? TeamColor.Red : TeamColor.Blue
            );
        }

        // Called by button
        public void LeaveButtonClick()
        {
            if (playerModel.isServer)
            {
                RoomManager.Singleton.StopHost();
            }
            else
            {
                playerModel.connectionToServer.Disconnect();
            }
        }

        private void Update()
        {
            if (playerModel == null)
            {
                Destroy(gameObject);
            }
        }
    }
}
