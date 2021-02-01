using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;

    public int m_ID;
    public Player m_Player;
    public TCP m_TCP;
    public UDP m_UDP;

    public Client(int _clientId)
    {
        m_ID = _clientId;
        m_TCP = new TCP(m_ID);
        m_UDP = new UDP(m_ID);
    }

    public class TCP
    {
        public TcpClient m_Socket;

        private readonly int m_ID;
        private NetworkStream m_Stream;
        private Packet m_ReceivedData;
        private byte[] m_ReceiveBuffer;

        public TCP(int _id)
        {
            m_ID = _id;
        }

        public void Connect(TcpClient _socket)
        {
            m_Socket = _socket;
            m_Socket.ReceiveBufferSize = dataBufferSize;
            m_Socket.SendBufferSize = dataBufferSize;

            m_Stream = m_Socket.GetStream();

            m_ReceivedData = new Packet();
            m_ReceiveBuffer = new byte[dataBufferSize];

            m_Stream.BeginRead(m_ReceiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(m_ID, "Welcome to the server!");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (m_Socket != null)
                {
                    m_Stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {m_ID} via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = m_Stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.s_Dic_Clients[m_ID].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(m_ReceiveBuffer, _data, _byteLength);

                m_ReceivedData.Reset(HandleData(_data));
                m_Stream.BeginRead(m_ReceiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Server.s_Dic_Clients[m_ID].Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            m_ReceivedData.SetBytes(_data);

            if (m_ReceivedData.UnreadLength() >= 4)
            {
                _packetLength = m_ReceivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= m_ReceivedData.UnreadLength())
            {
                byte[] _packetBytes = m_ReceivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.s_Dic_PacketHandlers[_packetId](m_ID, _packet);
                    }
                });

                _packetLength = 0;
                if (m_ReceivedData.UnreadLength() >= 4)
                {
                    _packetLength = m_ReceivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            m_Socket.Close();
            m_Stream = null;
            m_ReceivedData = null;
            m_ReceiveBuffer = null;
            m_Socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint m_EndPoint;

        private int m_ID;

        public UDP(int iID)
        {
            m_ID = iID;
        }

        public void Connect(IPEndPoint iEndPoint)
        {
            m_EndPoint = iEndPoint;
        }

        public void SendData(Packet iPacket)
        {
            Server.SendUDPData(m_EndPoint, iPacket);
        }

        public void HandleData(Packet iPacket)
        {
            int _PacketLength = iPacket.ReadInt();
            byte[] _PacketBytes = iPacket.ReadBytes(_PacketLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _Packet = new Packet(_PacketBytes))
                {
                    int _PacketID = _Packet.ReadInt();
                    Server.s_Dic_PacketHandlers[_PacketID](m_ID, _Packet);
                }
            });
        }

        public void Disconnect()
        {
            m_EndPoint = null;
        }
    }

    public void SendIntoGame(string iPlayerName)
    {
        m_Player = NetworkManager.s_Instance.InstantiatePlayer();
        m_Player.Initalize(m_ID, iPlayerName);


        foreach (Client _client in Server.s_Dic_Clients.Values)
        {
            if (_client.m_Player != null)
            {
                if (_client.m_ID != m_ID)
                {
                    ServerSend.SpawnPlayer(m_ID, _client.m_Player);
                }
            }
        }

        foreach (Client _client in Server.s_Dic_Clients.Values)
        {
            if (_client.m_Player != null)
            {
                ServerSend.SpawnPlayer(_client.m_ID, m_Player);
            }
        }

        foreach(ItemSpawner _ItemSpawner in ItemSpawner.s_Dic_Spawners.Values)
        {
            ServerSend.CreateItemSpawner(m_ID, _ItemSpawner.m_SpawnerID, _ItemSpawner.transform.position, _ItemSpawner.m_IsHasItem);
        }
    }

    public void Disconnect()
    {
        Debug.Log($"{m_TCP.m_Socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(m_Player.gameObject);
        });

        UnityEngine.Object.Destroy(m_Player.gameObject);
        m_Player = null;

        m_TCP.Disconnect();
        m_UDP.Disconnect();

        ServerSend.PlayerDisconnected(m_ID);
    }
}
