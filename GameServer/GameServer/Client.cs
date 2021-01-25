
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    class Client
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

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                m_Socket = _socket;
                m_Socket.ReceiveBufferSize = dataBufferSize;
                m_Socket.SendBufferSize = dataBufferSize;

                stream = m_Socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (m_Socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        // TODO: disconnect
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {_ex}");
                    // TODO: disconnect
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.s_Dic_PacketHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
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
                    using(Packet _Packet = new Packet(_PacketBytes))
                    {
                        int _PacketID = _Packet.ReadInt();
                        Server.s_Dic_PacketHandlers[_PacketID](m_ID, _Packet);
                    }
                });
            }
        }

        public void SendIntoGame(string iPlayerName)
        {
            m_Player = new Player(m_ID,iPlayerName,new Vector3(0,0,0));

            foreach(Client _client in Server.s_Dic_Clients.Values)
            {
                if(_client.m_Player != null)
                {
                    if(_client.m_ID != m_ID)
                    {
                        ServerSend.SpawnPlayer(m_ID, _client.m_Player);
                    }
                }
            }

            foreach (Client _client in Server.s_Dic_Clients.Values)
            {
                if(_client.m_Player != null)
                {
                    ServerSend.SpawnPlayer(_client.m_ID,m_Player);
                }
            }
        }
    }
}
