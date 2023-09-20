using System;
using TMPro;
using UnityEngine;

namespace SpiderShooter.OpeningScene
{
    [AddComponentMenu("SpiderShooter/OpeningScene.TestView")]
    public class TestView : MonoBehaviour, IView
    {
        [SerializeField]
        private TMP_InputField codeToJoinLobby;

        [SerializeField]
        private TMP_InputField serverNameForCreating;

        [SerializeField]
        private TMP_InputField playerName;

        public string CodeToJoinLobby => codeToJoinLobby.text;

        public string ServerNameForCreating => serverNameForCreating.text;

        public string PlayerName => playerName.text;

        public event Action OnJoinLobby;
        public event Action OnCreateLobby;
        public event Action OnPlayRandomLobby;

        // Called by button
        public void OnJoinLobbyButtonPress()
        {
            OnJoinLobby?.Invoke();
        }

        // Called by button
        public void OnCreateLobbyButtonPress()
        {
            OnCreateLobby?.Invoke();
        }

        // Called by button
        public void OnPlayRandomLobbyButtonPress()
        {
            OnPlayRandomLobby?.Invoke();
        }

        public void ShowErrorDialog(string message)
        {
            Debug.Log(message);
        }
    }
}
