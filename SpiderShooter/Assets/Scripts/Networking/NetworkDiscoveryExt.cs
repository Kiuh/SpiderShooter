using Mirror;
using Mirror.Discovery;
using System;
using System.Net;
using UI;
using UnityEngine;

namespace Networking
{
    public struct ServerResponseExt : NetworkMessage
    {
        public IPEndPoint EndPoint { get; set; }

        public bool IsFullLobby;

        public LobbyMode LobbyMode;

        public Uri Uri;

        public long ServerId;
    }

    public class NetworkDiscoveryExt : NetworkDiscoveryBase<ServerRequest, ServerResponseExt>
    {
        #region Server

        protected override void ProcessClientRequest(ServerRequest request, IPEndPoint endpoint)
        {
            base.ProcessClientRequest(request, endpoint);
        }

        protected override ServerResponseExt ProcessRequest(
            ServerRequest request,
            IPEndPoint endpoint
        )
        {
            try
            {
                return new ServerResponseExt
                {
                    ServerId = ServerId,
                    Uri = transport.ServerUri(),
                    LobbyMode = ServerStorage.Singleton.LobbyMode,
                    IsFullLobby = NetworkRoomManagerExt.Singleton.IsFullLobby,
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
        }

        #endregion

        #region Client

        protected override ServerRequest GetRequest()
        {
            return new ServerRequest();
        }

        protected override void ProcessResponse(ServerResponseExt response, IPEndPoint endpoint)
        {
            response.EndPoint = endpoint;
            UriBuilder realUri = new(response.Uri) { Host = response.EndPoint.Address.ToString() };
            response.Uri = realUri.Uri;
            OnServerFound.Invoke(response);
        }

        #endregion
    }
}
