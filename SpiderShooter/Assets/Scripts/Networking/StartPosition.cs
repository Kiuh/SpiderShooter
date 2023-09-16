using Mirror;
using SpiderShooter.Common;
using UnityEngine;

namespace SpiderShooter.Networking
{
    [AddComponentMenu("Networking.StartPosition")]
    public class StartPosition : MonoBehaviour
    {
        [SerializeField]
        private TeamColor teamColor;
        public TeamColor TeamColor => teamColor;

        [SerializeField]
        [InspectorReadOnly]
        private bool isBusy;
        public bool IsBusy => isBusy;

        public void Awake()
        {
            NetworkManager.RegisterStartPosition(transform);
        }

        public void OnDestroy()
        {
            NetworkManager.UnRegisterStartPosition(transform);
        }
    }
}
