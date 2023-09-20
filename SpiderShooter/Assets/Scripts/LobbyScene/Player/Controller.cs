using AYellowpaper;
using SpiderShooter.Common;
using SpiderShooter.Networking;
using UnityEngine;

namespace SpiderShooter.LobbyScene.Player
{
    [AddComponentMenu("SpiderShooter/LobbyScene.Player.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private RoomPlayer playerModel;

        [SerializeField]
        private InterfaceReference<IView> view;
        private IView Visual => view.Value;

        public void SetNetworkRoomPlayer(RoomPlayer roomPlayerExt)
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
                Visual.SetPlayerName(playerModel.PlayerName);
                Visual.SetLocalPlayer(playerModel.isLocalPlayer);
                Visual.SetReadyStateSelectionCallBack((x) => playerModel.CmdChangeReadyState(x));

                if (playerModel.isLocalPlayer)
                {
                    // Can change ready state
                    Visual.SetReadyStateSelectionMode(VisualElementMode.Interactable);

                    // Can change team color
                    if (LobbyScene.Controller.Singleton.LobbyMode == LobbyMode.Private)
                    {
                        Visual.SetTeamColorSelectionCallBack((x) => playerModel.CmdSetTeamColor(x));
                        Visual.SetTeamColor(playerModel.TeamColor.Value);
                    }
                    else
                    {
                        // Readonly team color
                        Visual.SetTeamColorSelectionMode(VisualElementMode.ReadOnly);
                        Visual.SetTeamColor(playerModel.TeamColor.Value);
                    }
                }
                else
                {
                    // Readonly ready state
                    Visual.SetReadyStateSelectionMode(VisualElementMode.ReadOnly);
                    Visual.SetReadyState(playerModel.readyToBegin);

                    // Readonly team color
                    Visual.SetTeamColorSelectionMode(VisualElementMode.ReadOnly);
                    Visual.SetTeamColor(playerModel.TeamColor.Value);
                }

                if (playerModel.isServer) // I am HOST
                {
                    if (playerModel.isLocalPlayer)
                    {
                        if (LobbyScene.Controller.Singleton.LobbyMode == LobbyMode.Public)
                        {
                            Visual.SetLeavePlayerTriggerMode(VisualElementMode.Interactable);
                            Visual.SetLeavePlayerTriggerCallBack(RoomManager.Singleton.StopHost);
                        }
                        else
                        {
                            Visual.SetLeavePlayerTriggerMode(VisualElementMode.Hidden);
                        }
                    }
                    else
                    {
                        if (LobbyScene.Controller.Singleton.LobbyMode != LobbyMode.Public)
                        {
                            Visual.SetDeletePlayerTriggerMode(VisualElementMode.Interactable);
                            Visual.SetDeletePlayerTriggerCallBack(
                                playerModel.connectionToClient.Disconnect
                            );
                        }
                        else
                        {
                            Visual.SetDeletePlayerTriggerMode(VisualElementMode.Hidden);
                        }
                    }
                }
                else // I am CLIENT
                {
                    if (playerModel.isLocalPlayer)
                    {
                        Visual.SetLeavePlayerTriggerMode(VisualElementMode.Interactable);
                        Visual.SetLeavePlayerTriggerCallBack(
                            playerModel.connectionToServer.Disconnect
                        );
                    }
                    else
                    {
                        Visual.SetLeavePlayerTriggerMode(VisualElementMode.Hidden);
                    }
                }
            }
        }
    }
}
