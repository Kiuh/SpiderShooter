using Common;
using Mirror;
using Networking;
using UnityEngine;

namespace InGameNetworking
{
    public class NetworkStartPositionByTeam : MonoBehaviour
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
