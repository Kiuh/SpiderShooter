using Assets.Scripts;
using Common;
using Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace UI
{
    [Serializable]
    public enum LobbyMode
    {
        Private,
        Public
    }

    public class OfflineManager : MonoBehaviour
    {
        [SerializeField]
        private NetworkDiscoveryExt networkDiscovery;

        private readonly Dictionary<long, ServerResponseExt> discoveredServers = new();

        [SerializeField]
        private TMP_Text serversCount;

        [SerializeField]
        private TMP_InputField serverCode;

        [SerializeField]
        private TMP_InputField serverName;

        [SerializeField]
        private TMP_InputField playerName;

        [SerializeField]
        private float refreshTime = 2f;

        [SerializeField]
        private ServerStorage serverStoragePrefab;

        [SerializeField]
        [InspectorReadOnly]
        private float timer = 0;

        private void Awake()
        {
            networkDiscovery.StartDiscovery();
        }

        // Called by button
        public void DirectPlay()
        {
            Connect(serverCode.text);
        }

        public void CreateOwnLobby()
        {
            HostOwnServer(LobbyMode.Private);
        }

        public void HostOwnServer(LobbyMode lobbyMode)
        {
            // LAN Host
            NetworkRoomManagerExt.Singleton.networkAddress = GetLocalIPAddress();
            NetworkRoomManagerExt.Singleton.StartHost();
            networkDiscovery.AdvertiseServer();

            ServerStorage storage = Instantiate(serverStoragePrefab);
            storage.Initialize();

            ServerStorage.Singleton.LobbyMode = lobbyMode;
            ServerStorage.Singleton.LobbyName =
                lobbyMode is LobbyMode.Private ? serverName.text : "Random name";
            LocalGlobalData.Singleton.PlayerName = playerName.text;
        }

        // Called by button
        public void PlayRandom()
        {
            IEnumerable<ServerResponseExt> accessedServers = discoveredServers.Values.Where(
                x => x.LobbyMode == LobbyMode.Public
            );
            if (accessedServers.Count() > 0)
            {
                Connect(accessedServers.First());
            }
            else
            {
                HostOwnServer(LobbyMode.Public);
            }
        }

        private void Connect(ServerResponseExt info)
        {
            networkDiscovery.StopDiscovery();
            NetworkRoomManagerExt.Singleton.StartClient(info.Uri);
            LocalGlobalData.Singleton.PlayerName = playerName.text;
        }

        private void Connect(string code)
        {
            networkDiscovery.StopDiscovery();
            NetworkRoomManagerExt.Singleton.networkAddress = "192.168." + code.Replace("-", ".");
            NetworkRoomManagerExt.Singleton.StartClient();
            LocalGlobalData.Singleton.PlayerName = playerName.text;
        }

        // Called by Discovered manager
        public void OnDiscoveredServer(ServerResponseExt info)
        {
            discoveredServers[info.ServerId] = info;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            serversCount.text = $"Servers: {discoveredServers.Count}";
            if (timer <= 0)
            {
                discoveredServers.Clear();
                networkDiscovery.StartDiscovery();
                timer = refreshTime;
            }
        }

        public string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
