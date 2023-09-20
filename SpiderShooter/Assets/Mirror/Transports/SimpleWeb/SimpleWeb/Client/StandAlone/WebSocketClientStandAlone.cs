using System;
using System.Net.Sockets;
using System.Threading;

namespace Mirror.SimpleWeb
{
    public class WebSocketClientStandAlone : SimpleWebClient
    {
        private readonly ClientSslHelper sslHelper;
        private readonly ClientHandshake handshake;
        private readonly TcpConfig tcpConfig;
        private Connection conn;

        internal WebSocketClientStandAlone(
            int maxMessageSize,
            int maxMessagesPerTick,
            TcpConfig tcpConfig
        )
            : base(maxMessageSize, maxMessagesPerTick)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            throw new NotSupportedException();
#else
            sslHelper = new ClientSslHelper();
            handshake = new ClientHandshake();
            this.tcpConfig = tcpConfig;
#endif
        }

        public override void Connect(Uri serverAddress)
        {
            state = ClientState.Connecting;

            // create connection here before thread so that send queue exist before connected
            TcpClient client = new();
            tcpConfig.ApplyTo(client);

            // create connection object here so dispose correctly disconnects on failed connect
            conn = new Connection(client, AfterConnectionDisposed);

            Thread receiveThread =
                new(() => ConnectAndReceiveLoop(serverAddress)) { IsBackground = true };
            receiveThread.Start();
        }

        private void ConnectAndReceiveLoop(Uri serverAddress)
        {
            try
            {
                // connection created above
                TcpClient client = conn.client;
                conn.receiveThread = Thread.CurrentThread;

                try
                {
                    client.Connect(serverAddress.Host, serverAddress.Port);
                }
                catch (SocketException)
                {
                    client.Dispose();
                    throw;
                }

                bool success = sslHelper.TryCreateStream(conn, serverAddress);
                if (!success)
                {
                    Log.Warn($"[SimpleWebTransport] Failed to create Stream with {serverAddress}");
                    conn.Dispose();
                    return;
                }

                success = handshake.TryHandshake(conn, serverAddress);
                if (!success)
                {
                    Log.Warn($"[SimpleWebTransport] Failed Handshake with {serverAddress}");
                    conn.Dispose();
                    return;
                }

                Log.Info($"[SimpleWebTransport] HandShake Successful with {serverAddress}");

                state = ClientState.Connected;

                receiveQueue.Enqueue(new Message(EventType.Connected));

                Thread sendThread =
                    new(() =>
                    {
                        SendLoop.Config sendConfig =
                            new(
                                conn,
                                bufferSize: Constants.HeaderSize
                                    + Constants.MaskSize
                                    + maxMessageSize,
                                setMask: true
                            );

                        SendLoop.Loop(sendConfig);
                    });

                conn.sendThread = sendThread;
                sendThread.IsBackground = true;
                sendThread.Start();

                ReceiveLoop.Config config =
                    new(conn, maxMessageSize, false, receiveQueue, bufferPool);
                ReceiveLoop.Loop(config);
            }
            catch (ThreadInterruptedException e)
            {
                Log.InfoException(e);
            }
            catch (ThreadAbortException e)
            {
                Log.InfoException(e);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
            finally
            {
                // close here in case connect fails
                conn?.Dispose();
            }
        }

        private void AfterConnectionDisposed(Connection conn)
        {
            state = ClientState.NotConnected;
            // make sure Disconnected event is only called once
            receiveQueue.Enqueue(new Message(EventType.Disconnected));
        }

        public override void Disconnect()
        {
            state = ClientState.Disconnecting;
            Log.Info("[SimpleWebTransport] Disconnect Called");

            if (conn == null)
            {
                state = ClientState.NotConnected;
            }
            else
            {
                conn?.Dispose();
            }
        }

        public override void Send(ArraySegment<byte> segment)
        {
            ArrayBuffer buffer = bufferPool.Take(segment.Count);
            buffer.CopyFrom(segment);

            conn.sendQueue.Enqueue(buffer);
            conn.sendPending.Set();
        }
    }
}
