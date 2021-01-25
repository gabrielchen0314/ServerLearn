using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
    public static Client s_instance;
    public static int s_dataBufferSize = 4096;

    public string m_IP = "127.0.0.1";
    public int m_Port = 26950;
    public int m_MyID = 0;
    public TCP m_TCP;
    public UDP m_UDP;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> s_Dic_packetHandlers;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        m_TCP = new TCP();
        m_UDP = new UDP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();

        m_TCP.Connect();
    }

    public class TCP
    {
        public TcpClient m_Socket;

        private NetworkStream m_Stream;
        private Packet m_ReceivedData;
        private byte[] m_Ay_ReceiveBuffer;

        public void Connect()
        {
            m_Socket = new TcpClient
            {
                ReceiveBufferSize = s_dataBufferSize,
                SendBufferSize = s_dataBufferSize
            };

            m_Ay_ReceiveBuffer = new byte[s_dataBufferSize];
            m_Socket.BeginConnect(s_instance.m_IP, s_instance.m_Port, ConnectCallback, m_Socket);
        }

        private void ConnectCallback(IAsyncResult iResult)
        {
            m_Socket.EndConnect(iResult);

            if (!m_Socket.Connected)
            {
                return;
            }

            m_Stream = m_Socket.GetStream();

            m_ReceivedData = new Packet();

            m_Stream.BeginRead(m_Ay_ReceiveBuffer, 0, s_dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet iPacket)
        {
            try
            {
                if (m_Socket != null)
                {
                    m_Stream.BeginWrite(iPacket.ToArray(), 0, iPacket.Length(), null, null);
                }
            }
            catch (Exception iEx)
            {
                Debug.Log($"Error sending data to server via TCP: {iEx}");
            }
        }

        private void ReceiveCallback(IAsyncResult iResult)
        {
            try
            {
                int _byteLength = m_Stream.EndRead(iResult);
                if (_byteLength <= 0)
                {
                    // TODO: disconnect
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(m_Ay_ReceiveBuffer, _data, _byteLength);

                m_ReceivedData.Reset(HandleData(_data));
                m_Stream.BeginRead(m_Ay_ReceiveBuffer, 0, s_dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                // TODO: disconnect
            }
        }

        private bool HandleData(byte[] i_Ay_data)
        {
            int _packetLength = 0;

            m_ReceivedData.SetBytes(i_Ay_data);

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
                        s_Dic_packetHandlers[_packetId](_packet);
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
    }

    public class UDP
    {
        public UdpClient m_Socket;
        public IPEndPoint m_EndPoint;

        public UDP()
        {
            m_EndPoint = new IPEndPoint(IPAddress.Parse(s_instance.m_IP), s_instance.m_Port);
        }

        public void Connect(int iLocalPort)
        {
            m_Socket = new UdpClient(iLocalPort);

            m_Socket.Connect(m_EndPoint);
            m_Socket.BeginReceive(ReceiveCallback, null);

            using (Packet _Packet = new Packet())
            {
                SendData(_Packet);
            }
        }

        public void SendData(Packet iPacket)
        {
            try
            {
                iPacket.InsertInt(s_instance.m_MyID);
                if(m_Socket != null)
                {
                    m_Socket.BeginSend(iPacket.ToArray(), iPacket.Length(), null, null);
                }
            }
            catch(Exception iEx)
            {
                Debug.Log($"Erroe sending data to server via UDP: {iEx}");
            }
        }

        private void ReceiveCallback(IAsyncResult iResult)
        {
            try
            {
                byte[] _data = m_Socket.EndReceive(iResult, ref m_EndPoint);
                m_Socket.BeginReceive(ReceiveCallback, null);

                if(_data.Length <4)
                {
                    //TODO:disconnect
                    return;
                }

                HandleData(_data);
            }
            catch
            {
                //TODO: disconnect
            }
        }

        private void HandleData(byte[] iData)
        {
            using(Packet _Packet = new Packet(iData))
            {
                int _PacketLength = _Packet.ReadInt();
                iData = _Packet.ReadBytes(_PacketLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _Packet = new Packet(iData))
                {
                    int _PackedID = _Packet.ReadInt();
                    s_Dic_packetHandlers[_PackedID](_Packet);
                }
            });
        }
    }

    private void InitializeClientData()
    {
        s_Dic_packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
        };
        Debug.Log("Initialized packets.");
    }
}
