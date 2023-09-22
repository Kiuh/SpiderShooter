using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.General
{
    [AddComponentMenu("SpiderShooter/General.LocalClientData")]
    public class LocalClientData : MonoBehaviour
    {
        public static LocalClientData Singleton { get; private set; }

        public void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
            }
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        [InspectorReadOnly]
        public string BufferIP;

        [SerializeField]
        [InspectorReadOnly]
        private string playerName = "No player name";
        public string PlayerName
        {
            get => playerName;
            set => playerName = value;
        }
    }
}
