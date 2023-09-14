using UnityEngine;

namespace Assets.Scripts
{
    public class LocalGlobalData : MonoBehaviour
    {
        public static LocalGlobalData Singleton { get; private set; }

        public void Awake()
        {
            if (Singleton != null)
            {
                Destroy(this);
            }
            Singleton = this;
            DontDestroyOnLoad(this);
        }

        public string PlayerName = "";
    }
}
