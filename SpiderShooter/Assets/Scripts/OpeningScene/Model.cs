using Assets.Scripts;
using AYellowpaper;
using Common;
using Networking;
using SpiderShooter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace SpiderShooter.OpeningScene
{
    [AddComponentMenu("SpiderShooter.OpeningScene.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private NetworkDiscoveryExt networkDiscovery;

        [SerializeField]
        private ServerStorage serverStoragePrefab;

        [SerializeField]
        private InterfaceReference<IView> view;

        [SerializeField]
        private float refreshTime = 2f;

        [SerializeField]
        [InspectorReadOnly]
        private float timer = 0;

        private readonly Dictionary<long, ServerResponseExt> discoveredServers = new();

        private void Awake()
        {
            view.Value.OnJoinLobby += () => JoinLobby(view.Value.CodeToJoinLobby);
            view.Value.OnCreateLobby += () => HostServer(LobbyMode.Private);
            view.Value.OnPlayRandomLobby += PlayRandomLobby;

            networkDiscovery.StartDiscovery();
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                discoveredServers.Clear();
                networkDiscovery.StartDiscovery();
                timer = refreshTime;
            }
        }

        private void HostServer(LobbyMode lobbyMode)
        {
            try
            {
                // LAN Host
                NetworkRoomManagerExt.Singleton.networkAddress = GetLocalIPAddress();
                NetworkRoomManagerExt.Singleton.StartHost();
                networkDiscovery.AdvertiseServer();

                ServerStorage storage = Instantiate(serverStoragePrefab);
                storage.Initialize();

                ServerStorage.Singleton.LobbyMode = lobbyMode;
                ServerStorage.Singleton.LobbyName =
                    lobbyMode == LobbyMode.Private
                        ? view.Value.ServerNameForCreating
                        : GenerateRandomLobbyName();
                LocalGlobalData.Singleton.PlayerName = view.Value.PlayerName;
            }
            catch
            {
                view.Value.ShowErrorDialog("One computer cannot have more than one server.");
            }
        }

        private void PlayRandomLobby()
        {
            ServerResponseExt? accessedServers = discoveredServers.Values.FirstOrDefault(
                x => x.LobbyMode == LobbyMode.Public && !x.IsFullLobby
            );
            if (accessedServers != null)
            {
                ConnectToServer(accessedServers.Value.Uri);
            }
            else
            {
                HostServer(LobbyMode.Public);
            }
        }

        private void JoinLobby(string code)
        {
            string dnsSaveHost = "192.168." + code.Replace("-", ".");
            ServerResponseExt? server = discoveredServers.Values.FirstOrDefault(
                x => x.LobbyMode == LobbyMode.Private && x.Uri.DnsSafeHost == dnsSaveHost
            );
            if (server == null)
            {
                view.Value.ShowErrorDialog("Cannot find server with this code");
            }
            else
            {
                ConnectToServer(server.Value.Uri);
            }
        }

        private void ConnectToServer(Uri uri)
        {
            networkDiscovery.StopDiscovery();
            NetworkRoomManagerExt.Singleton.StartClient(uri);
            LocalGlobalData.Singleton.PlayerName = view.Value.PlayerName;
        }

        // Called by Discovered manager
        private void OnDiscoveredServer(ServerResponseExt info)
        {
            discoveredServers[info.ServerId] = info;
        }

        private string GenerateRandomLobbyName()
        {
            // TODO: implement random generator
            return "Random name";
        }

        private string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
