using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class Server
{
    public static int s_MaxPlayers { get; private set; }
    public static int s_Port { get; private set; }
    public static Dictionary<int, Client> s_Dic_Clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> s_Dic_PacketHandlers;

    private static TcpListener m_TcpListener;
    private static UdpClient m_UdpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        s_MaxPlayers = _maxPlayers;
        s_Port = _port;

        Debug.Log("Starting server...");
        InitializeServerData();

        m_TcpListener = new TcpListener(IPAddress.Any, s_Port);
        m_TcpListener.Start();
        m_TcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        m_UdpListener = new UdpClient(s_Port);
        m_UdpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on port {s_Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult iResult)
    {
        TcpClient _client = m_TcpListener.EndAcceptTcpClient(iResult);
        m_TcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= s_MaxPlayers; i++)
        {
            if (s_Dic_Clients[i].m_TCP.m_Socket == null)
            {
                s_Dic_Clients[i].m_TCP.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult iResult)
    {
        try
        {
            IPEndPoint _ClientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = m_UdpListener.EndReceive(iResult, ref _ClientEndPoint);
            m_UdpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _Packet = new Packet(_data))
            {
                int _ClientID = _Packet.ReadInt();

                if (_ClientID == 0)
                {
                    return;
                }

                if (s_Dic_Clients[_ClientID].m_UDP.m_EndPoint == null)
                {
                    s_Dic_Clients[_ClientID].m_UDP.Connect(_ClientEndPoint);
                    return;
                }

                if (s_Dic_Clients[_ClientID].m_UDP.m_EndPoint.ToString() == _ClientEndPoint.ToString())
                {
                    s_Dic_Clients[_ClientID].m_UDP.HandleData(_Packet);
                }
            }
        }
        catch (Exception iEx)
        {
            Debug.Log($"Error receiving UDP data: {iEx}");
        }
    }

    public static void SendUDPData(IPEndPoint iClientEndPoint, Packet iPacket)
    {
        try
        {
            if (iClientEndPoint != null)
            {
                m_UdpListener.BeginSend(iPacket.ToArray(), iPacket.Length(), iClientEndPoint, null, null);
            }
        }
        catch (Exception iEx)
        {
            Debug.Log($"Error sending data to {iClientEndPoint} via UDP:{iEx}");
        }
    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= s_MaxPlayers; i++)
        {
            s_Dic_Clients.Add(i, new Client(i));
        }

        s_Dic_PacketHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovent },
                { (int)ClientPackets.playerShoot, ServerHandle.PlayerShoot },
            };
        Debug.Log("Initialized packets.");
    }

    public static void Stop()
    {
        m_TcpListener.Stop();
        m_UdpListener.Close();
    }
}
