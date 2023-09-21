using SpiderShooter.Common;
using SpiderShooter.General;
using SpiderShooter.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace SpiderShooter.OpeningScene
{
    [AddComponentMenu("SpiderShooter/OpeningScene.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private ServerDiscovery networkDiscovery;

        [SerializeField]
        private float refreshTime = 2f;

        [SerializeField]
        [InspectorReadOnly]
        private float timer = 0;

        private readonly Dictionary<long, ServerResponseExt> discoveredServers = new();

        private void Awake()
        {
            networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
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

        public Result HostServer(string playerName)
        {
            try
            {
                Result<string> result = GetLocalIPAddress();
                if (result.Failure)
                {
                    return new FailResult(result.Error);
                }

                RoomManager.Singleton.networkAddress = result.Data;
                RoomManager.Singleton.StartHost();
                networkDiscovery.AdvertiseServer();

                ServerStorage.Singleton.LobbyCode = result.Data
                    .Split('.')
                    .Skip(2)
                    .Aggregate((x, y) => x + "-" + y);

                LocalClientData.Singleton.PlayerName = playerName;
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                return new FailResult(ex.Message);
            }
        }

        public Result JoinLobby(string code, string playerName)
        {
            string dnsSaveHost = "192.168." + code.Replace("-", ".");
            ServerResponseExt server = discoveredServers.Values.FirstOrDefault(
                x => x.Uri.DnsSafeHost == dnsSaveHost
            );
            return server.Uri == null
                ? new FailResult("Cannot find server with this code")
                : ConnectToServer(server.Uri, playerName);
        }

        private Result ConnectToServer(Uri uri, string playerName)
        {
            try
            {
                networkDiscovery.StopDiscovery();
                RoomManager.Singleton.StartClient(uri);
                LocalClientData.Singleton.PlayerName = playerName;
                return new SuccessResult();
            }
            catch (Exception ex)
            {
                networkDiscovery.StartDiscovery();
                return new FailResult(ex.Message);
            }
        }

        public void OnDiscoveredServer(ServerResponseExt info)
        {
            discoveredServers[info.ServerId] = info;
        }

        private Result<string> GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return new SuccessResult<string>(ip.ToString());
                }
            }
            return new FailResult<string>(
                "No network adapters with an IPv4 address in the system!"
            );
        }
    }
}
