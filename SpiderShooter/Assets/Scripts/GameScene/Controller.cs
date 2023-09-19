using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.GameScene
{
    [AddComponentMenu("GameScene.Controller")]
    public class Controller : MonoBehaviour
    {
        public static Controller Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int redTeamKillCount = 0;
        public int RedTeamKillCount
        {
            get => redTeamKillCount;
            set => redTeamKillCount = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int blueTeamKillCount = 0;
        public int BlueTeamKillCount
        {
            get => blueTeamKillCount;
            set => blueTeamKillCount = value;
        }
    }
}
