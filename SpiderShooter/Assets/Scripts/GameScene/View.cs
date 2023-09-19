using TMPro;
using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("GameScene.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text countText;

        [SerializeField]
        private Controller controller;

        private void Update()
        {
            countText.text =
                $"<color=#FF0000>{controller.RedTeamKillCount}</color>"
                + $" - <color=#0000FF>{controller.BlueTeamKillCount}</color>";
        }
    }
}
