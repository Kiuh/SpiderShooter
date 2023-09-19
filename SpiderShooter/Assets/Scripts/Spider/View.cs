using TMPro;
using UnityEngine;

namespace SpiderShooter.Spider
{
    [AddComponentMenu("SpiderShooter/Spider.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerInfo;

        [SerializeField]
        private SpiderImpl spider;

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
            playerInfo.text =
                $"{spider.PlayerName}\n{new string('-', (int)(spider.Health / 20))}HP";
        }
    }
}
