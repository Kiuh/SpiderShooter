using AINamesGenerator;
using SpiderShooter.Common;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SpiderShooter.OpeningScene
{
    [AddComponentMenu("SpiderShooter/OpeningScene.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Model model;

        [SerializeField]
        private TMP_InputField defaultNameInput;

        [SerializeField]
        private TMP_InputField joinLobbyNameInput;

        [SerializeField]
        private TMP_InputField lobbyCodeToJointInput;

        [SerializeField]
        private GameObject joinMenuPanel;

        [SerializeField]
        private GameObject errorPanel;

        [SerializeField]
        private TMP_Text errorText;

        public void Awake()
        {
            SetRandomNick();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        // Called by button
        public void ReloadNameButtonClick()
        {
            SetRandomNick();
        }

        private void SetRandomNick()
        {
            do
            {
                defaultNameInput.text = NickGenerator.GetRandomName();
            } while (defaultNameInput.text.Count() > 10);
        }

        // Called by button
        public void ShowJoinMenuButtonClick()
        {
            joinMenuPanel.SetActive(true);
            joinLobbyNameInput.text = defaultNameInput.text;
        }

        // Called by button
        public void JoinLobbyButtonClick()
        {
            Result result = model.JoinLobby(lobbyCodeToJointInput.text, joinLobbyNameInput.text);
            if (result.Failure)
            {
                ShowError(result.Error);
            }
        }

        // Called by button
        public void CreateLobbyButtonClick()
        {
            Result result = model.HostServer(defaultNameInput.text);
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

        // Called by button
        public void Quit()
        {
            Application.Quit();
        }
    }
}
