using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("SpiderShooter/GameScene.Controller")]
    public class Controller : MonoBehaviour
    {
        public static Controller Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
