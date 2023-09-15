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
        private TMP_Dropdown dropdownTeamColor;

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
                playerName.text = $"P.N.: {playerModel.PlayerName}";
                itsYou.gameObject.SetActive(playerModel.isLocalPlayer);

                if (playerModel.isLocalPlayer)
                {
                    // Can change ready state
                    toggle.gameObject.SetActive(true);
                    toggle.onValueChanged.AddListener((x) => playerModel.CmdChangeReadyState(x));

                    // Can change team color
                    if (LobbyManager.Singleton.LobbyMode == LobbyMode.Private)
                    {
                        dropdownTeamColor.onValueChanged.AddListener(
                            (index) => playerModel.CmdSetTeamColor((TeamColor)index)
                        );
                        playerModel.CmdSetTeamColor((TeamColor)dropdownTeamColor.value);
                    }
                    else
                    {
                        // Readonly team color
                        dropdownTeamColor.interactable = false;
                        dropdownTeamColor.value = (int)playerModel.TeamColor.Value;
                    }
                }
                else
                {
                    // Readonly ready state
                    toggle.interactable = false;
                    toggle.isOn = playerModel.readyToBegin;

                    // Readonly team color
                    dropdownTeamColor.interactable = false;
                    dropdownTeamColor.value = (int)playerModel.TeamColor.Value;
                }

                if (playerModel.isServer) // I am HOST
                {
                    if (playerModel.isLocalPlayer)
                    {
                        if (LobbyManager.Singleton.LobbyMode == LobbyMode.Public)
                        {
                            subButton.gameObject.SetActive(true);
                            subButtonText.text = "Leave";
                            subButton.onClick.AddListener(NetworkRoomManagerExt.Singleton.StopHost);
                        }
                        else
                        {
                            subButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (LobbyManager.Singleton.LobbyMode != LobbyMode.Public)
                        {
                            subButton.gameObject.SetActive(true);
                            subButtonText.text = "Delete Player";
                            subButton.onClick.AddListener(
                                playerModel.connectionToClient.Disconnect
                            );
                        }
                        else
                        {
                            subButton.gameObject.SetActive(false);
                        }
                    }
                }
                else // I am CLIENT
                {
                    if (playerModel.isLocalPlayer)
                    {
                        subButton.gameObject.SetActive(true);
                        subButtonText.text = "Leave";
                        subButton.onClick.AddListener(playerModel.connectionToServer.Disconnect);
                    }
                    else
                    {
                        subButton.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
