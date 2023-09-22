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

        private void Start()
        {
            if (spider.isLocalPlayer)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            transform.LookAt(Camera.main.transform);
            if (spider.Health > 0)
            {
                playerInfo.text = $"{spider.PlayerName}\n{(int)spider.Health}";
            }
        }
    }
}
