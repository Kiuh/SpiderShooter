using SpiderShooter.Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpiderShooter.LobbyScene.Player
{
    [AddComponentMenu("SpiderShooter/LobbyScene.Player.TestView")]
    public class TestView : MonoBehaviour, IView
    {
        [SerializeField]
        private Toggle isReady;

        [SerializeField]
        private TMP_Dropdown teamColor;

        [SerializeField]
        private TMP_Text playerName;

        [SerializeField]
        private TMP_Text itsYou;

        [SerializeField]
        private Button technicalButton;

        [SerializeField]
        private TMP_Text buttonText;

        public void SetDeletePlayerTriggerCallBack(Action action)
        {
            buttonText.text = "Delete Player";
            technicalButton.onClick.RemoveAllListeners();
            technicalButton.onClick.AddListener(() => action?.Invoke());
        }

        public void SetDeletePlayerTriggerMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    technicalButton.gameObject.SetActive(true);
                    technicalButton.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    technicalButton.gameObject.SetActive(true);
                    technicalButton.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    technicalButton.gameObject.SetActive(false);
                    technicalButton.interactable = true;
                    break;
            }
        }

        public void SetLeavePlayerTriggerCallBack(Action action)
        {
            buttonText.text = "Leave";
            technicalButton.onClick.RemoveAllListeners();
            technicalButton.onClick.AddListener(() => action?.Invoke());
        }

        public void SetLeavePlayerTriggerMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    technicalButton.gameObject.SetActive(true);
                    technicalButton.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    technicalButton.gameObject.SetActive(true);
                    technicalButton.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    technicalButton.gameObject.SetActive(false);
                    technicalButton.interactable = true;
                    break;
            }
        }

        public void SetLocalPlayer(bool value)
        {
            itsYou.gameObject.SetActive(value);
        }

        public void SetPlayerName(string name)
        {
            playerName.text = $"P.N. {name}";
        }

        public void SetReadyState(bool value)
        {
            isReady.isOn = value;
        }

        public void SetReadyStateSelectionCallBack(Action<bool> action)
        {
            isReady.onValueChanged.RemoveAllListeners();
            isReady.onValueChanged.AddListener((value) => action?.Invoke(value));
        }

        public void SetReadyStateSelectionMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    isReady.gameObject.SetActive(true);
                    isReady.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    isReady.gameObject.SetActive(true);
                    isReady.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    isReady.gameObject.SetActive(false);
                    isReady.interactable = true;
                    break;
            }
        }

        public void SetTeamColor(TeamColor color)
        {
            teamColor.value = (int)color;
        }

        public void SetTeamColorSelectionCallBack(Action<TeamColor> action)
        {
            teamColor.onValueChanged.RemoveAllListeners();
            teamColor.onValueChanged.AddListener((value) => action?.Invoke((TeamColor)value));
        }

        public void SetTeamColorSelectionMode(VisualElementMode visualElementMode)
        {
            switch (visualElementMode)
            {
                case VisualElementMode.Interactable:
                    teamColor.gameObject.SetActive(true);
                    teamColor.interactable = true;
                    break;
                case VisualElementMode.ReadOnly:
                    teamColor.gameObject.SetActive(true);
                    teamColor.interactable = false;
                    break;
                case VisualElementMode.Hidden:
                    teamColor.gameObject.SetActive(false);
                    teamColor.interactable = true;
                    break;
            }
        }
    }
}
