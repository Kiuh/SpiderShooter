using Common;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyPlayer : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        private TMP_Text playerName;

        [SerializeField]
        private TMP_Text itsYou;

        [SerializeField]
        private Button subButton;

        [SerializeField]
        private TMP_Text subButtonText;

        [SerializeField]
        [InspectorReadOnly]
        private NetworkRoomPlayerExt playerModel;

        public void SetLobbyPlayerModel(NetworkRoomPlayerExt roomPlayerExt)
        {
            playerModel = roomPlayerExt;
        }

        private void Update()
        {
            if (playerModel == null)
            {
                Destroy(gameObject);
            }
            else
            {
                playerName.text = $"Player Name: {playerModel.index}";

                if (playerModel.isServer) // I am HOST
                {
                    if (playerModel.isLocalPlayer)
                    {
                        itsYou.gameObject.SetActive(true);
                        toggle.gameObject.SetActive(false);
                        subButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        itsYou.gameObject.SetActive(false);
                        toggle.interactable = false;
                        toggle.isOn = playerModel.readyToBegin;

                        subButton.gameObject.SetActive(true);
                        subButtonText.text = "Delete Player";

                        subButton.onClick.AddListener(playerModel.connectionToClient.Disconnect);
                    }
                }
                else // I am CLIENT
                {
                    if (playerModel.isLocalPlayer)
                    {
                        itsYou.gameObject.SetActive(true);
                        toggle.gameObject.SetActive(true);
                        toggle.onValueChanged.AddListener(
                            playerModel.CmdChangeReadyStateAction.Invoke
                        );

                        subButton.gameObject.SetActive(true);
                        subButtonText.text = "Leave";

                        subButton.onClick.AddListener(playerModel.connectionToServer.Disconnect);
                    }
                    else
                    {
                        itsYou.gameObject.SetActive(false);
                        if (playerModel.isServer)
                        {
                            toggle.isOn = true;
                        }
                        toggle.interactable = false;
                        toggle.isOn = playerModel.readyToBegin;

                        subButton.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
