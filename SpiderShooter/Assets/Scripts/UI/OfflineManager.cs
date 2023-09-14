using Mirror.Discovery;
using Networking;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

namespace UI
{
    public class OfflineManager : MonoBehaviour
    {
        [SerializeField]
        private NetworkDiscovery networkDiscovery;

        private readonly Dictionary<long, ServerResponse> discoveredServers = new();

        [SerializeField]
        private TMP_Text serversCount;

        [SerializeField]
        private TMP_InputField serverName;

        private void Awake()
        {
            networkDiscovery.StartDiscovery();
        }

        // Called by button
        public void DirectPlay()
        {
            Connect(serverName.text);
        }

        // Called by button
        public void HostOwnServer()
        {
            // LAN Host
            NetworkRoomManagerExt.Singleton.networkAddress = GetLocalIPAddress();
            NetworkRoomManagerExt.Singleton.StartHost();
            networkDiscovery.AdvertiseServer();
            Debug.Log($"Server started on:\n {GetLocalIPAddress()}");
        }

        // Called by button
        public void PlayRandom()
        {
            if (discoveredServers.Count > 0)
            {
                Connect(discoveredServers.First().Value);
            }
            else
            {
                HostOwnServer();
            }
        }

        // Called by button
        public void RefreshServers()
        {
            discoveredServers.Clear();
            networkDiscovery.StartDiscovery();
        }

        private void Connect(ServerResponse info)
        {
            networkDiscovery.StopDiscovery();
            NetworkRoomManagerExt.Singleton.StartClient(info.uri);
        }

        private void Connect(string ip)
        {
            networkDiscovery.StopDiscovery();
            NetworkRoomManagerExt.Singleton.networkAddress = ip;
            NetworkRoomManagerExt.Singleton.StartClient();
        }

        public void OnDiscoveredServer(ServerResponse info)
        {
            discoveredServers[info.serverId] = info;
        }

        private void Update()
        {
            serversCount.text = $"Servers: {discoveredServers.Count}";
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
