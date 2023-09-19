using SpiderShooter.Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiderShooter.LobbyScene
{
    [AddComponentMenu("SpiderShooter/LobbyScene.TestView")]
    internal class TestView : MonoBehaviour, IView
    {
        [SerializeField]
        private TMP_Text lobbyCode;

        [SerializeField]
        private TMP_Text lobbyName;

        [SerializeField]
        private TMP_Text lobbyMode;

        [SerializeField]
        private TMP_Text playerMode;

        [SerializeField]
        private Button hostPlayButton;

        [SerializeField]
        private Button hostQuitButton;

        public event Action OnHostPlayTrigger;
        public event Action OnHostQuitTrigger;

        // Called by button
        public void HostPlayButtonClick()
        {
            OnHostPlayTrigger?.Invoke();
        }

        // Called by button
        public void HostQuitButtonClick()
        {
            OnHostQuitTrigger?.Invoke();
        }

        public void SetClientMode()
        {
            playerMode.text = "Client Mode";
        }

        public void SetHostMode()
        {
            playerMode.text = "Host Mode";
        }

        public void SetLobbyCode(string code)
        {
            lobbyCode.text = $"Lobby code: {code}";
        }

        public void SetLobbyMode(LobbyMode lobbyMode)
        {
            this.lobbyMode.text =
                $"Lobby mode: {(lobbyMode == LobbyMode.Public ? "Public" : "Private")}";
        }

        public void SetLobbyName(string name)
        {
            lobbyName.text = $"Lobby name: {name}";
        }

        public void SetPlayTriggerMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    hostPlayButton.gameObject.SetActive(true);
                    hostPlayButton.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    hostPlayButton.gameObject.SetActive(true);
                    hostPlayButton.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    hostPlayButton.gameObject.SetActive(false);
                    hostPlayButton.interactable = true;
                    break;
            }
        }

        public void SetQuitTriggerMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    hostQuitButton.gameObject.SetActive(true);
                    hostQuitButton.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    hostQuitButton.gameObject.SetActive(true);
                    hostQuitButton.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    hostQuitButton.gameObject.SetActive(false);
                    hostQuitButton.interactable = true;
                    break;
            }
        }
    }
}
