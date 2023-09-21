using SpiderShooter.Networking;
using TMPro;
using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("SpiderShooter/GameScene.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text countText;

        private void Update()
        {
            countText.text =
                $"<color=#FF0000>{ServerStorage.Singleton.RedTeamKillCount}</color>"
                + $" - <color=#0000FF>{ServerStorage.Singleton.BlueTeamKillCount}</color>";
        }
    }
}
